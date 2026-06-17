#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Reallusion.Import
{
    [Serializable]
    public struct DissolveMaterialSettings
    {
        public int RenderQueue;
        public bool EnableInstancing;
        public bool DoubleSidedGI;
        public int GlobalIlluminationFlagsInt;
        public float QueueOffset;
        public float QueueControl;
        public bool HasQueueOffset;
        public bool HasQueueControl;

        public static DissolveMaterialSettings Capture(Material material)
        {
            var settings = new DissolveMaterialSettings
            {
                RenderQueue = material.renderQueue,
                EnableInstancing = material.enableInstancing,
                DoubleSidedGI = material.doubleSidedGI,
                GlobalIlluminationFlagsInt = (int)material.globalIlluminationFlags,
            };

            if (material.HasProperty("_QueueOffset"))
            {
                settings.HasQueueOffset = true;
                settings.QueueOffset = material.GetFloat("_QueueOffset");
            }

            if (material.HasProperty("_QueueControl"))
            {
                settings.HasQueueControl = true;
                settings.QueueControl = material.GetFloat("_QueueControl");
            }

            return settings;
        }

        public void Apply(Material material)
        {
            material.renderQueue = RenderQueue;
            material.enableInstancing = EnableInstancing;
            material.doubleSidedGI = DoubleSidedGI;
            material.globalIlluminationFlags = (MaterialGlobalIlluminationFlags)GlobalIlluminationFlagsInt;

            if (HasQueueOffset && material.HasProperty("_QueueOffset"))
                material.SetFloat("_QueueOffset", QueueOffset);

            if (HasQueueControl && material.HasProperty("_QueueControl"))
                material.SetFloat("_QueueControl", QueueControl);

            EditorUtility.SetDirty(material);
        }
    }

    [InitializeOnLoad]
    public static class DissolveMaterialDefaults
    {
        // Only materials using these shaders are managed by this helper.
        static readonly string[] DissolveShaderNames =
        {
            "Reallusion/Amplify/RL_SkinShader_Variants_URP_Dissolve",
            "Reallusion/Amplify/RL_CorneaShaderParallax_URP_Dissolve",
            "Reallusion/Amplify/RL_TeethShader_URP_Dissolve",
            "Reallusion/Amplify/RL_TongueShader_URP_Dissolve",
            "Reallusion/Amplify/RL_HairShader_2nd_Pass_Variants_URP_Dissolve",
            "Reallusion/Amplify/RL_HairShader_1st_Pass_Variants_URP_Dissolve",
            "Universal Render Pipeline/Lit_Dissolve",
            "Shader Graphs/RL_EyeOcclusionShader_URP wDissolve",
            "Shader Graphs/RL_TearlineShader_URP wDissolve",
            "Shader Graphs/RL_EyeShaderParallax_Dummy_URP wDissolve",
        };

        static readonly string[] DissolveShaderGuids =
        {
            "5fd40f0e0a263bd41af2d914b71bbf21",
            "8b3518b880e941b4f8418dceadf5584a",
            "ee6fae872707aa2468d33e9691b52fea",
            "10e493368a040934980f01c473be07b9",
            "1b33dc92f592de74e835c6daacbe1c3f",
            "f935d64899736dd4495f284482b3f6d8",
            "968aaff2158a4b04fbfaa479777b79f7",
            "ad8a75b659697f94d83b3ebb4fd3ba35",
            "413716a9208fa1a4587e67ea610aa0bc",
            "ad3331f0a69372649b743e0ffebf237a",
        };

        // skip specific material paths even when they use a managed dissolve shader.
        static readonly string[] IgnoredMaterialPathContains =
        {
        };

        const string NoiseTexProperty = "_NoiseTex";
        const string DefaultNoiseTexPath =
            "Assets/Shaders/DissolveShaders/noiseTexs/NoiseEffectGrain.png";
        const string SettingsPrefsPrefix = "Reallusion.DissolveMatSettings.";
        const int MaterialsPerFrame = 40;

        static Texture2D _defaultNoise;
        static readonly Dictionary<int, string> LastSeenShader = new Dictionary<int, string>();
        static readonly Dictionary<int, DissolveMaterialSettings> LastSeenSettings = new Dictionary<int, DissolveMaterialSettings>();
        static Queue<string> _pendingMaterialGuids;
        static bool _primeUpdateRegistered;

        static DissolveMaterialDefaults()
        {
            ObjectChangeEvents.changesPublished += OnChangesPublished;
            EditorApplication.delayCall += SchedulePrimeAllMaterials;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static Texture2D DefaultNoise =>
            _defaultNoise != null
                ? _defaultNoise
                : (_defaultNoise = AssetDatabase.LoadAssetAtPath<Texture2D>(DefaultNoiseTexPath));

        public static bool IsDissolveShader(string shaderName)
        {
            return IsDissolveShaderName(shaderName);
        }

        static bool IsDissolveShaderName(string shaderName)
        {
            if (string.IsNullOrEmpty(shaderName))
                return false;

            for (int i = 0; i < DissolveShaderNames.Length; i++)
            {
                if (DissolveShaderNames[i] == shaderName)
                    return true;
            }

            return false;
        }

        public static bool IsDissolveShader(Shader shader)
        {
            if (shader == null)
                return false;

            if (IsDissolveShaderName(shader.name))
                return true;

            string path = AssetDatabase.GetAssetPath(shader);
            if (string.IsNullOrEmpty(path))
                return false;

            string guid = AssetDatabase.AssetPathToGUID(path);
            for (int i = 0; i < DissolveShaderGuids.Length; i++)
            {
                if (DissolveShaderGuids[i] == guid)
                    return true;
            }

            return false;
        }

        public static bool ShouldManageMaterial(Material material)
        {
            if (material == null || material.shader == null)
                return false;

            if (!IsDissolveShader(material.shader))
                return false;

            string path = AssetDatabase.GetAssetPath(material);
            if (string.IsNullOrEmpty(path))
                return true;

            for (int i = 0; i < IgnoredMaterialPathContains.Length; i++)
            {
                if (path.IndexOf(IgnoredMaterialPathContains[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return false;
            }

            return true;
        }

        public static string GetMaterialKey(Material material)
        {
            string path = AssetDatabase.GetAssetPath(material);
            if (!string.IsNullOrEmpty(path))
                return AssetDatabase.AssetPathToGUID(path);

            return "instance_" + material.GetInstanceID();
        }

        public static void HandleShaderAssignment(Material material, Shader oldShader, Shader newShader, Action assignShader)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            var before = DissolveMaterialSettings.Capture(material);
            bool toDissolve = IsDissolveShader(newShader);
            bool fromDissolve = IsDissolveShader(oldShader);
            string key = GetMaterialKey(material);

            assignShader?.Invoke();

            if (toDissolve)
            {
                RestoreOnDissolveAssignment(material, before);
            }
            else if (fromDissolve)
            {
                SaveSettings(key, before);
            }

            UpdateTracking(material);
        }

        public static void ApplyDefaults(Material material)
        {
            if (!ShouldManageMaterial(material))
                return;

            TrackMaterial(material);
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode)
                SchedulePrimeAllMaterials();
        }

        static void SchedulePrimeAllMaterials()
        {
            if (_primeUpdateRegistered)
                return;

            // Wait until the play-mode transition finishes so Play is not blocked.
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                return;

            _pendingMaterialGuids = new Queue<string>();
            foreach (var guid in AssetDatabase.FindAssets("t:Material"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (ShouldPrimeMaterialPath(path))
                    _pendingMaterialGuids.Enqueue(guid);
            }

            if (_pendingMaterialGuids.Count == 0)
                return;

            _primeUpdateRegistered = true;
            EditorApplication.update += ProcessPrimeBatch;
        }

        static bool ShouldPrimeMaterialPath(string path)
        {
            if (string.IsNullOrEmpty(path) || !path.EndsWith(".mat", StringComparison.OrdinalIgnoreCase))
                return false;

            for (int i = 0; i < IgnoredMaterialPathContains.Length; i++)
            {
                if (path.IndexOf(IgnoredMaterialPathContains[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return false;
            }

            string materialSource;
            try
            {
                materialSource = File.ReadAllText(path);
            }
            catch (IOException)
            {
                return false;
            }

            for (int i = 0; i < DissolveShaderGuids.Length; i++)
            {
                if (materialSource.IndexOf(DissolveShaderGuids[i], StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        static void ProcessPrimeBatch()
        {
            if (EditorApplication.isCompiling)
                return;

            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                return;

            int processed = 0;
            while (_pendingMaterialGuids.Count > 0 && processed < MaterialsPerFrame)
            {
                var guid = _pendingMaterialGuids.Dequeue();
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (ShouldManageMaterial(material))
                    TrackMaterial(material);

                processed++;
            }

            if (_pendingMaterialGuids.Count == 0)
            {
                EditorApplication.update -= ProcessPrimeBatch;
                _primeUpdateRegistered = false;
                _pendingMaterialGuids = null;
            }
        }

        static void TrackMaterial(Material material)
        {
            int id = material.GetInstanceID();
            string shaderName = material.shader.name;

            if (LastSeenShader.TryGetValue(id, out string previousShader) && previousShader != shaderName)
            {
                if (IsDissolveShader(material.shader))
                {
                    LastSeenSettings.TryGetValue(id, out DissolveMaterialSettings previousSettings);
                    RestoreOnDissolveAssignment(material, previousSettings);
                }
                else if (IsDissolveShaderName(previousShader))
                {
                    LastSeenSettings.TryGetValue(id, out DissolveMaterialSettings dissolveSettings);
                    SaveSettings(GetMaterialKey(material), dissolveSettings);
                }
            }

            UpdateTracking(material);

            if (IsDissolveShader(material.shader))
            {
                ApplyNoiseDefault(material);
                SaveSettings(GetMaterialKey(material), DissolveMaterialSettings.Capture(material));
            }
        }

        static void RestoreOnDissolveAssignment(Material material, DissolveMaterialSettings beforeSwitch)
        {
            string key = GetMaterialKey(material);
            if (TryLoadSettings(key, out DissolveMaterialSettings saved))
                saved.Apply(material);
            else
                beforeSwitch.Apply(material);

            ApplyNoiseDefault(material);
        }

        static void UpdateTracking(Material material)
        {
            int id = material.GetInstanceID();
            LastSeenShader[id] = material.shader != null ? material.shader.name : string.Empty;
            LastSeenSettings[id] = DissolveMaterialSettings.Capture(material);
        }

        static void ApplyNoiseDefault(Material material)
        {
            if (!IsDissolveShader(material.shader))
                return;

            if (!material.HasProperty(NoiseTexProperty))
                return;

            if (material.GetTexture(NoiseTexProperty) != null || DefaultNoise == null)
                return;

            material.SetTexture(NoiseTexProperty, DefaultNoise);
            EditorUtility.SetDirty(material);
        }

        static void SaveSettings(string key, DissolveMaterialSettings settings)
        {
            if (string.IsNullOrEmpty(key))
                return;

            EditorPrefs.SetString(SettingsPrefsPrefix + key, JsonUtility.ToJson(settings));
        }

        static bool TryLoadSettings(string key, out DissolveMaterialSettings settings)
        {
            settings = default;
            if (string.IsNullOrEmpty(key))
                return false;

            string json = EditorPrefs.GetString(SettingsPrefsPrefix + key, string.Empty);
            if (string.IsNullOrEmpty(json))
                return false;

            settings = JsonUtility.FromJson<DissolveMaterialSettings>(json);
            return true;
        }

        static void OnChangesPublished(ref ObjectChangeEventStream stream)
        {
            for (int i = 0; i < stream.length; i++)
            {
                if (stream.GetEventType(i) != ObjectChangeKind.ChangeAssetObjectProperties)
                    continue;

                stream.GetChangeAssetObjectPropertiesEvent(i, out var changeEvent);
                if (EditorUtility.InstanceIDToObject(changeEvent.instanceId) is Material material)
                    ApplyDefaults(material);
            }
        }
    }

    public class DissolveMaterialPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (var path in importedAssets)
            {
                if (!path.EndsWith(".mat"))
                    continue;

                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material != null)
                    DissolveMaterialDefaults.ApplyDefaults(material);
            }
        }
    }
}
#endif

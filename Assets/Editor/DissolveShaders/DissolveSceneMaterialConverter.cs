#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Reallusion.Import
{
    public static class DissolveSceneMaterialConverter
    {
        static readonly (string BaseShaderName, string DissolveShaderName)[] ShaderPairs =
        {
            ("Reallusion/Amplify/RL_SkinShader_Variants_URP", "Reallusion/Amplify/RL_SkinShader_Variants_URP_Dissolve"),
            ("Reallusion/Amplify/RL_CorneaShaderParallax_URP", "Reallusion/Amplify/RL_CorneaShaderParallax_URP_Dissolve"),
            ("Reallusion/Amplify/RL_TeethShader_URP", "Reallusion/Amplify/RL_TeethShader_URP_Dissolve"),
            ("Reallusion/Amplify/RL_TongueShader_URP", "Reallusion/Amplify/RL_TongueShader_URP_Dissolve"),
            ("Reallusion/Amplify/RL_HairShader_1st_Pass_Variants_URP", "Reallusion/Amplify/RL_HairShader_1st_Pass_Variants_URP_Dissolve"),
            ("Reallusion/Amplify/RL_HairShader_2nd_Pass_Variants_URP", "Reallusion/Amplify/RL_HairShader_2nd_Pass_Variants_URP_Dissolve"),
            ("Shader Graphs/RL_EyeShaderParallax_Dummy_URP", "Shader Graphs/RL_EyeShaderParallax_Dummy_URP wDissolve"),
            ("Shader Graphs/RL_EyeOcclusionShader_URP", "Shader Graphs/RL_EyeOcclusionShader_URP wDissolve"),
            ("Shader Graphs/RL_TearlineShader_URP", "Shader Graphs/RL_TearlineShader_URP wDissolve"),
            ("Universal Render Pipeline/Lit", "Universal Render Pipeline/Lit_Dissolve"),
        };

        static readonly Dictionary<string, Shader> DissolveShaderCache = new Dictionary<string, Shader>();

        [MenuItem("GameObject/Dissolve/Convert Materials To Dissolve", false, 0)]
        static void ConvertSelectedToDissolve()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
                return;

            var convertedMaterials = new List<string>();
            var alreadyDissolveMaterials = new List<string>();
            var skippedMaterials = new List<string>();
            var seenMaterials = new HashSet<int>();

            foreach (GameObject root in selected)
            {
                if (!IsSceneGameObject(root))
                    continue;

                foreach (Renderer renderer in root.GetComponentsInChildren<Renderer>(true))
                {
                    ConvertRendererMaterials(renderer, convertedMaterials, alreadyDissolveMaterials, skippedMaterials, seenMaterials);
                }
            }

            LogResults(convertedMaterials, alreadyDissolveMaterials, skippedMaterials);
        }

        [MenuItem("GameObject/Dissolve/Convert Materials To Dissolve", true)]
        static bool ValidateConvertSelectedToDissolve()
        {
            GameObject[] selected = Selection.gameObjects;
            if (selected == null || selected.Length == 0)
                return false;

            for (int i = 0; i < selected.Length; i++)
            {
                if (IsSceneGameObject(selected[i]))
                    return true;
            }

            return false;
        }

        static void ConvertRendererMaterials(
            Renderer renderer,
            List<string> convertedMaterials,
            List<string> alreadyDissolveMaterials,
            List<string> skippedMaterials,
            HashSet<int> seenMaterials)
        {
            Material[] materials = renderer.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
            {
                Material material = materials[i];
                if (material == null || !seenMaterials.Add(material.GetInstanceID()))
                    continue;

                if (DissolveMaterialDefaults.IsDissolveShader(material.shader))
                {
                    alreadyDissolveMaterials.Add(FormatMaterial(material));
                    continue;
                }

                if (!TryGetDissolveShader(material.shader, out Shader dissolveShader))
                {
                    skippedMaterials.Add(FormatMaterial(material));
                    continue;
                }

                Undo.RecordObject(material, "Convert To Dissolve Materials");

                Shader oldShader = material.shader;
                DissolveMaterialDefaults.HandleShaderAssignment(material, oldShader, dissolveShader, () =>
                {
                    material.shader = dissolveShader;
                });
                DissolveMaterialDefaults.ApplyDefaults(material);
                EditorUtility.SetDirty(material);

                convertedMaterials.Add(FormatMaterial(material));
            }
        }

        static void LogResults(
            List<string> convertedMaterials,
            List<string> alreadyDissolveMaterials,
            List<string> skippedMaterials)
        {
            var report = new StringBuilder();
            report.AppendLine(
                $"Dissolve conversion complete. Converted: {convertedMaterials.Count}, already dissolve: {alreadyDissolveMaterials.Count}, skipped (no mapping): {skippedMaterials.Count}.");

            AppendMaterialList(report, "Converted", convertedMaterials);
            AppendMaterialList(report, "Already dissolve", alreadyDissolveMaterials);
            AppendMaterialList(report, "Skipped (no mapping)", skippedMaterials);

            Debug.Log(report.ToString());
        }

        static void AppendMaterialList(StringBuilder report, string heading, List<string> materials)
        {
            report.AppendLine();
            report.AppendLine($"{heading}:");

            if (materials.Count == 0)
            {
                report.AppendLine("  (none)");
                return;
            }

            for (int i = 0; i < materials.Count; i++)
                report.AppendLine($"  - {materials[i]}");
        }

        static string FormatMaterial(Material material)
        {
            string shaderName = material.shader != null ? material.shader.name : "(no shader)";
            return $"{material.name} [{shaderName}]";
        }

        static bool TryGetDissolveShader(Shader baseShader, out Shader dissolveShader)
        {
            dissolveShader = null;
            if (baseShader == null)
                return false;

            for (int i = 0; i < ShaderPairs.Length; i++)
            {
                if (ShaderPairs[i].BaseShaderName != baseShader.name)
                    continue;

                if (!DissolveShaderCache.TryGetValue(ShaderPairs[i].DissolveShaderName, out dissolveShader))
                {
                    dissolveShader = Shader.Find(ShaderPairs[i].DissolveShaderName);
                    DissolveShaderCache[ShaderPairs[i].DissolveShaderName] = dissolveShader;
                }

                return dissolveShader != null;
            }

            return false;
        }

        static bool IsSceneGameObject(GameObject gameObject)
        {
            return gameObject != null && !EditorUtility.IsPersistent(gameObject) && gameObject.scene.IsValid();
        }
    }
}
#endif

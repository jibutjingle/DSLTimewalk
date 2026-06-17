#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

namespace Reallusion.Import
{
    static class DissolveInspectorDrawing
    {
        public const uint DissolveFoldoutFlag = 1u << 4;

        public static readonly GUIContent DissolveHeader = EditorGUIUtility.TrTextContent(
            "Dissolve",
            "Controls the noise-based dissolve cutout and edge glow.");

        static readonly string[] AmplifyPropertyNames =
        {
            "_NoiseTex",
            "_DissolveAmount",
            "_DissolveColor",
            "_DissolveWidth",
        };

        static readonly string[] ShaderGraphPropertyNames =
        {
            "_DissolveNoise",
            "_DissolveAmount",
            "_DissolveEdgeColor",
            "_DissolveEdgeWidth",
        };

        const string QueueControlProperty = "_QueueControl";
        const string WorkflowModeProperty = "_WorkflowMode";
        const string UniversalEditorAssembly = "Unity.RenderPipelines.Universal.Editor";

        static readonly object ModifiedMaterialUpdateType = GetMaterialUpdateType("ModifiedMaterial");

        public static bool IsAmplifyDissolveProperty(string propertyName)
        {
            return IsNamedDissolveProperty(propertyName, AmplifyPropertyNames);
        }

        public static bool IsShaderGraphDissolveProperty(string propertyName)
        {
            return IsNamedDissolveProperty(propertyName, ShaderGraphPropertyNames);
        }

        static bool IsNamedDissolveProperty(string propertyName, string[] dissolveNames)
        {
            if (string.IsNullOrEmpty(propertyName))
                return false;

            for (int i = 0; i < dissolveNames.Length; i++)
            {
                if (dissolveNames[i] == propertyName)
                    return true;
            }

            return false;
        }

        public static void FindAmplifyDissolveProperties(
            MaterialProperty[] properties,
            out MaterialProperty noiseTex,
            out MaterialProperty dissolveAmount,
            out MaterialProperty dissolveColor,
            out MaterialProperty dissolveWidth)
        {
            noiseTex = FindProperty("_NoiseTex", properties, false);
            dissolveAmount = FindProperty("_DissolveAmount", properties, false);
            dissolveColor = FindProperty("_DissolveColor", properties, false);
            dissolveWidth = FindProperty("_DissolveWidth", properties, false);
        }

        public static void FindShaderGraphDissolveProperties(
            MaterialProperty[] properties,
            out MaterialProperty noiseTex,
            out MaterialProperty dissolveAmount,
            out MaterialProperty dissolveColor,
            out MaterialProperty dissolveWidth)
        {
            noiseTex = FindProperty("_DissolveNoise", properties, false);
            dissolveAmount = FindProperty("_DissolveAmount", properties, false);
            dissolveColor = FindProperty("_DissolveEdgeColor", properties, false);
            dissolveWidth = FindProperty("_DissolveEdgeWidth", properties, false);
        }

        public static void DrawDissolveProperties(
            MaterialEditor editor,
            MaterialProperty noiseTex,
            MaterialProperty dissolveAmount,
            MaterialProperty dissolveColor,
            MaterialProperty dissolveWidth)
        {
            if (noiseTex == null && dissolveAmount == null && dissolveColor == null && dissolveWidth == null)
                return;

            if (noiseTex != null)
                editor.ShaderProperty(noiseTex, noiseTex.displayName);
            if (dissolveAmount != null)
                editor.ShaderProperty(dissolveAmount, dissolveAmount.displayName);
            if (dissolveColor != null)
                editor.ShaderProperty(dissolveColor, dissolveColor.displayName);
            if (dissolveWidth != null)
                editor.ShaderProperty(dissolveWidth, dissolveWidth.displayName);
        }

        public static void DrawVisibleShaderProperties(
            MaterialEditor editor,
            MaterialProperty[] properties,
            Func<string, bool> isDissolveProperty)
        {
            if (properties == null)
                return;

            foreach (var property in properties)
            {
                if ((property.flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) != 0)
                    continue;

                if (isDissolveProperty(property.name))
                    continue;

                editor.ShaderProperty(property, property.displayName);
            }
        }

        public static void DrawShaderGraphSurfaceInputs(MaterialEditor editor, MaterialProperty[] properties)
        {
            var filtered = new List<MaterialProperty>(properties.Length);
            for (int i = 0; i < properties.Length; i++)
            {
                if (!IsShaderGraphDissolveProperty(properties[i].name))
                    filtered.Add(properties[i]);
            }

            var drawerType = Type.GetType("UnityEditor.ShaderGraph.Drawing.ShaderGraphPropertyDrawers, Unity.ShaderGraph.Editor");
            if (drawerType == null)
                throw new InvalidOperationException("Could not resolve ShaderGraphPropertyDrawers.");

            var drawMethod = drawerType.GetMethod(
                "DrawShaderGraphGUI",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(MaterialEditor), typeof(IEnumerable<MaterialProperty>) },
                null);

            if (drawMethod == null)
                throw new InvalidOperationException("Could not resolve DrawShaderGraphGUI.");

            drawMethod.Invoke(null, new object[] { editor, filtered });
        }

        public static void RegisterDissolveFoldout(MaterialHeaderScopeList materialScopesList, Action drawDissolveProperties)
        {
            materialScopesList.RegisterHeaderScope(DissolveHeader, DissolveFoldoutFlag, _ => drawDissolveProperties());
        }

        public static void DrawAmplifyShaderProperties(
            MaterialEditor editor,
            MaterialProperty[] properties,
            Func<string, bool> isDissolveProperty)
        {
            if (editor == null || properties == null)
                return;

            var material = editor.target as Material;
            if (material == null)
                return;

            editor.SetDefaultGUIWidths();

            var infoField = typeof(MaterialEditor).GetField(
                "m_InfoMessage",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (infoField != null)
            {
                string info = infoField.GetValue(editor) as string;
                if (!string.IsNullOrEmpty(info))
                    EditorGUILayout.HelpBox(info, MessageType.Info);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if ((property.flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) != 0)
                    continue;

                if (isDissolveProperty(property.name))
                    continue;

                if ((property.flags & MaterialProperty.PropFlags.NoScaleOffset) == MaterialProperty.PropFlags.NoScaleOffset)
                {
                    object handler = AmplifyMaterialPropertyHandler.GetHandler(material.shader, property.name);
                    if (handler != null)
                    {
                        float height = AmplifyMaterialPropertyHandler.GetPropertyHeight(
                            handler,
                            property,
                            property.displayName,
                            editor);
                        Rect rect = EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
                        AmplifyMaterialPropertyHandler.OnGUI(
                            handler,
                            ref rect,
                            property,
                            new GUIContent(property.displayName),
                            editor);

                        if (AmplifyMaterialPropertyHandler.HasPropertyDrawer(handler))
                            continue;

                        rect = EditorGUILayout.GetControlRect(true, height, EditorStyles.layerMaskField);
                        editor.TexturePropertyMiniThumbnail(rect, property, property.displayName, string.Empty);
                    }
                    else
                    {
                        editor.TexturePropertySingleLine(new GUIContent(property.displayName), property);
                    }
                }
                else
                {
                    float propertyHeight = editor.GetPropertyHeight(property, property.displayName);
                    Rect controlRect = EditorGUILayout.GetControlRect(
                        true,
                        propertyHeight,
                        EditorStyles.layerMaskField);
                    editor.ShaderProperty(controlRect, property, property.displayName);
                }
            }
        }

        public static void DrawAmplifyAdvancedOptions(MaterialEditor editor, Material material)
        {
            if (editor == null || material == null)
                return;

            EditorGUILayout.Space();
            editor.RenderQueueField();
            editor.EnableInstancingField();
            editor.DoubleSidedGIField();
            editor.LightmapEmissionProperty();

            string isEmissive = material.GetTag("IsEmissive", false, "false");
            if (isEmissive.Equals("true", StringComparison.Ordinal))
                material.globalIlluminationFlags &= (MaterialGlobalIlluminationFlags)3;
            else
                material.globalIlluminationFlags |= MaterialGlobalIlluminationFlags.EmissiveIsBlack;
        }

        public static bool TryDrawAmplifyShaderEditorToolbar(MaterialEditor editor)
        {
            if (editor == null || !editor.isVisible)
                return false;

            var openWindowType = FindAmplifyType("AmplifyShaderEditor.AmplifyShaderEditorWindow");
            var ioUtilsType = FindAmplifyType("AmplifyShaderEditor.IOUtils");
            var uiUtilsType = FindAmplifyType("AmplifyShaderEditor.UIUtils");
            if (openWindowType == null || ioUtilsType == null)
                return false;

            var material = editor.target as Material;
            if (material == null)
                return false;

            ioUtilsType.GetMethod("Init", BindingFlags.Public | BindingFlags.Static)?.Invoke(null, null);

            GUILayout.BeginVertical();
            GUILayout.Space(3);
            if (GUILayout.Button("Open in Shader Editor"))
            {
                openWindowType.GetMethod(
                        "LoadMaterialToASE",
                        BindingFlags.Public | BindingFlags.Static)
                    ?.Invoke(null, new object[] { material });
            }

            GUILayout.EndVertical();
            GUILayout.Space(5);
            return true;
        }

        public static void NotifyAmplifyMaterialChanged(Material material)
        {
            if (material == null)
                return;

            var uiUtilsType = FindAmplifyType("AmplifyShaderEditor.UIUtils");
            uiUtilsType?.GetMethod(
                    "CopyValuesFromMaterial",
                    BindingFlags.Public | BindingFlags.Static)
                ?.Invoke(null, new object[] { material });
        }

        static Type FindAmplifyType(string typeName)
        {
            Type type = Type.GetType($"{typeName}, Assembly-CSharp-Editor");
            if (type != null)
                return type;

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName, false);
                if (type != null)
                    return type;
            }

            return null;
        }

        static class AmplifyMaterialPropertyHandler
        {
            static Type s_HandlerType;

            static Type HandlerType =>
                s_HandlerType ??= Type.GetType("UnityEditor.MaterialPropertyHandler, UnityEditor");

            public static object GetHandler(Shader shader, string name)
            {
                if (HandlerType == null)
                    return null;

                return HandlerType.InvokeMember(
                    "GetHandler",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                    null,
                    null,
                    new object[] { shader, name });
            }

            public static void OnGUI(
                object handler,
                ref Rect position,
                MaterialProperty prop,
                GUIContent label,
                MaterialEditor editor)
            {
                HandlerType.InvokeMember(
                    "OnGUI",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                    null,
                    handler,
                    new object[] { position, prop, label, editor });
            }

            public static float GetPropertyHeight(
                object handler,
                MaterialProperty prop,
                string label,
                MaterialEditor editor)
            {
                return (float)HandlerType.InvokeMember(
                    "GetPropertyHeight",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod,
                    null,
                    handler,
                    new object[] { prop, label, editor });
            }

            public static bool HasPropertyDrawer(object handler)
            {
                return HandlerType.InvokeMember(
                           "propertyDrawer",
                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty,
                           null,
                           handler,
                           null) != null;
            }
        }

        public static MaterialProperty FindWorkflowModeProperty(MaterialProperty[] properties)
        {
            return FindProperty(WorkflowModeProperty, properties, false);
        }

        public static bool UsesUserRenderQueue(Material material)
        {
            return material.HasProperty(QueueControlProperty) &&
                   material.GetFloat(QueueControlProperty) >= 0.5f;
        }

        public static void ValidateShaderGraphLitMaterial(Material material)
        {
            InvokeShaderGraphUpdateMaterial("UnityEditor.ShaderGraphLitGUI", material);
        }

        public static void ValidateShaderGraphUnlitMaterial(Material material)
        {
            InvokeShaderGraphUpdateMaterial("UnityEditor.ShaderGraphUnlitGUI", material);
        }

        static void InvokeShaderGraphUpdateMaterial(string typeName, Material material)
        {
            var guiType = Type.GetType($"{typeName}, {UniversalEditorAssembly}");
            if (guiType == null)
                throw new InvalidOperationException($"Could not resolve {typeName}.");

            var updateMethod = guiType.GetMethod(
                "UpdateMaterial",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(Material), ModifiedMaterialUpdateType.GetType() },
                null);

            if (updateMethod == null)
                throw new InvalidOperationException($"Could not resolve {typeName}.UpdateMaterial.");

            updateMethod.Invoke(null, new object[] { material, ModifiedMaterialUpdateType });
        }

        static object GetMaterialUpdateType(string valueName)
        {
            var updateType = Type.GetType(
                $"Unity.Rendering.Universal.ShaderUtils+MaterialUpdateType, {UniversalEditorAssembly}");

            if (updateType == null)
                throw new InvalidOperationException("Could not resolve ShaderUtils.MaterialUpdateType.");

            return Enum.Parse(updateType, valueName);
        }

        static MaterialProperty FindProperty(string name, MaterialProperty[] properties, bool mandatory)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] != null && properties[i].name == name)
                    return properties[i];
            }

            if (mandatory)
                throw new ArgumentException($"Could not find MaterialProperty: '{name}'");

            return null;
        }
    }
}
#endif

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Reallusion.Import
{
    public class DissolveLitShaderGUI : BaseShaderGUI
    {
        enum DissolveExpandable : uint
        {
            Dissolve = 1 << 4,
        }

        static readonly string[] WorkflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));

        LitGUI.LitProperties _litProperties;
        DetailProperties _detailProperties;
        MaterialProperty _noiseTex;
        MaterialProperty _dissolveAmount;
        MaterialProperty _dissolveColor;
        MaterialProperty _dissolveWidth;

        struct DetailProperties
        {
            public MaterialProperty detailMask;
            public MaterialProperty detailAlbedoMapScale;
            public MaterialProperty detailAlbedoMap;
            public MaterialProperty detailNormalMapScale;
            public MaterialProperty detailNormalMap;

            public DetailProperties(MaterialProperty[] properties)
            {
                detailMask = FindProperty("_DetailMask", properties, false);
                detailAlbedoMapScale = FindProperty("_DetailAlbedoMapScale", properties, false);
                detailAlbedoMap = FindProperty("_DetailAlbedoMap", properties, false);
                detailNormalMapScale = FindProperty("_DetailNormalMapScale", properties, false);
                detailNormalMap = FindProperty("_DetailNormalMap", properties, false);
            }
        }

        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            _litProperties = new LitGUI.LitProperties(properties);
            _detailProperties = new DetailProperties(properties);
            _noiseTex = FindProperty("_NoiseTex", properties, false);
            _dissolveAmount = FindProperty("_DissolveAmount", properties, false);
            _dissolveColor = FindProperty("_DissolveColor", properties, false);
            _dissolveWidth = FindProperty("_DissolveWidth", properties, false);
        }

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            materialScopesList.RegisterHeaderScope(
                EditorGUIUtility.TrTextContent("Dissolve",
                    "Controls the noise-based dissolve cutout and edge glow."),
                DissolveExpandable.Dissolve,
                _ => DrawDissolveProperties());

            materialScopesList.RegisterHeaderScope(
                EditorGUIUtility.TrTextContent("Detail Inputs",
                    "These settings define the surface details by tiling and overlaying additional maps on the surface."),
                Expandable.Details,
                _ => DrawDetailArea());
        }

        void DrawDetailArea()
        {
            materialEditor.TexturePropertySingleLine(
                EditorGUIUtility.TrTextContent("Mask",
                    "Select a mask for the Detail map. The mask uses the alpha channel of the selected texture. The Tiling and Offset settings have no effect on the mask."),
                _detailProperties.detailMask);
            materialEditor.TexturePropertySingleLine(
                EditorGUIUtility.TrTextContent("Base Map",
                    "Select the surface detail texture.The alpha of your texture determines surface hue and intensity."),
                _detailProperties.detailAlbedoMap,
                _detailProperties.detailAlbedoMap.textureValue != null ? _detailProperties.detailAlbedoMapScale : null);

            if (_detailProperties.detailAlbedoMapScale != null && _detailProperties.detailAlbedoMapScale.floatValue != 1.0f)
            {
                EditorGUILayout.HelpBox(
                    EditorGUIUtility.TrTextContent("Setting the scaling factor to a value other than 1 results in a less performant shader variant.").text,
                    MessageType.Info,
                    true);
            }

            var detailAlbedoTexture = _detailProperties.detailAlbedoMap.textureValue as Texture2D;
            if (detailAlbedoTexture != null && GraphicsFormatUtility.IsSRGBFormat(detailAlbedoTexture.graphicsFormat))
            {
                EditorGUILayout.HelpBox(
                    EditorGUIUtility.TrTextContent("This texture is not in linear space.").text,
                    MessageType.Warning,
                    true);
            }

            materialEditor.TexturePropertySingleLine(
                EditorGUIUtility.TrTextContent("Normal Map",
                    "Designates a Normal Map to create the illusion of bumps and dents in the details of this Material's surface."),
                _detailProperties.detailNormalMap,
                _detailProperties.detailNormalMap.textureValue != null ? _detailProperties.detailNormalMapScale : null);
            materialEditor.TextureScaleOffsetProperty(_detailProperties.detailAlbedoMap);
        }

        static void SetDetailKeywords(Material material)
        {
            if (!material.HasProperty("_DetailAlbedoMap") || !material.HasProperty("_DetailNormalMap") ||
                !material.HasProperty("_DetailAlbedoMapScale"))
                return;

            bool isScaled = material.GetFloat("_DetailAlbedoMapScale") != 1.0f;
            bool hasDetailMap = material.GetTexture("_DetailAlbedoMap") || material.GetTexture("_DetailNormalMap");
            CoreUtils.SetKeyword(material, "_DETAIL_MULX2", !isScaled && hasDetailMap);
            CoreUtils.SetKeyword(material, "_DETAIL_SCALED", isScaled && hasDetailMap);
        }

        public override void ValidateMaterial(Material material)
        {
            SetMaterialKeywords(material, LitGUI.SetMaterialKeywords, SetDetailKeywords);
        }

        public override void DrawSurfaceOptions(Material material)
        {
            EditorGUIUtility.labelWidth = 0f;

            if (_litProperties.workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, _litProperties.workflowMode, WorkflowModeNames);

            base.DrawSurfaceOptions(material);
        }

        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            LitGUI.Inputs(_litProperties, materialEditor, material);
            DrawEmissionProperties(material, true);
            DrawTileOffset(materialEditor, baseMapProp);
        }

        void DrawDissolveProperties()
        {
            if (_noiseTex == null && _dissolveAmount == null && _dissolveColor == null && _dissolveWidth == null)
                return;

            if (_noiseTex != null)
                materialEditor.ShaderProperty(_noiseTex, _noiseTex.displayName);
            if (_dissolveAmount != null)
                materialEditor.ShaderProperty(_dissolveAmount, _dissolveAmount.displayName);
            if (_dissolveColor != null)
                materialEditor.ShaderProperty(_dissolveColor, _dissolveColor.displayName);
            if (_dissolveWidth != null)
                materialEditor.ShaderProperty(_dissolveWidth, _dissolveWidth.displayName);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            if (_litProperties.reflections != null && _litProperties.highlights != null)
            {
                materialEditor.ShaderProperty(_litProperties.highlights, LitGUI.Styles.highlightsText);
                materialEditor.ShaderProperty(_litProperties.reflections, LitGUI.Styles.reflectionsText);
            }

            base.DrawAdvancedOptions(material);
        }

        public override void OnGUI(MaterialEditor materialEditorIn, MaterialProperty[] properties)
        {
            foreach (Material material in materialEditorIn.targets)
                DissolveMaterialDefaults.ApplyDefaults(material);

            base.OnGUI(materialEditorIn, properties);
        }

        public override void AssignNewShaderToMaterial(Material material, Shader oldShader, Shader newShader)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            if (material.HasProperty("_Emission"))
                material.SetColor("_EmissionColor", material.GetColor("_Emission"));

            DissolveMaterialDefaults.HandleShaderAssignment(material, oldShader, newShader, () =>
            {
                base.AssignNewShaderToMaterial(material, oldShader, newShader);
            });

            if (oldShader == null || !oldShader.name.Contains("Legacy Shaders/"))
            {
                SetupMaterialBlendMode(material);
                DissolveMaterialDefaults.ApplyDefaults(material);
                return;
            }

            SurfaceType surfaceType = SurfaceType.Opaque;
            BlendMode blendMode = BlendMode.Alpha;
            if (oldShader.name.Contains("/Transparent/Cutout/"))
            {
                surfaceType = SurfaceType.Opaque;
                material.SetFloat("_AlphaClip", 1);
            }
            else if (oldShader.name.Contains("/Transparent/"))
            {
                surfaceType = SurfaceType.Transparent;
                blendMode = BlendMode.Alpha;
            }

            material.SetFloat("_Blend", (float)blendMode);
            material.SetFloat("_Surface", (float)surfaceType);
            if (surfaceType == SurfaceType.Opaque)
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            else
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");

            if (oldShader.name.Equals("Standard (Specular setup)"))
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Specular);
                Texture texture = material.GetTexture("_SpecGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }
            else
            {
                material.SetFloat("_WorkflowMode", (float)LitGUI.WorkflowMode.Metallic);
                Texture texture = material.GetTexture("_MetallicGlossMap");
                if (texture != null)
                    material.SetTexture("_MetallicSpecGlossMap", texture);
            }

            DissolveMaterialDefaults.ApplyDefaults(material);
        }
    }
}
#endif

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;
using UnityEngine.Rendering;

namespace Reallusion.Import
{
    public class DissolveAmplifyCharacterShaderGUI : BaseShaderGUI
    {
        static readonly string[] WorkflowModeNames = Enum.GetNames(typeof(LitGUI.WorkflowMode));

        MaterialProperty[] _properties;
        MaterialProperty _workflowMode;
        MaterialProperty _noiseTex;
        MaterialProperty _dissolveAmount;
        MaterialProperty _dissolveColor;
        MaterialProperty _dissolveWidth;

        public override void FindProperties(MaterialProperty[] properties)
        {
            _properties = properties;
            base.FindProperties(properties);
            _workflowMode = FindProperty("_WorkflowMode", properties, false);
            DissolveInspectorDrawing.FindAmplifyDissolveProperties(
                properties,
                out _noiseTex,
                out _dissolveAmount,
                out _dissolveColor,
                out _dissolveWidth);
        }

        public override void ValidateMaterial(Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            SetMaterialKeywords(material, SetAmplifyShaderKeywords);
        }

        static void SetAmplifyShaderKeywords(Material material)
        {
            if (material.HasProperty("_WorkflowMode"))
            {
                bool isSpecularWorkflow = material.GetFloat("_WorkflowMode") == (float)LitGUI.WorkflowMode.Specular;
                CoreUtils.SetKeyword(material, "_SPECULAR_SETUP", isSpecularWorkflow);
            }

            if (material.HasProperty("_SpecularHighlights"))
            {
                CoreUtils.SetKeyword(material, "_SPECULARHIGHLIGHTS_OFF",
                    material.GetFloat("_SpecularHighlights") == 0.0f);
            }

            if (material.HasProperty("_EnvironmentReflections"))
            {
                CoreUtils.SetKeyword(material, "_ENVIRONMENTREFLECTIONS_OFF",
                    material.GetFloat("_EnvironmentReflections") == 0.0f);
            }
        }

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            DissolveInspectorDrawing.RegisterDissolveFoldout(materialScopesList, DrawDissolveProperties);
        }

        public override void DrawSurfaceOptions(Material material)
        {
            if (material == null)
                throw new ArgumentNullException(nameof(material));

            EditorGUIUtility.labelWidth = 0f;

            if (_workflowMode != null)
                DoPopup(LitGUI.Styles.workflowModeText, _workflowMode, WorkflowModeNames);

            base.DrawSurfaceOptions(material);
        }

        public override void DrawSurfaceInputs(Material material)
        {
            DissolveInspectorDrawing.DrawVisibleShaderProperties(
                materialEditor,
                _properties,
                DissolveInspectorDrawing.IsAmplifyDissolveProperty);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            DoPopup(Styles.queueControl, queueControlProp, Styles.queueControlNames);
            if (material.HasProperty("_QueueControl") &&
                material.GetFloat("_QueueControl") == (float)QueueControl.UserOverride)
            {
                materialEditor.RenderQueueField();
            }

            base.DrawAdvancedOptions(material);
            materialEditor.DoubleSidedGIField();
            materialEditor.LightmapEmissionFlagsProperty(0, true, true);
        }

        void DrawDissolveProperties()
        {
            DissolveInspectorDrawing.DrawDissolveProperties(
                materialEditor,
                _noiseTex,
                _dissolveAmount,
                _dissolveColor,
                _dissolveWidth);
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

            DissolveMaterialDefaults.HandleShaderAssignment(material, oldShader, newShader, () =>
            {
                base.AssignNewShaderToMaterial(material, oldShader, newShader);
            });

            DissolveMaterialDefaults.ApplyDefaults(material);
        }
    }
}
#endif

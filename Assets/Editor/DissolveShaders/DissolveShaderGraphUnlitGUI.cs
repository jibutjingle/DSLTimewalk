#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEditor.Rendering.Universal.ShaderGUI;
using UnityEngine;

namespace Reallusion.Import
{
    public class DissolveShaderGraphUnlitGUI : BaseShaderGUI
    {
        MaterialProperty[] _properties;
        MaterialProperty _noiseTex;
        MaterialProperty _dissolveAmount;
        MaterialProperty _dissolveColor;
        MaterialProperty _dissolveWidth;

        public override void FindProperties(MaterialProperty[] properties)
        {
            _properties = properties;
            base.FindProperties(properties);
            DissolveInspectorDrawing.FindShaderGraphDissolveProperties(
                properties,
                out _noiseTex,
                out _dissolveAmount,
                out _dissolveColor,
                out _dissolveWidth);
        }

        public override void ValidateMaterial(Material material)
        {
            DissolveInspectorDrawing.ValidateShaderGraphUnlitMaterial(material);
        }

        public override void FillAdditionalFoldouts(MaterialHeaderScopeList materialScopesList)
        {
            DissolveInspectorDrawing.RegisterDissolveFoldout(materialScopesList, DrawDissolveProperties);
        }

        public override void DrawSurfaceInputs(Material material)
        {
            DissolveInspectorDrawing.DrawShaderGraphSurfaceInputs(materialEditor, _properties);
        }

        public override void DrawAdvancedOptions(Material material)
        {
            DoPopup(Styles.queueControl, queueControlProp, Styles.queueControlNames);
            if (DissolveInspectorDrawing.UsesUserRenderQueue(material))
                materialEditor.RenderQueueField();

            base.DrawAdvancedOptions(material);
            materialEditor.DoubleSidedGIField();
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

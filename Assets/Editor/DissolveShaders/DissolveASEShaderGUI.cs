#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Reallusion.Import
{
    public class DissolveASEShaderGUI : ShaderGUI
    {
        MaterialEditor _materialEditor;
        MaterialProperty _noiseTex;
        MaterialProperty _dissolveAmount;
        MaterialProperty _dissolveColor;
        MaterialProperty _dissolveWidth;

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            _materialEditor = materialEditor;

            foreach (Material material in materialEditor.targets)
                DissolveMaterialDefaults.ApplyDefaults(material);

            DissolveInspectorDrawing.FindAmplifyDissolveProperties(
                properties,
                out _noiseTex,
                out _dissolveAmount,
                out _dissolveColor,
                out _dissolveWidth);

            DissolveInspectorDrawing.TryDrawAmplifyShaderEditorToolbar(materialEditor);

            EditorGUI.BeginChangeCheck();
            DissolveInspectorDrawing.DrawAmplifyShaderProperties(
                materialEditor,
                properties,
                DissolveInspectorDrawing.IsAmplifyDissolveProperty);

            var targetMaterial = materialEditor.target as Material;
            if (targetMaterial != null)
            {
                DissolveInspectorDrawing.DrawAmplifyAdvancedOptions(materialEditor, targetMaterial);

                var scopes = new MaterialHeaderScopeList(uint.MaxValue);
                DissolveInspectorDrawing.RegisterDissolveFoldout(scopes, DrawDissolveProperties);
                scopes.DrawHeaders(materialEditor, targetMaterial);
            }

            if (EditorGUI.EndChangeCheck())
            {
                foreach (Material material in materialEditor.targets)
                    DissolveInspectorDrawing.NotifyAmplifyMaterialChanged(material);
            }
        }

        void DrawDissolveProperties()
        {
            DissolveInspectorDrawing.DrawDissolveProperties(
                _materialEditor,
                _noiseTex,
                _dissolveAmount,
                _dissolveColor,
                _dissolveWidth);
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

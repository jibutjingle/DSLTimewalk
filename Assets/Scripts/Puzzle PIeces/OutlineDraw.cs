using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteAlways]
public class OutlineDraw : MonoBehaviour
{
    public enum DrawMode
    {
        GizmosOnly,
        RuntimeDraw,
        Both
    }

    // cached mesh + world matrix pairs gathered from outlineRoot
    struct OutlinePart
    {
        public Mesh Mesh;
        public Matrix4x4 Matrix;
    }

    [Header("Outline source")]
    [Tooltip("Mesh is read from MeshFilters on this object and its children")]
    [SerializeField] GameObject outlineRoot;

    [Header("Drawing")]
    [SerializeField] DrawMode drawMode = DrawMode.Both;
    // required for RuntimeDraw / Both during play mode
    [SerializeField] Material runtimeMaterial;
    [SerializeField] Color gizmoColor = new Color(0f, 1f, 1f, 0.18f);
    [Range(0, 31)][SerializeField] int layer = 0;

    readonly List<OutlinePart> _outlineParts = new List<OutlinePart>();

    void OnEnable()
    {
        RebuildOutline();
    }

    void OnValidate()
    {
        RebuildOutline();
    }

    [ContextMenu("Rebuild Outline")]
    public void RebuildOutline()
    {
        _outlineParts.Clear();

        if (!outlineRoot)
            return;

        // true includes disabled children under outlineRoot
        MeshFilter[] meshFilters = outlineRoot.GetComponentsInChildren<MeshFilter>(true);
        for (int i = 0; i < meshFilters.Length; i++)
        {
            MeshFilter meshFilter = meshFilters[i];
            if (meshFilter == null || meshFilter.sharedMesh == null)
                continue;

            _outlineParts.Add(new OutlinePart
            {
                Mesh = meshFilter.sharedMesh,
                Matrix = meshFilter.transform.localToWorldMatrix
            });
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (drawMode == DrawMode.RuntimeDraw || !enabled)
            return;

        if (_outlineParts.Count == 0)
            RebuildOutline();

        Color previousColor = Gizmos.color;
        Gizmos.color = gizmoColor;

        // scene view guide — visible without entering play mode
        for (int i = 0; i < _outlineParts.Count; i++)
        {
            OutlinePart part = _outlineParts[i];
            if (part.Mesh == null)
                continue;

            Gizmos.matrix = part.Matrix;
            Gizmos.DrawMesh(part.Mesh);
            Gizmos.DrawWireMesh(part.Mesh);
        }

        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = previousColor;
    }
#endif

    void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        if (drawMode == DrawMode.GizmosOnly)
            return;

        if (!runtimeMaterial)
            return;

        if (_outlineParts.Count == 0)
            RebuildOutline();

        // inheadset / play mode outline using Graphics.DrawMesh
        for (int i = 0; i < _outlineParts.Count; i++)
        {
            OutlinePart part = _outlineParts[i];
            if (part.Mesh == null)
                continue;

            Graphics.DrawMesh(
                part.Mesh,
                part.Matrix,
                runtimeMaterial,
                layer,
                null,
                0,
                null,
                ShadowCastingMode.Off,
                receiveShadows: false);
        }
    }
}

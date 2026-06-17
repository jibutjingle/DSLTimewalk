using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class BakeFaceCenters02 : MonoBehaviour
{
    [Tooltip("If true, bakes on Start.")]
    public bool bakeOnStart = true;

    [Tooltip("If true and no tag is provided, bakes all children (including inactive).")]
    public bool useChildren = true;

    [SerializeField] private string burstTag = "";
    [SerializeField] private bool verboseLogs = false;

    private GameObject[] burstAbleObjects;

    void Start()
    {
        if (!string.IsNullOrEmpty(burstTag))
        {
            burstAbleObjects = GameObject.FindGameObjectsWithTag(burstTag);
        }
        else if (useChildren)
        {
            burstAbleObjects = GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToArray();
        }
        else
        {
            Debug.LogWarning("Burst tag not assigned and useChildren is false. No objects baked.");
            return;
        }

        if (!bakeOnStart) return;

        int baked = 0;
        foreach (var obj in burstAbleObjects)
        {
            if (BakeFaceCenter(obj)) baked++;
        }

        Debug.Log($"BakeFaceCenters02: baked {baked}/{burstAbleObjects.Length} objects.");
    }

    [ContextMenu("Bake Face Centers Now (Selection Scope)")]
    public void BakeAllNow()
    {
        if (burstAbleObjects == null || burstAbleObjects.Length == 0)
        {
            burstAbleObjects = GetComponentsInChildren<Transform>(true).Select(t => t.gameObject).ToArray();
        }

        int baked = 0;
        foreach (var obj in burstAbleObjects)
        {
            if (BakeFaceCenter(obj)) baked++;
        }

        Debug.Log($"BakeFaceCenters02: baked {baked}/{burstAbleObjects.Length} objects.");
    }

    public bool BakeFaceCenter(GameObject burstObject)
    {
        var mf = burstObject.GetComponent<MeshFilter>();
        if (!mf) return false;
        var src = mf.sharedMesh;
        if (!src) return false;
        
        try
        {
            return BakeFaceCenter_Internal(mf, src, burstObject.name);
        }
        catch (Exception ex)
        {
            Debug.LogError($"BakeFaceCenters02: '{burstObject.name}' bake failed: {ex}");
            return false;
        }
    }

    private bool BakeFaceCenter_Internal(MeshFilter mf, Mesh src, string debugName)
    {
        int subMeshCount = Mathf.Max(1, src.subMeshCount);

        // Ensure all submeshes are triangles
        for (int s = 0; s < subMeshCount; s++)
        {
            var sm = src.GetSubMesh(s);
            if (sm.topology != MeshTopology.Triangles)
            {
                Debug.LogWarning($"'{debugName}' submesh {s} is not triangles. Skipping.");
                return false;
            }
        }

        var srcVerts = src.vertices;

        int totalNewVertCount = 0;
        int totalIndexCount = 0;

        for (int s = 0; s < subMeshCount; s++)
        {
            var inds = src.GetTriangles(s);
            totalNewVertCount += inds.Length;
            totalIndexCount += inds.Length;
        }

        if (totalNewVertCount == 0) return false;

        var newToOld = new int[totalNewVertCount];
        var bakedColors = new Color[totalNewVertCount];

        int[] submeshIndexCounts = new int[subMeshCount];
        Bounds[] submeshBounds = new Bounds[subMeshCount];

        int writeVert = 0;

        for (int s = 0; s < subMeshCount; s++)
        {
            var inds = src.GetTriangles(s);
            submeshIndexCounts[s] = inds.Length;
            submeshBounds[s] = src.GetSubMesh(s).bounds;

            for (int i = 0; i < inds.Length; i += 3)
            {
                int i0 = inds[i];
                int i1 = inds[i + 1];
                int i2 = inds[i + 2];

                Vector3 v0 = srcVerts[i0];
                Vector3 v1 = srcVerts[i1];
                Vector3 v2 = srcVerts[i2];

                // ✅ TRUE FACE CENTER (object space)
                Vector3 center = (v0 + v1 + v2) / 3f;

                Color col = new Color(center.x, center.y, center.z, 1f);

                newToOld[writeVert + 0] = i0;
                newToOld[writeVert + 1] = i1;
                newToOld[writeVert + 2] = i2;

                bakedColors[writeVert + 0] = col;
                bakedColors[writeVert + 1] = col;
                bakedColors[writeVert + 2] = col;

                writeVert += 3;
            }
        }

        if (verboseLogs)
        {
            Debug.Log($"{debugName}: newVerts={totalNewVertCount}");
        }

        var descriptors = new List<VertexAttributeDescriptor>(src.GetVertexAttributes());

        var dst = new Mesh();
        dst.name = src.name + "_FaceCenters";

        using (var ro = Mesh.AcquireReadOnlyMeshData(src))
        {
            var srcMd = ro[0];
            var wd = Mesh.AllocateWritableMeshData(1);
            var dstMd = wd[0];

            dstMd.SetVertexBufferParams(totalNewVertCount, descriptors.ToArray());

            int srcStreamCount = srcMd.vertexBufferCount;
            int dstStreamCount = dstMd.vertexBufferCount;

            for (int stream = 0; stream < dstStreamCount; stream++)
            {
                var dstBytes = dstMd.GetVertexData<byte>(stream);
                int dstStride = dstMd.GetVertexBufferStride(stream);

                if (stream < srcStreamCount)
                {
                    var srcBytes = srcMd.GetVertexData<byte>(stream);
                    int srcStride = srcMd.GetVertexBufferStride(stream);

                    int stride = Mathf.Min(srcStride, dstStride);

                    for (int nv = 0; nv < totalNewVertCount; nv++)
                    {
                        int ov = newToOld[nv];

                        int srcBase = ov * srcStride;
                        int dstBase = nv * dstStride;

                        for (int b = 0; b < stride; b++)
                            dstBytes[dstBase + b] = srcBytes[srcBase + b];
                    }
                }
            }

            var indexFormat = totalNewVertCount > 65535
                ? IndexFormat.UInt32
                : IndexFormat.UInt16;

            dstMd.SetIndexBufferParams(totalIndexCount, indexFormat);

            if (indexFormat == IndexFormat.UInt16)
            {
                var idx = dstMd.GetIndexData<ushort>();
                for (int i = 0; i < totalIndexCount; i++) idx[i] = (ushort)i;
            }
            else
            {
                var idx = dstMd.GetIndexData<uint>();
                for (int i = 0; i < totalIndexCount; i++) idx[i] = (uint)i;
            }

            dstMd.subMeshCount = subMeshCount;

            int indexStart = 0;
            for (int s = 0; s < subMeshCount; s++)
            {
                var smd = new SubMeshDescriptor(indexStart, submeshIndexCounts[s]);
                smd.bounds = submeshBounds[s];

                dstMd.SetSubMesh(s, smd, MeshUpdateFlags.DontRecalculateBounds);
                indexStart += submeshIndexCounts[s];
            }

            Mesh.ApplyAndDisposeWritableMeshData(wd, dst);
        }

        // ✅ WRITE FULL FLOAT CENTER DIRECTLY
        dst.colors = bakedColors;

        dst.bounds = src.bounds;

        mf.mesh = dst;

        var mr = mf.GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.additionalVertexStreams = null;

            // Force refresh (important for imported meshes)
            mr.enabled = false;
            mr.enabled = true;
        }

        return true;
    }
}
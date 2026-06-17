// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Rendering;

// [ExecuteAlways]
// public class GuideFromZonesDraw : MonoBehaviour
// {
//     public enum DrawMode { GizmosOnly, RuntimeDraw, Both }

//     [Header("Where your zones live")]
//     public Transform board;

//     [System.Serializable]
//     public class GhostDef
//     {
//         public string pieceId;
//         public Mesh mesh;
//         public Vector3 localOffset = Vector3.zero;
//         public Vector3 localEuler = Vector3.zero;
//         public Vector3 localScale = Vector3.one;
//     }

//     [Header("Map pieceId → ghost mesh/offset")]
//     public List<GhostDef> ghosts = new List<GhostDef>();

//     [Header("Fallback if an id isn't mapped")]
//     public Mesh defaultMesh;

//     [Header("Complete Puzzle Outline")]
//     [Tooltip("If assigned, this mesh will be used as the complete puzzle outline instead of individual piece meshes")]
//     public Mesh completePuzzleMesh;
//     [Tooltip("Transform to position the complete puzzle mesh")]
//     public Transform completePuzzleTransform;

//     [Header("Drawing")]
//     public DrawMode drawMode = DrawMode.Both;
//     public Material runtimeMaterial;
//     [Range(1, 4)] public int render = 0;
//     public bool rebuildAutomatically = true;

//     [Header("Mesh Combination (optional)")]
//     [Tooltip("If enabled, all ghost instances are combined into a single mesh to reduce draw calls.")]
//     public bool combineIntoSingleMesh = false;

//     [Header("Occupancy Hiding (optional)")]
//     [Tooltip("If enabled, ghosts are hidden for zones that appear occupied (via OverlapBox). This prevents tinting placed objects.")]
//     public bool hideWhenOccupied = false;
//     [Tooltip("Physics layers considered as occupying objects.")]
//     public LayerMask occupancyMask = ~0;
//     [Tooltip("Local-space box size used to detect an occupying object at the snap pose.")]
//     public Vector3 occupancyBoxSize = new Vector3(0.08f, 0.08f, 0.08f);
//     [Tooltip("Local-space offset applied before testing occupancy.")]
//     public Vector3 occupancyBoxOffset = Vector3.zero;
//     [Tooltip("Colliders under this root are ignored by occupancy checks (e.g., the board/guide itself). Defaults to 'board'.")]
//     public Transform overlapIgnoreRoot;
//     [Tooltip("How often to re-check occupancy while playing (seconds). Lower is more responsive but slightly more CPU).")]
//     [Range(0.02f, 0.5f)] public float occupancyPollInterval = 0.1f;

//     private Dictionary<string, GhostDef> _map = new Dictionary<string, GhostDef>();
//     private readonly Dictionary<Mesh, List<Matrix4x4>> _matricesByMesh = new Dictionary<Mesh, List<Matrix4x4>>();
//     private readonly List<PuzzleSnapZone> _zones = new List<PuzzleSnapZone>();
//     private int _cachedZoneCount = -1;
//     private Mesh _combinedMesh;
//     private bool _lastHideWhenOccupied;
//     private bool _lastCombineIntoSingleMesh;
//     private readonly Dictionary<PuzzleSnapZone, bool> _zoneOccupied = new Dictionary<PuzzleSnapZone, bool>();
//     private float _nextOccupancyPollTime = 0f;

//     void OnEnable()
//     {
//         BuildLookup();
//         RebuildNow();
//         _lastHideWhenOccupied = hideWhenOccupied;
//         _lastCombineIntoSingleMesh = combineIntoSingleMesh;
//     }

//     void OnValidate()
//     {
//         if (!rebuildAutomatically) return;
//         BuildLookup();
//         RebuildNow();
//     }

//     void OnDisable()
//     {
//         DestroyCombinedMesh();
//     }

//     void Start()
//     {
//         if (!rebuildAutomatically) return;
//         RebuildNow();
//         if (Application.isPlaying)
//             StartCoroutine(DelayedInitialRebuild());
//     }

//     void Update()
//     {
//         if (Application.isPlaying && drawMode != DrawMode.GizmosOnly)
//         {
//             var currentCount = CountZones();
//             if (currentCount != _cachedZoneCount)
//                 RebuildNow();

//             if (hideWhenOccupied != _lastHideWhenOccupied || combineIntoSingleMesh != _lastCombineIntoSingleMesh)
//             {
//                 _lastHideWhenOccupied = hideWhenOccupied;
//                 _lastCombineIntoSingleMesh = combineIntoSingleMesh;
//                 RebuildNow();
//             }

//             if (hideWhenOccupied && Time.time >= _nextOccupancyPollTime)
//             {
//                 _nextOccupancyPollTime = Time.time + Mathf.Max(0.02f, occupancyPollInterval);
//                 bool anyChanged = false;
//                 if (_zones.Count == 0 && board)
//                     board.GetComponentsInChildren(true, _zones);

//                 for (int i = 0; i < _zones.Count; i++)
//                 {
//                     var z = _zones[i];
//                     if (!z || z.snapPose == null) continue;
//                     bool nowOcc = IsZoneOccupied(z);
//                     bool prevOcc = false;
//                     _zoneOccupied.TryGetValue(z, out prevOcc);
//                     if (nowOcc != prevOcc)
//                     {
//                         _zoneOccupied[z] = nowOcc;
//                         anyChanged = true;
//                     }
//                 }
//                 if (anyChanged)
//                     RebuildNow();
//             }
//         }
//     }

//     [ContextMenu("Rebuild Guide From Zones")]
//     public void RebuildNow()
//     {
//         _zones.Clear();
//         _matricesByMesh.Clear();
//         _cachedZoneCount = 0;
//         DestroyCombinedMesh();

//         if (!board)
//         {
//             Debug.LogWarning("[GuideFromZonesDraw] Assign 'board' (the parent containing PuzzleSnapZone).");
//             return;
//         }

//         board.GetComponentsInChildren(true, _zones);
//         if (_zones.Count == 0) return;
//         _cachedZoneCount = _zones.Count;

//         foreach (var z in _zones)
//         {
//             if (!z || z.snapPose == null) continue;
//             bool occupied = hideWhenOccupied && IsZoneOccupied(z);
//             _zoneOccupied[z] = hideWhenOccupied ? occupied : false;
//             if (occupied)
//                 continue;
//             var id = z.expectedPieceId;
//             Mesh mesh = null;
//             Vector3 off = Vector3.zero;
//             Vector3 eul = Vector3.zero;
//             Vector3 scl = Vector3.one;

//             if (!string.IsNullOrEmpty(id) && _map.TryGetValue(id, out var def) && def != null && def.mesh)
//             {
//                 mesh = def.mesh;
//                 off = def.localOffset;
//                 eul = def.localEuler;
//                 scl = def.localScale;
//             }
//             else
//             {
//                 mesh = defaultMesh;
//                 if (!mesh) continue;
//             }

//             var mtx = z.snapPose.localToWorldMatrix * Matrix4x4.TRS(off, Quaternion.Euler(eul), scl);

//             if (!_matricesByMesh.TryGetValue(mesh, out var list))
//             {
//                 list = new List<Matrix4x4>(16);
//                 _matricesByMesh[mesh] = list;
//             }
//             list.Add(mtx);
//         }

//         if (combineIntoSingleMesh && _matricesByMesh.Count > 0)
//         {
//             var combines = new List<CombineInstance>(256);
//             foreach (var kvp in _matricesByMesh)
//             {
//                 var mesh = kvp.Key;
//                 var mats = kvp.Value;
//                 if (!mesh || mats == null || mats.Count == 0) continue;
//                 for (int i = 0; i < mats.Count; i++)
//                 {
//                     var ci = new CombineInstance
//                     {
//                         mesh = mesh,
//                         transform = mats[i],
//                         subMeshIndex = 0
//                     };
//                     combines.Add(ci);
//                 }
//             }

//             if (combines.Count > 0)
//             {
//                 _combinedMesh = new Mesh();
//                 _combinedMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // support many verts
//                 _combinedMesh.name = "GuideFromZonesDraw_Combined";
//                 _combinedMesh.CombineMeshes(combines.ToArray(), mergeSubMeshes: true, useMatrices: true, hasLightmapData: false);
//             }
//         }
//     }

//     private System.Collections.IEnumerator DelayedInitialRebuild()
//     {
//         yield return null;
//         RebuildNow();
//     }

//     private void BuildLookup()
//     {
//         _map.Clear();
//         foreach (var g in ghosts)
//         {
//             if (g != null && !string.IsNullOrEmpty(g.pieceId) && g.mesh != null)
//                 _map[g.pieceId] = g;
//         }
//     }

//     private void DestroyCombinedMesh()
//     {
//         if (_combinedMesh)
//         {
// #if UNITY_EDITOR
//             if (!Application.isPlaying)
//                 DestroyImmediate(_combinedMesh);
//             else
// #endif
//                 Destroy(_combinedMesh);
//             _combinedMesh = null;
//         }
//     }

//     private bool IsZoneOccupied(PuzzleSnapZone zone)
//     {
//         if (!zone || zone.snapPose == null) return false;
//         var t = zone.snapPose;
//         var center = t.TransformPoint(occupancyBoxOffset);
//         var halfExtents = occupancyBoxSize * 0.5f;
//         var rotation = t.rotation;

//         var hits = Physics.OverlapBox(center, halfExtents, rotation, occupancyMask, QueryTriggerInteraction.Ignore);
//         if (hits == null || hits.Length == 0) return false;

//         var ignore = overlapIgnoreRoot ? overlapIgnoreRoot : board;
//         for (int i = 0; i < hits.Length; i++)
//         {
//             var c = hits[i];
//             if (!c) continue;
//             if (ignore && c.transform.IsChildOf(ignore))
//                 continue;

//             if (IsObjectSnappedInPlace(c.transform, zone))
//                 return true;
//         }
//         return false;
//     }

//     private bool IsObjectSnappedInPlace(Transform objTransform, PuzzleSnapZone zone)
//     {
//         if (!objTransform || !zone || !zone.snapPose) return false;

//         if (IsObjectBeingHeld(objTransform)) return false;

//         var snapPos = zone.snapPose.position;
//         var objPos = objTransform.position;
//         var distance = Vector3.Distance(snapPos, objPos);

//         var snapThreshold = occupancyBoxSize.magnitude * 0.3f;

//         if (distance > snapThreshold) return false;

//         var snapRot = zone.snapPose.rotation;
//         var objRot = objTransform.rotation;
//         var angleDiff = Quaternion.Angle(snapRot, objRot);

//         var rotationThreshold = 45f;

//         return angleDiff <= rotationThreshold;
//     }

//     private bool IsObjectBeingHeld(Transform objTransform)
//     {
//         if (!objTransform) return false;

//         var grabInteractable = objTransform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
//         if (grabInteractable != null && grabInteractable.isSelected)
//             return true;

//         var directInteractor = objTransform.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor>();
//         if (directInteractor != null && directInteractor.hasSelection)
//             return true;

//         var rb = objTransform.GetComponent<Rigidbody>();
//         if (rb != null)
//         {
//             if (rb.velocity.magnitude > 0.1f || rb.angularVelocity.magnitude > 0.1f)
//                 return true;
//         }

//         var interactables = objTransform.GetComponentsInChildren<MonoBehaviour>();
//         foreach (var comp in interactables)
//         {
//             if (comp == null) continue;

//             var compName = comp.GetType().Name.ToLower();
//             if (compName.Contains("grab") || compName.Contains("interact") || compName.Contains("select"))
//             {
//                 var isSelectedProperty = comp.GetType().GetProperty("isSelected");
//                 if (isSelectedProperty != null)
//                 {
//                     var isSelected = (bool)isSelectedProperty.GetValue(comp);
//                     if (isSelected) return true;
//                 }
//             }
//         }

//         return false;
//     }

//     private int CountZones()
//     {
//         var count = 0;
//         if (board)
//         {
//             var arr = board.GetComponentsInChildren<PuzzleSnapZone>(true);
//             count = arr != null ? arr.Length : 0;
//         }
//         return count;
//     }

//     private bool IsValidMesh(Mesh mesh)
//     {
//         if (mesh == null) return false;

//         if (mesh.vertexCount == 0) return false;

//         try
//         {
//             if (mesh.triangles == null || mesh.triangles.Length == 0) return false;

//             if (mesh.normals == null || mesh.normals.Length == 0) return false;
//         }
//         catch (System.Exception)
//         {
//         }

//         return true;
//     }


// #if UNITY_EDITOR
//     void OnDrawGizmos()
//     {
//         if (drawMode == DrawMode.RuntimeDraw) return;
//         if (!enabled) return;

//         var color = new Color(0f, 1f, 1f, 0.18f);
//         Gizmos.color = color;

//         if (completePuzzleMesh && IsValidMesh(completePuzzleMesh))
//         {
//             Gizmos.matrix = Matrix4x4.identity;
//             var puzzleTransform = completePuzzleTransform ? completePuzzleTransform : board;
//             if (puzzleTransform)
//             {
//                 Gizmos.matrix = puzzleTransform.localToWorldMatrix;
//             }

// #if UNITY_2021_2_OR_NEWER
//             Gizmos.DrawMesh(completePuzzleMesh);
//             Gizmos.DrawWireMesh(completePuzzleMesh);
// #else
//             Gizmos.DrawMesh(completePuzzleMesh);
// #endif
//         }
//         else if (combineIntoSingleMesh && _combinedMesh)
//         {
//             Gizmos.matrix = Matrix4x4.identity;
//             if (IsValidMesh(_combinedMesh))
//             {
// #if UNITY_2021_2_OR_NEWER
//                 Gizmos.DrawMesh(_combinedMesh);
//                 Gizmos.DrawWireMesh(_combinedMesh);
// #else
//                 Gizmos.DrawMesh(_combinedMesh);
// #endif
//             }
//         }
//         else
//         {
//             foreach (var kvp in _matricesByMesh)
//             {
//                 var mesh = kvp.Key;
//                 var mats = kvp.Value;
//                 if (!mesh || mats == null) continue;

//                 for (int i = 0; i < mats.Count; i++)
//                 {
//                     Gizmos.matrix = mats[i];
//                     if (IsValidMesh(mesh))
//                     {
// #if UNITY_2021_2_OR_NEWER
//                         Gizmos.DrawMesh(mesh);
//                         Gizmos.DrawWireMesh(mesh);
// #else
//                         Gizmos.DrawMesh(mesh);
// #endif
//                     }
//                 }
//             }
//         }
//         Gizmos.matrix = Matrix4x4.identity;
//     }
// #endif

//     void LateUpdate()
//     {
//         if (!Application.isPlaying) return;
//         if (drawMode == DrawMode.GizmosOnly) return;
//         if (!runtimeMaterial) return;

//         if (completePuzzleMesh && IsValidMesh(completePuzzleMesh))
//         {
//             var puzzleTransform = completePuzzleTransform ? completePuzzleTransform : board;
//             var matrix = puzzleTransform ? puzzleTransform.localToWorldMatrix : Matrix4x4.identity;

//             Graphics.DrawMesh(
//                 completePuzzleMesh,
//                 matrix,
//                 runtimeMaterial,
//                 render,
//                 null,
//                 0,
//                 null,
//                 ShadowCastingMode.Off,
//                 receiveShadows: false
//             );
//         }
//         else if (combineIntoSingleMesh && _combinedMesh)
//         {
//             Graphics.DrawMesh(
//                 _combinedMesh,
//                 Matrix4x4.identity,
//                 runtimeMaterial,
//                 render,
//                 null,
//                 0,
//                 null,
//                 ShadowCastingMode.Off,
//                 receiveShadows: false
//             );
//         }
//         else
//         {
//             foreach (var kvp in _matricesByMesh)
//             {
//                 var mesh = kvp.Key;
//                 var mats = kvp.Value;
//                 if (!mesh || mats == null || mats.Count == 0) continue;

//                 const int batch = 1023;
//                 for (int i = 0; i < mats.Count; i += batch)
//                 {
//                     int count = Mathf.Min(batch, mats.Count - i);

//                     var subset = mats.GetRange(i, count);
//                     Graphics.DrawMeshInstanced(
//                         mesh,
//                         submeshIndex: 0,
//                         material: runtimeMaterial,
//                         matrices: subset,
//                         properties: null,
//                         castShadows: ShadowCastingMode.Off,
//                         receiveShadows: false,
//                         layer: render
//                     );
//                 }
//             }
//         }
//     }
// }

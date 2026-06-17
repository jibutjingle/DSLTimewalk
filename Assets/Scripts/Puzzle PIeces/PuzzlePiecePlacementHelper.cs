// using Unity.Netcode;
// using UnityEngine;


// [RequireComponent(typeof(PuzzlePiece))]
// public class PuzzlePiecePlacementHelper : MonoBehaviour
// {
//     private PuzzlePiece piece;

//     void Awake()
//     {
//         piece = GetComponent<PuzzlePiece>();
//         var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
//         grab.selectExited.AddListener(_ => TryPlaceIfInZone());
//     }

//     public void TryPlaceIfInZone()
//     {
//         TryPlaceIfInZoneServerRpc();
//     }

//     [ServerRpc(RequireOwnership = false)]
//     void TryPlaceIfInZoneServerRpc()
//     {
//         TryPlaceIfInZoneClientRpc();
//     }

//     [ClientRpc]
//     void TryPlaceIfInZoneClientRpc()
//     {
//         // Debug.Log("TryPlaceIfInZoneClientRpc called");
//         if (!piece || piece.IsPlaced.Value) return;

//         var zones = Physics.OverlapSphere(transform.position, 0.5f);
//         PuzzleSnapZone best = null;
//         float bestDist = Mathf.Infinity;

//         foreach (var c in zones)
//         {
//             var z = c.GetComponentInParent<PuzzleSnapZone>();
//             if (!z) continue;
//             var d = Vector3.Distance(transform.position, z.snapPose.position);
//             if (d < bestDist) { bestDist = d; best = z; }
//         }

//         // Debug.Log("best zone found: " + (best ? best.name : "none"));
//         if (best && piece.NetworkObject)
//         {
//             // Debug.Log("entered if best");
//             best.TryPlaceServerRpc(piece.NetworkObject.NetworkObjectId);
//             // Debug.Log("exited if best");
//         }
//     }
// }
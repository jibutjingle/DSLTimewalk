// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using TMPro;
// using System.Collections.Generic;

// [RequireComponent(typeof(NetworkObject))]
// public class PuzzleSnapZone : NetworkBehaviour
// {
//     [Header("Amount of pieces expected")]
//     public int totalPieces;

//     [SerializeField]
//     private PuzzleGameManager puzzleGameManager;
//     public List<GameObject> PuzzlePieces = new List<GameObject>();
//     public GameObject CompletePuzzle;
//     public GameObject SnapZones;
//     // public GameObject PuzzleButton;
//     public GameObject Outlines;


//     [Header("Count Display")]
//     [SerializeField]
//     private TextMeshProUGUI countDisplayText;

//     [Header("Which piece fits here")]
//     public string expectedPieceId;

//     [Header("Snap pose")]
//     public Transform snapPose;
//     public float positionTolerance = 3f;
//     public float rotationToleranceDeg = 80f;

//     [Header("Zone state")]
//     public NetworkVariable<bool> IsOccupied = new NetworkVariable<bool>(false,
//         NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//     public NetworkVariable<ulong> ClaimedByClientId = new NetworkVariable<ulong>(ulong.MaxValue);

//     void OnTriggerStay(Collider other)
//     {
//         // Debug.Log("OnTriggerStay first called");
//         // if (!IsServer) return;
//         if (IsOccupied.Value) return;

//         var piece = other.GetComponentInParent<PuzzlePiece>();
//         if (!piece || piece.IsPlaced.Value) return;
//         if (piece.pieceId != expectedPieceId) return;

//         var dist = Vector3.Distance(piece.transform.position, snapPose.position);
//         var ang = Quaternion.Angle(piece.transform.rotation, snapPose.rotation);
//         if (dist <= positionTolerance && ang <= rotationToleranceDeg)
//         {
//             if (ClaimedByClientId.Value == ulong.MaxValue && piece.NetworkObject != null)
//                 ClaimedByClientId.Value = piece.NetworkObject.OwnerClientId;
//         }
//         // Debug.Log("OnTriggerStay last called");
//     }

//     void OnTriggerExit(Collider other)
//     {
//         if (!IsServer) return;
//         var piece = other.GetComponentInParent<PuzzlePiece>();
//         if (!piece) return;
//         if (!IsOccupied.Value && piece.NetworkObject && ClaimedByClientId.Value == piece.NetworkObject.OwnerClientId)
//             ClaimedByClientId.Value = ulong.MaxValue;
//     }

//     public bool CanClientPlace(ulong clientId) =>
//         !IsOccupied.Value && (ClaimedByClientId.Value == ulong.MaxValue || ClaimedByClientId.Value == clientId);

//     public bool CanClientPlaceWithPiece(ulong clientId, PuzzlePiece piece) =>
//         !IsOccupied.Value && (
//             ClaimedByClientId.Value == ulong.MaxValue ||
//             ClaimedByClientId.Value == clientId ||
//             (piece != null && piece.NetworkObject != null && piece.NetworkObject.OwnerClientId == clientId)
//         );

//     [ServerRpc(RequireOwnership = false)]
//     public void TryPlaceServerRpc(ulong pieceNetId, ServerRpcParams rpcParams = default)
//     {
//         // Debug.Log("tryplaceserverrpc called");
//         if (IsOccupied.Value) return;
//         var requester = rpcParams.Receive.SenderClientId;

//         if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(pieceNetId, out var netObj)) return;
//         var piece = netObj.GetComponent<PuzzlePiece>();
//         if (!piece || piece.IsPlaced.Value) return;
//         if (piece.pieceId != expectedPieceId) return;

//         if (!CanClientPlaceWithPiece(requester, piece)) return;

//         var dist = Vector3.Distance(piece.transform.position, snapPose.position);
//         var ang = Quaternion.Angle(piece.transform.rotation, snapPose.rotation);
//         if (dist > positionTolerance || ang > rotationToleranceDeg) return;

//         piece.FinalizePlacementServerRpc(NetworkObject.NetworkObjectId, snapPose.position, snapPose.rotation);
//         // Debug.Log("exited finalizeplacementserverrpc");
//         IsOccupied.Value = true;
//         ClaimedByClientId.Value = ulong.MaxValue;
//         CheckAllPiecesPlacedServerRpc();
//         // Debug.Log("exited tryplaceserverrpc");
//     }

//     [ServerRpc(RequireOwnership = false)]
//     public void CheckAllPiecesPlacedServerRpc()
//     {
//         var allZones = FindObjectsOfType<PuzzleSnapZone>();
//         int remainingCount = 0;

//         foreach (var zone in allZones)
//         {
//             if (!zone.IsOccupied.Value)
//             {
//                 remainingCount++;
//             }
//         }

//         CheckAllPiecesPlacedClientRpc(remainingCount);
//     }

//     [ClientRpc]
//     public void CheckAllPiecesPlacedClientRpc(int remainingCount)
//     {
//         // Debug.Log("CheckAllPiecesPlacedclientrpc called");
//         // Debug.Log("Pieces remaining: " + remainingCount);
//         if (remainingCount <= 0)
//         {
//             // Debug.Log("All pieces placed!");
//             puzzleGameManager.TogglePause();
//             foreach (var piece in PuzzlePieces)
//             {
//                 piece.SetActive(false);
//             }
//             SnapZones.SetActive(false);
//             // PuzzleButton.SetActive(false);
//             Outlines.SetActive(false);
//             CompletePuzzle.SetActive(true);
//         }
//     }
// }
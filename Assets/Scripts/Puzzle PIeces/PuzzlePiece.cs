// using Unity.Netcode;
// using UnityEngine;
// using UnityEngine.Networking;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;

// [RequireComponent(typeof(XRGrabInteractable))]
// [RequireComponent(typeof(NetworkObject))]
// public class PuzzlePiece : NetworkBehaviour
// {
//     public string pieceId;
//     public Transform visualRoot;
//     [SerializeField] PuzzleGameManager puzzleGameManager;
//     private AudioSource audioSource;
//     private AudioClip soundConfirm;

//     [HideInInspector] public XRGrabInteractable grab;
//     public NetworkVariable<bool> IsPlaced = new NetworkVariable<bool>(false,
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Server);

//     void Awake()
//     {
//         grab = GetComponent<XRGrabInteractable>();
//         audioSource = GetComponent<AudioSource>();
//         soundConfirm = puzzleGameManager.GetConfirmationSound();
//         // grab.selectEntered.AddListener(_ => OnGrabbed());
//         // grab.selectExited.AddListener(_ => OnReleased());
//     }

//     override public void OnDestroy()
//     {
//         if (grab != null)
//         {
//             grab.selectEntered.RemoveAllListeners();
//             grab.selectExited.RemoveAllListeners();
//         }
//     }

//     // public void OnGrabbed()
//     // {
//     //     OnGrabbedServerRpc();
//     // }

//     // [ServerRpc(RequireOwnership = false)]
//     // public void OnGrabbedServerRpc(ServerRpcParams rpcParams = default)
//     // {
//     //     if (NetworkObject != null && !IsPlaced.Value)
//     //     {
//     //         // Debug.Log("Changing ownership of piece " + pieceId + " to client " + rpcParams.Receive.SenderClientId);
//     //         // Debug.Log("Ongrabbedserverrpc called");
//     //         NetworkObject.ChangeOwnership(rpcParams.Receive.SenderClientId);
//     //     }
//     //     // Debug.Log("Ongrabbedserverrpc called outside if");
//     // }

//     void OnReleased()
//     {
//     }

//     [ServerRpc(RequireOwnership = false)]
//     public void FinalizePlacementServerRpc(ulong zoneNetId, Vector3 snapPos, Quaternion snapRot)
//     {
//         // Debug.Log("FinalizePlacementServerRpc called for piece " + pieceId);
//         if (IsPlaced.Value) return;
//         if (grab) grab.enabled = false;

//         var rb = GetComponent<Rigidbody>();
//         if (rb)
//         {
//             rb.velocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//         }

//         transform.SetPositionAndRotation(snapPos, snapRot);

//         if (rb)
//         {
//             rb.useGravity = false;
//             rb.isKinematic = true;
//         }

//         IsPlaced.Value = true;
//         FinalizePlacementClientRpc(zoneNetId, snapPos, snapRot);
//     }

//     [ClientRpc]
//     public void FinalizePlacementClientRpc(ulong zoneNetId, Vector3 snapPos, Quaternion snapRot)
//     {
//         audioSource.PlayOneShot(soundConfirm);
//         // Debug.Log("FinalizePlacementClientRpc entered");
//         if (grab) grab.enabled = false;

//         var rb = GetComponent<Rigidbody>();
//         if (rb)
//         {
//             rb.velocity = Vector3.zero;
//             rb.angularVelocity = Vector3.zero;
//         }

//         transform.SetPositionAndRotation(snapPos, snapRot);

//         if (rb)
//         {
//             rb.useGravity = false;
//             rb.isKinematic = true;
//         }
//     }
// }

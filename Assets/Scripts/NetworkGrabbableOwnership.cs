using System.Collections;
using System.Collections.Generic;
// using Unity.Netcode;
using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;

// [RequireComponent(typeof(XRGrabInteractable))]
// [RequireComponent(typeof(NetworkObject))]

public class NetworkGrabbableOwnership : MonoBehaviour
{
    // private NetworkObject netObj;
    // [HideInInspector] public XRGrabInteractable grab;

    // private void Awake()
    // {
    //     netObj = GetComponent<NetworkObject>();
    //     grab = GetComponent<XRGrabInteractable>();
    //     grab.selectEntered.AddListener(_ => OnGrabbed());
    //     grab.selectExited.AddListener(_ => OnReleased());
    // }

    // void OnDestroy()
    // {
    //     if (grab != null)
    //     {
    //         grab.selectEntered.RemoveAllListeners();
    //         grab.selectExited.RemoveAllListeners();
    //     }
    // }

    // void OnGrabbed()
    // {
    //     RequestOwnershipServerRpc();

    // }

    // void OnReleased()
    // {
    //     ReturnOwnershipServerRpc();
    // }



    // [ServerRpc(RequireOwnership = false)]
    // private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
    // {
    //     // The client who called this RPC:
    //     ulong clientId = rpcParams.Receive.SenderClientId;

    //     // Server assigns ownership
    //     netObj.ChangeOwnership(clientId);
    // }

    // [ServerRpc(RequireOwnership = false)]
    // private void ReturnOwnershipServerRpc()
    // {
    //     netObj.RemoveOwnership(); // returns to server
    // }
}

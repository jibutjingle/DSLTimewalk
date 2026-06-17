// using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using Unity.XR.CoreUtils;
// using UnityEngine;
// using UnityEngine.Playables;
// using XRMultiplayer;

// [RequireComponent(typeof(Collider))]
// public class DetectPlayersWTriggerArea : NetworkBehaviour
// {
//     [SerializeField] public PlayableDirector transitionTimeline;

//     private readonly HashSet<ulong> clientsInZone = new HashSet<ulong>();
//     private bool localPlayerInZone;
//     private bool transitionStarted;
//     private Coroutine pollCoroutine;
//     private float pollInterval = 1f;

//     public override void OnNetworkSpawn()
//     {
//         if (IsServer)
//             pollCoroutine = StartCoroutine(PeriodicRefresh());

//     }

//     public override void OnNetworkDespawn()
//     {
//         if (pollCoroutine != null)
//         {
//             StopCoroutine(pollCoroutine);
//             pollCoroutine = null;
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (!IsLocalPlayerCollider(other))
//             return;

//         if (localPlayerInZone)
//             return;

//         localPlayerInZone = true;
//         ReportZonePresence(true);
//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (!IsLocalPlayerCollider(other))
//             return;

//         if (!localPlayerInZone)
//             return;

//         localPlayerInZone = false;
//         ReportZonePresence(false);
//     }

//     private bool IsLocalPlayerCollider(Collider other)
//     {
//         if (other.GetComponentInParent<XRINetworkPlayer>() is { } networkPlayer)
//             return networkPlayer.IsOwner;

//         return other.GetComponentInParent<XROrigin>() != null;
//     }

//     private void ReportZonePresence(bool inZone)
//     {
//         if (IsServer)
//             SetClientInZone(NetworkManager.Singleton.LocalClientId, inZone);
//         else
//             SetClientInZoneServerRpc(inZone);
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void SetClientInZoneServerRpc(bool inZone, ServerRpcParams rpcParams = default)
//     {
//         SetClientInZone(rpcParams.Receive.SenderClientId, inZone);
//     }

//     private void SetClientInZone(ulong clientId, bool inZone)
//     {
//         if (!IsServer)
//             return;

//         if (inZone)
//         {
//             clientsInZone.Add(clientId);
//             // Debug.Log("Client added: " + clientId);
//         }
//         else
//         {
//             clientsInZone.Remove(clientId);
//             // Debug.Log("Client removed: " + clientId);
//         }
//         TryStartTransition();
//     }

//     private IEnumerator PeriodicRefresh()
//     {
//         var wait = new WaitForSeconds(pollInterval);

//         while (!transitionStarted)
//         {
//             yield return wait;
//             PruneDisconnectedClients();
//             TryStartTransition();
//         }
//     }

//     private void PruneDisconnectedClients()
//     {
//         if (!IsServer || NetworkManager.Singleton == null)
//             return;

//         clientsInZone.RemoveWhere(id => !NetworkManager.Singleton.ConnectedClients.ContainsKey(id));
//     }

//     private void TryStartTransition()
//     {
//         if (!IsServer || transitionStarted || NetworkManager.Singleton == null)
//             return;

//         int required = NetworkManager.Singleton.ConnectedClients.Count;
//         if (required <= 0)
//             return;

//         if (clientsInZone.Count < required)
//             return;

//         transitionStarted = true;

//         if (transitionTimeline != null)
//             transitionTimeline.Play();

//         PlayTransitionClientRpc();
//     }

//     [ClientRpc]
//     private void PlayTransitionClientRpc()
//     {
//         if (transitionTimeline != null)
//             transitionTimeline.Play();
//     }
// }

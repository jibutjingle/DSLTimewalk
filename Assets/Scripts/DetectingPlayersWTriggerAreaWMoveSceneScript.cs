// using System.Collections;
// using System.Collections.Generic;
// using Unity.Netcode;
// using Unity.XR.CoreUtils;
// using UnityEngine;
// using XRMultiplayer;

// [RequireComponent(typeof(Collider))]
// public class DetectingPlayersWTriggerAreaWMoveSceneScript : NetworkBehaviour
// {
//     [SerializeField] private MoveToNewScene moveToNewSceneScript;
//     [SerializeField] private FadeLightsCallAcrossScenes fadeCallScript;
//     [SerializeField] private float timeToWaitBeforeSceneChange = 5f;

//     private readonly HashSet<ulong> clientsInZone = new HashSet<ulong>();
//     private bool localPlayerInZone;
//     private bool sceneChangeStarted;
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
//             clientsInZone.Add(clientId);
//         else
//             clientsInZone.Remove(clientId);

//         TryMoveScene();
//     }

//     private IEnumerator PeriodicRefresh()
//     {
//         var wait = new WaitForSeconds(pollInterval);

//         while (!sceneChangeStarted)
//         {
//             yield return wait;
//             PruneDisconnectedClients();
//             TryMoveScene();
//         }
//     }

//     private void PruneDisconnectedClients()
//     {
//         if (!IsServer || NetworkManager.Singleton == null)
//             return;

//         clientsInZone.RemoveWhere(id => !NetworkManager.Singleton.ConnectedClients.ContainsKey(id));
//     }

//     private void TryMoveScene()
//     {
//         if (!IsServer || sceneChangeStarted || NetworkManager.Singleton == null)
//             return;

//         int required = NetworkManager.Singleton.ConnectedClients.Count;
//         if (required <= 0)
//             return;

//         if (clientsInZone.Count < required)
//             return;

//         sceneChangeStarted = true;
//         MoveSceneClientRpc();
//     }

//     [ClientRpc]
//     private void MoveSceneClientRpc()
//     {
//         StartCoroutine(FadeThenMoveScene());
//     }

//     private IEnumerator FadeThenMoveScene()
//     {
//         fadeCallScript.CallFadeOut();
//         yield return new WaitForSeconds(timeToWaitBeforeSceneChange);

//         if (moveToNewSceneScript != null)
//             moveToNewSceneScript.ChangeScene();
//     }
// }

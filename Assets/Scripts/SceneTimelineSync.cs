using System.Collections;
using System.Collections.Generic;
// using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;

public class SceneTimelineSync : MonoBehaviour
{
    //     public PlayableDirector timeline;

    //     private int readyCount = 0;
    //     private double startNetworkTime;

    //     public override void OnNetworkSpawn()
    //     {
    //         if (IsClient)
    //         {
    //             // Client tells server it finished loading
    //             NotifyReadyServerRpc();
    //         }
    //     }

    //     [ServerRpc(RequireOwnership = false)]
    //     private void NotifyReadyServerRpc(ServerRpcParams rpcParams = default)
    //     {
    //         readyCount++;

    //         // All connected clients are ready
    //         if (readyCount == NetworkManager.Singleton.ConnectedClients.Count)
    //         {
    //             ScheduleTimelineStart();
    //         }
    //     }

    //     private void ScheduleTimelineStart()
    //     {
    //         // Schedule timeline 1 second in the future
    //         startNetworkTime = NetworkManager.Singleton.ServerTime.Time + 1.0;

    //         StartTimelineClientRpc(startNetworkTime);
    //     }

    //     [ClientRpc]
    //     private void StartTimelineClientRpc(double networkTime)
    //     {
    //         startNetworkTime = networkTime;
    //         StartCoroutine(WaitAndPlayTimeline());
    //     }

    //     private IEnumerator WaitAndPlayTimeline()
    //     {
    //         while (NetworkManager.Singleton.ServerTime.Time < startNetworkTime)
    //             yield return null;

    //         timeline.time = 0;
    //         timeline.Play();
    //     }
}

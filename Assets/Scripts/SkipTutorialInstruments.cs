// using System.Collections;
// using System.Collections.Generic;
// // using Unity.Netcode;
// using UnityEngine;
// using TMPro;

// // [RequireComponent(typeof(NetworkObject))]
// public class SkipTutorialInstruments : MonoBehaviour
// {
//     [SerializeField] private BeginTutorial beginTutorial;
//     [SerializeField] private TextMeshProUGUI skipCountDisplayText;
//     [SerializeField] private float disconnectPollInterval = 1f;

//     private readonly HashSet<ulong> clientsWhoVotedSkip = new HashSet<ulong>();

//     private readonly NetworkVariable<int> skipVoteCount = new NetworkVariable<int>(
//         0,
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Server);

//     private readonly NetworkVariable<int> requiredSkipVotes = new NetworkVariable<int>(
//         0,
//         NetworkVariableReadPermission.Everyone,
//         NetworkVariableWritePermission.Server);

//     private bool localPlayerVoted;
//     private bool skipTriggered;
//     public bool SkipTriggered => skipTriggered; //read-only var
//     private Coroutine pollCoroutine;
//     [SerializeField] private CountdownTimer countdownTimer;
//     [SerializeField] private List<GameObject> disableForSkip;

//     public override void OnNetworkSpawn()
//     {
//         skipVoteCount.OnValueChanged += OnVoteDisplayChanged;
//         requiredSkipVotes.OnValueChanged += OnVoteDisplayChanged;
//         RefreshDisplay();

//         if (IsServer)
//         {
//             SyncVoteState();
//             NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
//             pollCoroutine = StartCoroutine(PeriodicPruneAndCheck());
//         }
//     }

//     public override void OnNetworkDespawn()
//     {
//         skipVoteCount.OnValueChanged -= OnVoteDisplayChanged;
//         requiredSkipVotes.OnValueChanged -= OnVoteDisplayChanged;

//         if (IsServer && NetworkManager.Singleton != null)
//             NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;

//         if (pollCoroutine != null)
//         {
//             StopCoroutine(pollCoroutine);
//             pollCoroutine = null;
//         }
//     }

//     /// <summary>
//     /// Wire this to the skip button's SelectExited UnityEvent.
//     /// </summary>
//     public void OnSkipButtonPressed()
//     {
//         if (!IsSpawned || localPlayerVoted || skipTriggered)
//             return;

//         localPlayerVoted = true;

//         if (IsServer)
//             RegisterSkipVote(NetworkManager.Singleton.LocalClientId);
//         else
//             RegisterSkipVoteServerRpc();
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void RegisterSkipVoteServerRpc(ServerRpcParams rpcParams = default)
//     {
//         RegisterSkipVote(rpcParams.Receive.SenderClientId);
//     }

//     private void RegisterSkipVote(ulong clientId)
//     {
//         if (!IsServer || skipTriggered || NetworkManager.Singleton == null)
//             return;

//         if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
//             return;

//         if (!clientsWhoVotedSkip.Add(clientId))
//             return;

//         SyncVoteState();
//         TryTriggerSkip();
//     }

//     private void OnClientDisconnected(ulong clientId)
//     {
//         if (!IsServer)
//             return;

//         clientsWhoVotedSkip.Remove(clientId);
//         SyncVoteState();
//         TryTriggerSkip();
//     }

//     private IEnumerator PeriodicPruneAndCheck()
//     {
//         var wait = new WaitForSeconds(disconnectPollInterval);

//         while (!skipTriggered)
//         {
//             yield return wait;
//             PruneDisconnectedVoters();
//             TryTriggerSkip();
//         }
//     }

//     private void PruneDisconnectedVoters()
//     {
//         if (!IsServer || NetworkManager.Singleton == null)
//             return;

//         int removed = clientsWhoVotedSkip.RemoveWhere(
//             id => !NetworkManager.Singleton.ConnectedClients.ContainsKey(id));

//         if (removed > 0)
//             SyncVoteState();
//     }

//     private void SyncVoteState()
//     {
//         skipVoteCount.Value = clientsWhoVotedSkip.Count;
//         requiredSkipVotes.Value = GetRequiredVotes();
//     }

//     private static int GetRequiredVotes(int connectedCount)
//     {
//         if (connectedCount <= 0)
//             return 0;

//         // Strict majority: more than half of connected players must vote to skip
//         return (connectedCount / 2) + 1;
//     }

//     private int GetRequiredVotes()
//     {
//         if (NetworkManager.Singleton == null)
//             return 0;

//         return GetRequiredVotes(NetworkManager.Singleton.ConnectedClients.Count);
//     }

//     private void TryTriggerSkip()
//     {
//         if (!IsServer || skipTriggered || NetworkManager.Singleton == null)
//             return;

//         int connected = NetworkManager.Singleton.ConnectedClients.Count;
//         if (connected <= 0)
//             return;

//         int votes = clientsWhoVotedSkip.Count;
//         if (votes * 2 <= connected)
//             return;

//         skipTriggered = true;
//         TriggerSkipClientRpc();
//     }

//     [ClientRpc]
//     private void TriggerSkipClientRpc()
//     {
//         if (beginTutorial != null)
//         {
//             beginTutorial.StartPlayableDirectorCall();
//             countdownTimer.StopCountdown();
//             for (int i = 0; i < disableForSkip.Count; i++)
//             {
//                 disableForSkip[i].SetActive(false);
//             }
//         }
//     }

//     private void OnVoteDisplayChanged(int previous, int current)
//     {
//         RefreshDisplay();
//     }

//     private void RefreshDisplay()
//     {
//         if (skipCountDisplayText == null)
//             return;

//         skipCountDisplayText.text = $"{skipVoteCount.Value} / {requiredSkipVotes.Value} voted to skip";
//     }
// }

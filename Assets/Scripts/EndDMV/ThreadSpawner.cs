// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using XRMultiplayer;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;
// using Unity.Netcode;
// using UnityEngine.Playables;


// public class ThreadSpawner : NetworkBehaviour
// {
//     [Header("Thread Settings")]
//     public Threads threadPrefab;
//     // private XRINetworkPlayer _ownerPlayer;
//     public Transform _instrumentAnchor;
//     public Transform _joseAnchor;
//     private Threads _threadInstance;
//     private Coroutine _deinitializeCoroutine;
//     [SerializeField] private float _shrinkDelaySeconds = 5f;


//     public void onSelectEntered(SelectEnterEventArgs args)
//     {
//         CreateThreadForPlayerServerRpc();
//     }

//     public void OnSelectExited(SelectExitEventArgs args)
//     {
//         DeinitializeThreadForPlayerServerRpc();
//     }

//     public void onHoverEntered(HoverEnterEventArgs args) //only for the Djembe drum currently
//     {
//         CreateThreadForPlayerServerRpc();
//     }

//     public void onHoverExited(HoverExitEventArgs args) //only for the Djembe drum currently
//     {
//         DeinitializeThreadForPlayerServerRpc();
//     }
//     private IEnumerator DelayedDeinitialize()
//     {
//         yield return new WaitForSeconds(_shrinkDelaySeconds);

//         if (_threadInstance != null)
//         {
//             _threadInstance.Deinitialize(_instrumentAnchor, _joseAnchor);
//         }

//         _deinitializeCoroutine = null;
//     }

//     [ServerRpc(RequireOwnership = false)]
//     public void CreateThreadForPlayerServerRpc()
//     {
//         CreateThreadForPlayerClientRpc();
//     }

//     [ClientRpc]
//     private void CreateThreadForPlayerClientRpc()
//     {
//         if (_deinitializeCoroutine != null)
//         {
//             StopCoroutine(_deinitializeCoroutine);
//             _deinitializeCoroutine = null;
//         }

//         if (_threadInstance == null)
//         {
//             _threadInstance = Instantiate(threadPrefab);
//         }
//         _threadInstance.Initialize(_instrumentAnchor, _joseAnchor);

//         //increase audio playable asset for all
//     }

//     [ServerRpc(RequireOwnership = false)]
//     public void DeinitializeThreadForPlayerServerRpc()
//     {
//         DeinitializeThreadForPlayerClientRpc();
//     }

//     [ClientRpc]
//     private void DeinitializeThreadForPlayerClientRpc()
//     {
//         if (_deinitializeCoroutine != null)
//         {
//             StopCoroutine(_deinitializeCoroutine);
//             _deinitializeCoroutine = null;
//         }
//         _deinitializeCoroutine = StartCoroutine(DelayedDeinitialize());
//     }

//     void OnDisable()
//     {
//         if (_deinitializeCoroutine != null)
//         {
//             StopCoroutine(_deinitializeCoroutine);
//             _deinitializeCoroutine = null;
//         }
//     }


//     public override void OnDestroy()
//     {
//         if (_threadInstance != null)
//         {
//             Destroy(_threadInstance.gameObject);
//         }
//     }
// }
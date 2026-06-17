// using System.Collections;
// //using System.Collections.Generic;
// // using Unity.Netcode;
// using UnityEngine;

// public class MarimbaSounds : MonoBehaviour
// {
//     public GameObject drumstickObject;
//     //public AudioClip Sound1; // Assign the sound clip in the Inspector
//     private AudioSource audioSource;

//     private void Awake()
//     {
//         audioSource = GetComponent<AudioSource>();
//     }
//     //void Start()
//     //{
//     //    audioSource = GetComponent<AudioSource>();
//     //    audioSource.playOnAwake = false; // Prevent the sound from playing on start
//     //}

//     private void OnCollisionEnter(Collision collision)
//     {
//         if (collision.transform.name == drumstickObject.name)
//         {
//             audioSource.Play();

//             if (IsServer)
//             {
//                 // Server already knows drum was hit
//                 PlaySoundClientRpc(0.00);
//             }
//             else if (IsOwner)
//             {
//                 // Tell server this drum was hit
//                 NotifyHitServerRpc();
//             }
//         }
//     }

//     [ServerRpc(RequireOwnership = false)]
//     private void NotifyHitServerRpc(ServerRpcParams rpcParams = default)
//     {
//         //Debug.Log("Made it into playnoteserver" + gameObject.name);
//         double eventTime = NetworkManager.ServerTime.Time + 0.05; // schedule a bit ahead

//         // Server confirms hit and tells all clients
//         PlaySoundClientRpc(eventTime);
//     }

//     [ClientRpc]
//     private void PlaySoundClientRpc(double scheduledTime, ClientRpcParams rpcParams = default)
//     {
//         if (audioSource != null)
//         {
//             // Convert network time to local audio DSP time
//             double dspTime = AudioSettings.dspTime + (scheduledTime - NetworkManager.ServerTime.Time);
//             audioSource.PlayScheduled(dspTime);
//         }
//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("DrumInstrument"))
//         {
//             audioSource.Play();

//             if (IsServer)
//             {
//                 // Server already knows drum was hit
//                 PlaySoundClientRpc(0.00);
//             }
//             else if (IsOwner)
//             {
//                 // Tell server this drum was hit
//                 NotifyHitServerRpc();
//             }
//         }
//     }

// }

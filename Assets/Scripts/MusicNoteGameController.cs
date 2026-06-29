using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
// using Unity.Netcode;
using UnityEngine;
using UnityEngine.Playables;
// using UnityEngine.XR.Interaction.Toolkit.Interactors;
// using UnityEngine.XR.Interaction.Toolkit;


public class MusicNoteGameController : MonoBehaviour
{
    //     public AudioSource babyAudioSource;

    //     [SerializeField]
    //     private Animator babyAnimator;

    //     [SerializeField]
    //     private AudioClip correctClip;

    //     [SerializeField]
    //     private AudioClip finishClip;

    //     [SerializeField]
    //     private float finishVolume = 0.5f;

    //     [SerializeField]
    //     private GameObject musicStaffGame;

    //     [SerializeField]
    //     private Transform musicStaffGamePlayPosition;

    //     [SerializeField]
    //     private PlayableDirector babyDirector;

    //     private PlayableDirector playableDirectorFinish;

    //     public static MusicNoteGameController Instance;

    //     [HideInInspector]
    //     public bool musicGameStarted = false;

    //     [HideInInspector]
    //     public bool endGameStarted = false;

    //     // [Header("Sockets")]
    //     // public List<XRSocketInteractor> musicNoteSockets = new List<XRSocketInteractor>();

    //     // // Tracks which sockets are filled
    //     // private HashSet<XRSocketInteractor> filledSockets = new HashSet<XRSocketInteractor>();

    //     private AudioSource audiosource;

    //     [HideInInspector]
    //     public int endGameTriggerCount = 7;//replaced later with count of socket list

    //     private int currentCount = 0;

    //     public List<GameObject> gameFinishObjects = new List<GameObject>();


    //     [SerializeField]
    //     private MoveToNewScene moveSceneScript;

    //     private void Awake()
    //     {
    //         Instance = this;
    //     }

    //     // Start is called before the first frame update
    //     void Start()
    //     {
    //         audiosource = GetComponent<AudioSource>();
    //         playableDirectorFinish = GetComponent<PlayableDirector>();

    //         foreach (var socket in musicNoteSockets)
    //         {
    //             socket.selectEntered.AddListener(OnSocketFilled);
    //             socket.selectExited.AddListener(OnSocketEmptied);
    //         }

    //         endGameTriggerCount = musicNoteSockets.Count;
    //     }

    //     private void OnSocketFilled(SelectEnterEventArgs args)
    //     {
    //         XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

    //         if (IsHost)
    //         {
    //             if (socket == null)
    //                 return;

    //             // Prevent double counting
    //             if (filledSockets.Contains(socket))
    //                 return;

    //             filledSockets.Add(socket);

    //             RegisterCorrectNoteServerRpc(true);
    //         }
    //     }


    //     private void OnSocketEmptied(SelectExitEventArgs args)
    //     {
    //         XRSocketInteractor socket = args.interactorObject as XRSocketInteractor;

    //         if (IsHost)
    //         {
    //             if (socket == null)
    //                 return;

    //             if (!filledSockets.Contains(socket))
    //                 return;

    //             filledSockets.Remove(socket);

    //             RegisterCorrectNoteServerRpc(false);
    //         }
    //     }


    //     [ServerRpc(RequireOwnership = false)]
    //     private void EndGameServerRpc()
    //     {
    //         EndGameClientRpc();
    //     }

    //     [ClientRpc]
    //     private void EndGameClientRpc()
    //     {
    //         StartCoroutine(WaitForEndScene());
    //     }
    //     IEnumerator WaitForEndScene()
    //     {
    //         babyAudioSource.Stop();
    //         audiosource.clip = finishClip;
    //         audiosource.volume = finishVolume;
    //         audiosource.Play();
    //         foreach (var item in gameFinishObjects)
    //         {
    //             item.gameObject.SetActive(true);
    //         }

    //         yield return new WaitForSeconds(5); //wait the length of the finishclip clip


    //         DestroyMusicNotesServerRpc(); //Remove musical notes and staff
    //         playableDirectorFinish.Play();
    //         //MoveToNewScene is triggered by a marker on the GamePlay Manager Timeline
    //     }

    //     [ServerRpc(RequireOwnership = false)]
    //     public void StartMusicNoteGameServerRpc() //called by marker on Baby timeline
    //     {
    //         musicGameStarted = true;
    //         musicStaffGame.transform.position = musicStaffGamePlayPosition.position;
    //     }

    //     private void PlayAudio(AudioClip clip) //Audio player function
    //     {
    //         babyAudioSource.clip = clip;
    //         babyAudioSource.Play();
    //     }

    //     // [ServerRpc(RequireOwnership = false)]
    //     private void DestroyMusicNotesServerRpc() // called with a  signal emitter on Timeline
    //     {
    //         DestroyMusicNotesClientRpc();
    //     }

    //     // [ClientRpc]
    //     private void DestroyMusicNotesClientRpc()
    //     {
    //         var notesArray = GameObject.FindGameObjectsWithTag("MusicNotes");
    //         for (int i = 0; i < notesArray.Length; i++)
    //         {
    //             Destroy(notesArray[i]);
    //         }
    //         musicStaffGame.SetActive(false);
    //     }

    //     // [ServerRpc(RequireOwnership =false)]
    //     public void PlayAnimationServerRpc(string trigger)
    //     {
    //         // Tell all clients to play the animation
    //         PlayAnimationClientRpc();
    //     }

    //     // [ServerRpc(RequireOwnership = false)]
    //     public void RegisterCorrectNoteServerRpc(bool notePlaced)
    //     {
    //         if (notePlaced)
    //         {
    //             currentCount++;
    //             PlayAnimationClientRpc();
    //         }
    //         else
    //         {
    //             currentCount--;
    //         }
    //     }

    //     // [ClientRpc]
    //     private void PlayAnimationClientRpc()
    //     {
    //         if (playableDirectorFinish != null && currentCount >= endGameTriggerCount)
    //         {
    //             EndGameServerRpc();
    //         }
    //         else
    //         {
    //             babyAnimator.SetTrigger("Correct Trigger");
    //             PlayAudio(correctClip);
    //         }
    //     }
}



using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using RvSdk.Component;
using RvSdk.Avatar;

// #if UNITY_EDITOR
// using UnityEditor;
// #endif
// public enum DrumSoundType
// {
//     Soft,
//     Medium,
//     Hard
// }

// [RequireComponent(typeof(AudioSource))]

public class djembe_sound : MonoBehaviour
{
    //     public AudioClip softImpactClip;
    //     public AudioClip mediumImpactClip;
    //     public AudioClip hardImpactClip;

    //     public float mediumThreshold = 5f; //currently unused
    //     public float hardThreshold = 10f; //currently unused
    //     private AudioSource audioSource;
    //     [HideInInspector] public bool LightThreadsInScene;
    //     private ThreadSpawner threadSpawner;
    //     private AudioIncreaseOnGrab audioIncreaseOnGrab;
    //     [HideInInspector] public GameObject lightAura;
    //     private ClientToServerTrigger clientToServerTrigger;
    //     private NetworkSound networkSound;

    //     void Start()
    //     {
    //         audioSource = GetComponent<AudioSource>();
    //         threadSpawner = GetComponent<ThreadSpawner>();
    //         audioIncreaseOnGrab = GetComponent<AudioIncreaseOnGrab>();
    //     }

    //     public void HandleServerHit(string triggerName, AvatarController avatar, string clipIndex)
    //     {
    //         if (networkSound == null) return;
    //         networkSound.PlayOnce(int.Parse(clipIndex));
    //     }

    //     private bool IsWithinNearDistance(IXRInteractor interactor, float nearMaxDistance)
    //     {
    //         return Vector3.Distance(interactor.transform.position, transform.position) <= nearMaxDistance;
    //     }

    //     public void TryGetNearCaster(HoverEnterEventArgs args)
    //     {
    //         if (!IsWithinNearDistance(args.interactorObject, 1.0f)) return;
    //         PlaySoundTest(hardImpactClip);
    //         if (LightThreadsInScene)
    //         {
    //             threadSpawner.onHoverEntered(args);
    //             audioIncreaseOnGrab.onHoverEntered(args);
    //             lightAura.SetActive(true);
    //         }
    //     }

    //     public void PlaySoundTest(AudioClip clip)
    //     {

    //         DrumSoundType type = DrumSoundType.Soft;

    //         if (clip == mediumImpactClip) type = DrumSoundType.Medium;
    //         else if (clip == hardImpactClip) type = DrumSoundType.Hard;

    //         PlayNoteServerRpc(type);

    //     }

    //     // [ServerRpc(RequireOwnership = false)]
    //     private void PlayNoteServerRpc(DrumSoundType type)
    //     {
    //         // Tell everyone (including the original player) to play the sound
    //         PlayNoteClientRpc(type);
    //     }

    //     // [ClientRpc]
    //     private void PlayNoteClientRpc(DrumSoundType type)
    //     {
    //         AudioClip selectedClip = null;

    //         switch (type)
    //         {
    //             case DrumSoundType.Soft: selectedClip = softImpactClip; break;
    //             case DrumSoundType.Medium: selectedClip = mediumImpactClip; break;
    //             case DrumSoundType.Hard: selectedClip = hardImpactClip; break;
    //         }

    //         if (selectedClip != null)
    //             audioSource.PlayOneShot(selectedClip);

    //     }

}

// #if UNITY_EDITOR
// [CustomEditor(typeof(djembe_sound))]
// public class djembeEditor : Editor
// {
//     public override void OnInspectorGUI()
//     {
//         base.OnInspectorGUI();
//         var script = (djembe_sound)target;

//         script.LightThreadsInScene = EditorGUILayout.Toggle("Light Threads In Scene", script.LightThreadsInScene);

//         if (script.LightThreadsInScene == false)
//             return;

//         script.lightAura = EditorGUILayout.ObjectField("Light Aura Object", script.lightAura, typeof(GameObject), true) as GameObject;
//     }
// }
// #endif
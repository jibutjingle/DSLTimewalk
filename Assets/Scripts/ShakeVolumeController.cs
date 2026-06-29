using System.Collections;
//using System.Collections.Generic;
// using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ShakeVolumeController : MonoBehaviour
{
    // public float sensitivity = 1.0f;         // How responsive the volume is to shaking
    // public float maxVolume = 1.0f;           // Maximum volume
    // public float shakeThreshold = 0.1f;      // Minimum movement to be considered a shake
    // public float shakeAmount = 0.05f;        // Visual shake intensity
    // private float soundCooldown = 3.0f; // minimum time between sounds
    // private float lastSoundTime = 0f;

    // private AudioSource audioSource;

    // [SerializeField]
    // private AudioClip soundClip; //audioclip to play, currently only used in network setting, local is already attached to audioSource

    // private Vector3 lastPosition;
    // private float currentVolume;

    // private ShakeVolumeController shakeVolumeController;

    // void Start()
    // {
    //     audioSource = GetComponent<AudioSource>();
    //     lastPosition = transform.position;
    //     shakeVolumeController.GetComponent<ShakeVolumeController>();
    // }
    // void Update()
    // {
    //     // Calculate movement delta
    //     Vector3 movement = transform.position - lastPosition;
    //     float shakeIntensity = movement.magnitude / Time.deltaTime;
    //     //Debug.Log("Shake Intensity: " + shakeIntensity);

    //     // Apply threshold
    //     if (shakeIntensity < shakeThreshold)
    //     {
    //         shakeIntensity = 0;
    //     }
    //     // Map intensity to volume
    //     currentVolume = Mathf.Clamp(shakeIntensity * sensitivity, 0, maxVolume);
    //     audioSource.volume = currentVolume;
    //     //Debug.Log("Current volume played: "+ currentVolume);

    //     if (shakeIntensity >= shakeThreshold && Time.time - lastSoundTime > soundCooldown)
    //     {
    //         lastSoundTime = Time.time;

    //         PlayNoteServerRpc();
    //     }

    //     lastPosition = transform.position;
    // }

    // void OnDisable()
    // {
    //     shakeVolumeController.enabled = false;
    // }

    // // [ServerRpc(RequireOwnership = false)]
    // private void PlayNoteServerRpc()
    // {
    //     // Tell everyone (including the original player) to play the sound

    //     audioSource.volume = currentVolume;
    //     // PlayNoteClientRpc();
    //     audioSource.PlayOneShot(soundClip);
    // }

    // // [ClientRpc]
    // // private void PlayNoteClientRpc()
    // // {
    // //     audioSource.PlayOneShot(soundClip);


    // // }
}

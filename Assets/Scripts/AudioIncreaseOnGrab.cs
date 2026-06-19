using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
//using UnityEngine.Timeline;

public class AudioIncreaseOnGrab : MonoBehaviour
{

    //     [Range(0f, 2f)] public float volumeMultiplier = 1.5f; // 1.0 = normal, >1 = louder
    //     public AudioSource backgroundMusic; //currently Beethoven's Fur Elise

    //     public void onSelectEntered(SelectEnterEventArgs args)
    //     {
    //         AudioIncreaseForPlayerServerRpc();
    //     }

    //     public void OnSelectExited(SelectExitEventArgs args)
    //     {
    //         AudioDecreaseForPlayerServerRpc();
    //     }

    //     public void onHoverEntered(HoverEnterEventArgs args)//only for the Djembe drum currently
    //     {
    //         AudioIncreaseForPlayerServerRpc();
    //     }

    //     public void onHoverExited(HoverExitEventArgs args)//only for the Djembe drum currently
    //     {
    //         AudioDecreaseForPlayerServerRpc();
    //     }

    //     [ServerRpc(RequireOwnership = false)]
    //     public void AudioIncreaseForPlayerServerRpc()
    //     {
    //         AudioIncreaseForPlayerClientRpc();
    //     }

    //     [ClientRpc]
    //     private void AudioIncreaseForPlayerClientRpc()
    //     {
    //         backgroundMusic.volume *= volumeMultiplier; //increase audio playable asset for all

    //     }

    //     [ServerRpc(RequireOwnership = false)]
    //     public void AudioDecreaseForPlayerServerRpc()
    //     {
    //         AudioDecreaseForPlayerClientRpc();
    //     }

    //     [ClientRpc]
    //     private void AudioDecreaseForPlayerClientRpc()
    //     {
    //         backgroundMusic.volume /= volumeMultiplier; //increase audio playable asset for all
    //     }

}

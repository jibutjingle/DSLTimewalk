using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class IdleAudioController : StateMachineBehaviour
{
    private AudioSource audioSource;

    // Called when entering the state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource == null)
        {
            audioSource = animator.GetComponent<AudioSource>();
        }
            

        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
            
    }

    // Called when exiting the state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
            
    }
}

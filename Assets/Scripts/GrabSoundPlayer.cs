using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;

// [RequireComponent(typeof(AudioSource))]
// [RequireComponent(typeof(XRGrabInteractable))]
public class GrabSoundPlayer : MonoBehaviour
{
    //     private AudioSource audioSource;
    //     private XRGrabInteractable grabInteractable;

    //     void Awake()
    //     {
    //         audioSource = GetComponent<AudioSource>();
    //         grabInteractable = GetComponent<XRGrabInteractable>();
    //         audioSource.playOnAwake = false;
    //         //audioSource.loop = true; // Optional: Loop while holding
    //     }

    //     void OnEnable()
    //     {
    //         grabInteractable.selectEntered.AddListener(OnGrab);
    //         grabInteractable.selectExited.AddListener(OnRelease);
    //     }

    //     void OnDisable()
    //     {
    //         grabInteractable.selectEntered.RemoveListener(OnGrab);
    //         grabInteractable.selectExited.RemoveListener(OnRelease);
    //     }

    //     private void OnGrab(SelectEnterEventArgs args)
    //     {
    //         audioSource.Play();
    //     }

    //     private void OnRelease(SelectExitEventArgs args)
    //     {
    //         audioSource.Stop();
    //     }
}

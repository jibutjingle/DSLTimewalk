using UnityEngine;

[RequireComponent(typeof(DreamscapeGrabbable))]
[RequireComponent(typeof(AudioSource))]
public class ShakeVolumeController : MonoBehaviour
{
    [SerializeField] DreamscapeGrabbable grabbable;
    [SerializeField] AudioSource audioSource;

    [SerializeField] float sensitivity = 0.1f;
    [SerializeField] float maxVolume = 1f;
    [SerializeField] float shakeThreshold = 2f;

    bool _wasHeldLocally;

    void Awake()
    {
        if (grabbable == null)
            grabbable = GetComponent<DreamscapeGrabbable>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
            audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (grabbable == null || audioSource == null || audioSource.clip == null)
            return;

        bool heldLocally = grabbable.IsHeldByLocalPlayer;

        if (!heldLocally)
        {
            if (_wasHeldLocally)
                StopShakeAudio();

            _wasHeldLocally = false;
            return;
        }

        if (!_wasHeldLocally)
            StartShakeAudio();

        _wasHeldLocally = true;

        float shakeSpeed = grabbable.CurrentHandShakeSpeed;
        float intensity = shakeSpeed >= shakeThreshold ? shakeSpeed - shakeThreshold : 0f;
        audioSource.volume = Mathf.Clamp(intensity * sensitivity, 0f, maxVolume);
    }

    void OnDisable()
    {
        StopShakeAudio();
        _wasHeldLocally = false;
    }

    void StartShakeAudio()
    {
        audioSource.volume = 0f;

        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    void StopShakeAudio()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.volume = 0f;
    }

#if UNITY_EDITOR
    public void AutoAssignComponents()
    {
        grabbable = GetComponent<DreamscapeGrabbable>();
        audioSource = GetComponent<AudioSource>();
    }
#endif
}

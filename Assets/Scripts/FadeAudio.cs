using System.Collections;
using UnityEngine;

public class FadeAudio : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 5f;
    [Tooltip("Y axis = how much volume to keep. Keep this high for longer to hold the music before it tapers off.")]
    [SerializeField] private AnimationCurve fadeCurve = new AnimationCurve(
        new Keyframe(0f, 1f),
        new Keyframe(0.7f, 0.92f),
        new Keyframe(1f, 0f)
    );

    [Header("Cross-Scene Bridge (optional)")]
    [SerializeField] private string persistentAudioObjectName;

    private AudioSource audioSource;
    private float defaultVolume;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            defaultVolume = audioSource.volume;
    }

    public void CallFadeOut()
    {
        FadeOutInternal();
    }

    public void FadeOut()
    {
        FadeOutInternal();
    }

    private void FadeOutInternal()
    {
        if (audioSource == null)
        {
            if (TryFadePersistentAudioByName())
                return;

            Debug.LogWarning("[FadeAudio] No AudioSource found to fade. Add an AudioSource to this object, or set Persistent Audio Object Name on a bridge FadeAudio in this scene.");
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutRoutine());
    }

    private bool TryFadePersistentAudioByName()
    {
        if (string.IsNullOrEmpty(persistentAudioObjectName))
            return false;

        GameObject persistentAudio = GameObject.Find(persistentAudioObjectName);
        if (persistentAudio == null)
        {
            Debug.LogWarning($"[FadeAudio] Could not find persistent audio object: {persistentAudioObjectName}");
            return true;
        }

        FadeAudio fadeAudio = persistentAudio.GetComponent<FadeAudio>();
        if (fadeAudio == null)
        {
            Debug.LogWarning($"[FadeAudio] {persistentAudioObjectName} is missing a FadeAudio component.");
            return true;
        }

        fadeAudio.FadeOutInternal();
        return true;
    }

    public void FadeOutAndStopAt(float clipTime)
    {
        if (audioSource == null)
        {
            Debug.LogWarning("[FadeAudio] No AudioSource found to fade.");
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAtTimeRoutine(clipTime));
    }

    private IEnumerator FadeOutRoutine()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;
        const float minVolume = 0.0001f;
        float startVolumeLog = Mathf.Log10(Mathf.Max(startVolume, minVolume));
        float minVolumeLog = Mathf.Log10(minVolume);

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float fadeAmount = 1f - fadeCurve.Evaluate(t);
            audioSource.volume = Mathf.Pow(10f, Mathf.Lerp(startVolumeLog, minVolumeLog, fadeAmount));
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = defaultVolume;
        fadeCoroutine = null;
    }

    private IEnumerator FadeOutAtTimeRoutine(float stopTime)
    {
        float fadeStart = stopTime - fadeDuration;

        while (audioSource.isPlaying && audioSource.time < fadeStart)
            yield return null;

        yield return FadeOutRoutine();
    }
}

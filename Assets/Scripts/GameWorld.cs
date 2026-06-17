using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class GameWorld : MonoBehaviour
{
    [SerializeField] private UniversalRendererData rendererData;
    [SerializeField] private Volume postProcessing;
    [SerializeField] private float newBloomIntensity;
    [SerializeField] private float dimDuration;
    private float originalBloomIntensity;
    private Bloom bloom;
    private string renderFeatureName = "GameWorld";
    ScriptableRendererFeature gameWorldRenderFeature;

    void Start()
    {
        if (postProcessing.profile.TryGet(out bloom))
        {
            originalBloomIntensity = bloom.intensity.value;
        }
        StartCoroutine(InitializeGameWorld());
    }

    void OnDisable()
    {
        SetGameWorld(false);
        bloom.intensity.value = originalBloomIntensity;
    }

    IEnumerator InitializeGameWorld()
    {
        yield return null;

        if (rendererData == null)
        {
            Debug.LogError("GameWorld: Unable to find UniversalRendererData.");
            yield break;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature == null) continue;

            if (feature.name == renderFeatureName)
            {
                gameWorldRenderFeature = feature;
                break;
            }
        }

        if (gameWorldRenderFeature == null)
        {
            Debug.LogError($"GameWorld: Unable to find Render Feature named {renderFeatureName}.");
            yield break;
        }
    }

    public void SetGameWorld(bool enabled)
    {
        if (gameWorldRenderFeature != null)
        {
            gameWorldRenderFeature.SetActive(enabled);
            if (enabled)
            {
                StartCoroutine(dimBloom());
            }
            else
            {
                bloom.intensity.value = originalBloomIntensity;
            }
        }
    }

    IEnumerator dimBloom()
    {
        float timeElapsed = 0;
        while (timeElapsed < dimDuration)
        {
            timeElapsed += Time.deltaTime;
            float ongoingDuration = timeElapsed / dimDuration;
            float newBloom = Mathf.Lerp(originalBloomIntensity, newBloomIntensity, ongoingDuration);
            bloom.intensity.value = newBloom;
            yield return null;
        }
        // Debug.Log("Exited dimBloom at " + Time.time);
    }
}

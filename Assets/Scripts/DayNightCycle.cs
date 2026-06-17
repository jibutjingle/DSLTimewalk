using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DayNightCycle : MonoBehaviour
{
    [Header("Lights")]
    [SerializeField] private Light sunLight;
    // [SerializeField] private List<Light> interiorLights;
    [SerializeField] private List<Renderer> lightFixtureMeshes;
    private Material[] FixtureMaterials;

    [Header("Post Processing Components")]
    [SerializeField] private Volume postProcessing;
    private Bloom bloom;
    private ColorAdjustments colorAdjustments;
    private float originalBloomIntensity;
    [SerializeField] private float newBloomIntensity;
    private float originalPostExporsure;
    [SerializeField] private float newPostExposure;

    [Header("Dimming and Lighting Specs")]
    [SerializeField] private float secondsPerCycle;
    [SerializeField] private float dimDuration;
    private Color originalEmissionColor;
    [SerializeField] private Color newEmissionColor;
    private Color originalSunLightColor;
    // [SerializeField] private float sunOffHeightAngle;
    // [SerializeField] private float sunOnHeightAngle;

    void Start()
    {
        FixtureMaterials = lightFixtureMeshes[0].materials;
        // Debug.LogWarning("Captured Fixture Materials from: " + lightFixtureMeshes[0].name);

        if (postProcessing.profile.TryGet(out bloom))
        {
            originalBloomIntensity = bloom.intensity.value;
        }

        if (postProcessing.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments.active = false;
            originalPostExporsure = colorAdjustments.postExposure.value;
        }

        originalEmissionColor = FixtureMaterials[1].color;
        originalSunLightColor = sunLight.color;
        Debug.Log("sunlight local angle: " + sunLight.transform.localEulerAngles.x);
        Debug.Log("sunlight angle: " + sunLight.transform.eulerAngles.x);
        Debug.Log("sunlight localRotation: " + sunLight.transform.localRotation.x);
        Debug.Log("sunlight Rotation: " + sunLight.transform.rotation.x);

    }

    void OnDestroy()
    {
        FixtureMaterials[1].SetColor("_EmissionColor", originalEmissionColor);
        bloom.intensity.value = originalBloomIntensity;
        colorAdjustments.postExposure.value = originalPostExporsure;
        colorAdjustments.active = false;
        sunLight.color = originalSunLightColor;
    }

    public void StartLightShow()
    {
        StartCoroutine(RunLightShow());
    }

    IEnumerator RunLightShow()
    {
        Coroutine bloom = StartCoroutine(dimBloom());
        Coroutine emission = StartCoroutine(dimEmission());
        Coroutine adjustments = StartCoroutine(turnOffColorAdjustments());
        yield return bloom;
        yield return emission;
        yield return adjustments;

        StartCoroutine(spinSun());
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

    IEnumerator dimEmission()
    {
        float timeElapsedv2 = 0;
        while (timeElapsedv2 < dimDuration)
        {
            timeElapsedv2 += Time.deltaTime;
            float ongoingDuration = timeElapsedv2 / dimDuration;
            Color newColor = Color.Lerp(originalEmissionColor, newEmissionColor, ongoingDuration);
            FixtureMaterials[1].SetColor("_EmissionColor", newColor);
            yield return null;
        }
        // Debug.Log("Exited dimEmission at " + Time.time);
    }

    IEnumerator turnOffColorAdjustments()
    {
        colorAdjustments.active = true;
        float timeElapsedv3 = 0;
        while (timeElapsedv3 < dimDuration)
        {
            timeElapsedv3 += Time.deltaTime;
            float ongoingDuration = timeElapsedv3 / dimDuration;
            float postExposureValue = Mathf.Lerp(originalPostExporsure, newPostExposure, ongoingDuration);
            colorAdjustments.postExposure.value = postExposureValue;
            yield return null;
        }
        // Debug.Log("Exited turnOffColorAdjustments at " + Time.time);
    }

    IEnumerator spinSun()
    {
        while (true)
        {
            float degreesPerSecond = 360f / secondsPerCycle;
            float angle = degreesPerSecond * Time.deltaTime;

            // Debug.Log("angle: " + angle);
            sunLight.transform.Rotate(Vector3.right, angle, Space.Self);
            // if (sunLight.transform.eulerAngles.x > sunOffHeightAngle)
            // {
            //     // Debug.Log(angle + " > " + sunOffHeightAngle);
            //     sunLight.color = Color.black;
            // }
            // else if (sunLight.transform.eulerAngles.x > sunOnHeightAngle && sunLight.transform.eulerAngles.x < sunOffHeightAngle)
            // {
            //     sunLight.color = originalSunLightColor;
            // }

            yield return null;
        }
    }
}

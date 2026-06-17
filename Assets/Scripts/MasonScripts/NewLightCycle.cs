using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClassroomLightShow : MonoBehaviour
{
    [Header("1. Setup & Debug")]
    public bool setupMode = false;
    [Range(0, 360)] public float startHeight = 10f;
    [Range(0, 360)] public float facingDirection = -90f;

    [Header("2. Scene References")]
    public Light sunLight;
    public Light bounceLight;
    public Gradient sunColorGradient;

    [Header("3. Interior Objects")]
    public List<Light> interiorLights;
    public List<Renderer> fixtureMeshes;

    [Header("4. Show Settings")]
    public float minRotationSpeed = 10f;
    public float maxRotationSpeed = 500f;
    public float dimDuration = 2.0f;

    [Header("5. Blackout Control")]
    [Tooltip("0 = Pitch Black. 0.2 = Very faint reflections.")]
    [Range(0f, 1f)] public float blackoutReflection = 0.2f;

    private float currentSpeed;
    private float originalLightIntensity;
    private Color originalEmissionColor;

    // REFLECTION SETTINGS
    // private float originalReflectionIntensity;

    private Material[] FixtureMaterials;

    // void OnDisable()
    // {
    //     RenderSettings.reflectionIntensity = originalReflectionIntensity;
    // }

    void Start()
    {
        setupMode = false;
        sunLight.transform.rotation = Quaternion.Euler(startHeight, facingDirection, 0);

        FixtureMaterials = fixtureMeshes[0].materials;
        Debug.LogWarning("Captured Fixture Materials from: " + fixtureMeshes[0].name);

        if (interiorLights.Count > 0)
        {
            originalLightIntensity = interiorLights[0].intensity;
            Debug.LogWarning("Captured Original Light Intensity: " + originalLightIntensity);
        }


        if (fixtureMeshes.Count > 0 && FixtureMaterials[1].HasProperty("_EmissionColor"))
        {
            originalEmissionColor = FixtureMaterials[1].GetColor("_EmissionColor");
            Debug.LogWarning("Captured Original Emission Color: " + originalEmissionColor);
            Debug.LogWarning("Material name: " + FixtureMaterials[1].name);
        }
        else
        {
            originalEmissionColor = Color.white;
            Debug.LogWarning("Fixture meshes not set up correctly or missing _EmissionColor property. Defaulting to white.");
        }

        Debug.LogWarning("Exited Setup Mode");

        // originalReflectionIntensity = RenderSettings.reflectionIntensity;
    }

    void Update()
    {
        if (sunLight == null) return;
        if (setupMode)
        {
            sunLight.transform.rotation = Quaternion.Euler(startHeight, facingDirection, 0);
            UpdateSunColor();
        }
        else if (Application.isPlaying) UpdateSunColor();
    }

    void UpdateSunColor()
    {
        float sunHeight = Vector3.Dot(-sunLight.transform.forward, Vector3.up);

        if (sunHeight < -0.2f)
        {
            sunLight.color = Color.black;
            if (bounceLight) bounceLight.intensity = 0;
        }
        else
        {
            float gradientTime = Mathf.InverseLerp(-0.2f, 1.0f, sunHeight);
            sunLight.color = sunColorGradient.Evaluate(gradientTime);
            if (bounceLight) bounceLight.intensity = gradientTime;
        }
    }

    IEnumerator RunLightShow()
    {
        // PHASE 1: SPIN TO MIDNIGHT
        float targetAngleP1 = 270f;
        float currentAngleP1 = startHeight;
        float distToMidnight = targetAngleP1 - currentAngleP1;
        if (distToMidnight < 0) distToMidnight += 360f;
        float revsPhase1 = 15f + (distToMidnight / 360f);

        yield return StartCoroutine(FadeInterior(0.2f, blackoutReflection, Color.black, dimDuration));
        yield return StartCoroutine(SpinSun(revsPhase1, true));

        // PHASE 2: LIGHTS OUT (Fade Reflections)
        // sunLight.enabled = false;
        // if (bounceLight) bounceLight.enabled = false;
        // currentSpeed = 0;

        // This fades lights, emission, AND Reflections together


        // yield return new WaitForSeconds(1f);

        // PHASE 3: LIGHTS ON (Restore Reflections)
        // sunLight.enabled = true; // Turn back on before fading

        // yield return StartCoroutine(FadeInterior(originalLightIntensity, originalReflectionIntensity, originalEmissionColor, dimDuration));

        // PHASE 4: DECELERATE TO HOME
        // if (bounceLight) bounceLight.enabled = true;

        // float startAngleP4 = 270f;
        // float targetAngleP4 = startHeight;
        // float distToHome = targetAngleP4 - startAngleP4;
        // if (distToHome < 0) distToHome += 360f;
        // float revsPhase4 = 5f + (distToHome / 360f);

        // yield return StartCoroutine(SpinSun(revsPhase4, false));
    }

    IEnumerator SpinSun(float targetRevolutions, bool accelerating)
    {
        float totalDegreesRotated = 0f;
        float targetDegrees = 360f * targetRevolutions;

        while (totalDegreesRotated < targetDegrees)
        {
            float progress = totalDegreesRotated / targetDegrees;

            if (accelerating)
                currentSpeed = Mathf.Lerp(minRotationSpeed, maxRotationSpeed, progress);
            else
                currentSpeed = Mathf.Lerp(maxRotationSpeed, minRotationSpeed, progress);

            float rotationStep = currentSpeed * Time.deltaTime;

            if (totalDegreesRotated + rotationStep > targetDegrees)
                rotationStep = targetDegrees - totalDegreesRotated;

            sunLight.transform.Rotate(Vector3.right, rotationStep, Space.Self);
            totalDegreesRotated += rotationStep;

            yield return null;
        }
    }

    // UPDATED: Now handles Reflection Intensity
    IEnumerator FadeInterior(float targetLightIntensity, float targetReflectionIntensity, Color targetEmissionColor, float duration)
    {
        Debug.LogWarning("Entering FadeInterior: Target Light Intensity " + targetLightIntensity + ", Target Reflection Intensity " + targetReflectionIntensity + ", Target Emission Color " + targetEmissionColor + ", Duration: " + duration + "s");
        float startLightIntensity = (interiorLights.Count > 0) ? interiorLights[0].intensity : 0f;
        Color startEmissionColor = Color.black;
        if (fixtureMeshes.Count > 0) startEmissionColor = FixtureMaterials[1].GetColor("_EmissionColor");

        // Get start reflection
        // float startReflection = RenderSettings.reflectionIntensity;

        float timeElapsed = 0f;

        Debug.LogWarning($"Starting FadeInterior: From Light Intensity {startLightIntensity} to {targetLightIntensity}, " +
                         //  $"From Reflection Intensity {startReflection} to {targetReflectionIntensity}, " +
                         $"From Emission Color {startEmissionColor} to {targetEmissionColor}, Duration: {duration}s");
        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            dimDuration = timeElapsed / duration;
            // Calc Values
            float newIntensity = Mathf.Lerp(startLightIntensity, targetLightIntensity, dimDuration);
            // float newReflection = Mathf.Lerp(startReflection, targetReflectionIntensity, duration);
            Color newColor = Color.Lerp(startEmissionColor, targetEmissionColor, dimDuration);

            // Apply to Lights
            foreach (Light light in interiorLights) light.intensity = newIntensity;
            // Apply to Meshes
            FixtureMaterials[1].SetColor("_EmissionColor", newColor);
            // Apply to Environment
            // RenderSettings.reflectionIntensity = newReflection;
            yield return null;
        }
        FixtureMaterials[1].SetColor("_EmissionColor", targetEmissionColor);


    }

    // function to call from Timeline to start D&N cycle
    public void StartLightShow()
    {
        StartCoroutine(RunLightShow());
    }
}


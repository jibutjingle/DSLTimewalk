using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDilation : MonoBehaviour
{
    [Header("Oscillation Settings")]
    [Tooltip("How fast the face dilation oscillates.")]
    public float speed = 1f;

    [Tooltip("Minimum dilation value.")]
    [Range(-1f, 1f)]
    public float minDilation = 0f;

    [Tooltip("Maximum dilation value.")]
    [Range(-1f, 1f)]
    public float maxDilation = 1f;

    private TMP_Text tmpText;
    private float t;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        // t goes from 0 ? 1 ? 0 repeatedly using a sine wave
        t += Time.deltaTime * speed;
        float oscillation = (Mathf.Sin(t) + 1f) / 2f; // normalized 0–1

        // Lerp between min and max dilation
        float dilation = Mathf.Lerp(minDilation, maxDilation, oscillation);
        tmpText.fontMaterial.SetFloat(ShaderUtilities.ID_FaceDilate, dilation);
    }
}

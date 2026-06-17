using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutLights : MonoBehaviour
{
    public float fadeDuration = 2f;
    public Color fadeColor;
    private Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }
    
    public void FadeIn()
    {
        Fade(1, 0);
    }

    public void FadeOut()
    {
        Fade(0, 1);
        //Debug.Log("Fadeout called");
    }

    public void Fade(float alphaIn, float alphaOut)
    {
        StartCoroutine(FadeRoutine(alphaIn, alphaOut));
    }

    public IEnumerator FadeRoutine(float alphaIn, float alphaOut)
    {
        float timer = 0;
        while (timer <= fadeDuration)
        {
            Color newColor = fadeColor;
            newColor.a = Mathf.Lerp(alphaIn, alphaOut, timer / fadeDuration);
            rend.material.SetColor("_BaseColor", newColor);
            timer += Time.deltaTime;
            yield return null;
        }

        //make sure that the alpha channel ends on alphaOut
        Color newColor2 = fadeColor;
        newColor2.a = alphaOut;
        rend.material.SetColor("_BaseColor", newColor2);
    }
}

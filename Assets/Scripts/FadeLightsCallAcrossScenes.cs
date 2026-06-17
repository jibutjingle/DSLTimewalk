using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeLightsCallAcrossScenes : MonoBehaviour
{
    private GameObject faderScreen;
    private FadeOutLights fadeOutScript;
    // Start is called before the first frame update
    void Start()
    {
        faderScreen = GameObject.Find("Fader Screen");
        fadeOutScript = faderScreen.GetComponent<FadeOutLights>();
    }

    public void CallFadeOut()
    {
        fadeOutScript.FadeOut();
        Debug.Log("call fadeout made");
    }

    public void CallFadeIn()
    {
        fadeOutScript.FadeIn();
    }
}

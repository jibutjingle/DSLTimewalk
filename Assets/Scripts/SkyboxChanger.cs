using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public Material newSkybox; // Assign your desired skybox material in the Inspector

    [SerializeField]
    private Material newRuntimeSkyboxMaterial; //may be none if not needed
    void Start()
    {
        if (newSkybox != null)
        {
            RenderSettings.skybox = newSkybox; // Change the skybox
            //DynamicGI.UpdateEnvironment();    // Update lighting for the new skybox
        }
        else
        {
            Debug.LogWarning("No skybox material assigned!");
        }
    }

    public void RunTimeSkyboxChanger()
    {
        RenderSettings.skybox = newRuntimeSkyboxMaterial;
    }
}




using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticleEmit : MonoBehaviour
{
    private ParticleSystem SmokeParticleSystem;

    void Awake()
    {
        // Get the ParticleSystem component attached to this GameObject
        SmokeParticleSystem = GetComponent<ParticleSystem>();

        if (SmokeParticleSystem == null)
        {
            Debug.LogWarning("No ParticleSystem found on " + gameObject.name);
        }
    }

    // Call this function to stop the particle emission
    public void StopEmission()
    {
        if (SmokeParticleSystem == null) return;

        // Disable emission but keep existing particles alive until they fade
        var emission = SmokeParticleSystem.emission;
        emission.enabled = false;
    }
}

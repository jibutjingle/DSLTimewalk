using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BlinkingScript : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObjects; // The objects to blink.
    [SerializeField] private float blinkInterval = 5; // Time between blinks.
    [SerializeField] private float randomization = 2f; // Number of blinks before stopping.
    [SerializeField] private float blinkSpeed = 0.1f; // Speed of the blink animation.

    private SkinnedMeshRenderer[] renderers; // Array to hold the SkinnedMeshRenderers of the objects.
    private float blinkWeight; // weight of blend shape

    void Start()
    {
        if (gameObjects == null || gameObjects.Length == 0) return; // If no objects are assigned, exit the method.

        foreach (GameObject obj in gameObjects)
        {
            if (obj.GetComponent<SkinnedMeshRenderer>() == null) continue; // If the object does not have a SkinnedMeshRenderer, skip it.
            GetRenderers(obj); // Get the SkinnedMeshRenderers of the objects.
        }

        StartCoroutine(BlinkCoroutine()); // Start the blinking coroutine.
        // StartCoroutine(Blink()); // Start the blinking coroutine.
    }

    //added logic from animate blink to override any animation clip that might have blink blend shapes
    void LateUpdate()
    {
        if (renderers == null) return;

        foreach (SkinnedMeshRenderer renderer in renderers)
        {
            var blendShapeCount = renderer.sharedMesh.blendShapeCount; // Get the number of blend shapes in the mesh.
            if (blendShapeCount == 0) continue; // If there are no blend shapes, skip this renderer.

            var tgtIndices = GetBlinkIndices(renderer, blendShapeCount); // Get the indices of the blend shapes related to blinking.
            foreach (int num in tgtIndices)
            {
                if (num == 0) continue; // Skip indices that are not valid blend shapes.
                renderer.SetBlendShapeWeight(num, blinkWeight); // Set the weight of the blend shape to create the blinking effect.
            }
        }
    }

    private IEnumerator BlinkCoroutine()
    {
        while (true) // Loop indefinitely to keep the blinking effect ongoing.
        {
            yield return new WaitForSeconds(blinkInterval + UnityEngine.Random.Range(-randomization, randomization)); // Wait for the specified interval with some randomization.

            yield return StartCoroutine(AnimateBlink(0f, 100f)); // Start the blink animation coroutine and wait for it to finish before proceeding to the next blink.

            yield return StartCoroutine(AnimateBlink(100f, 0f)); // Open the eyes again by animating the blend shapes back to their original state.
        }
    }

    private IEnumerator AnimateBlink(float startWeight, float endWeight)
    {
        float elapsedTime = 0f; // Initialize the elapsed time for the animation.
        while (elapsedTime < blinkSpeed) // Loop until the elapsed time is less than the specified blink speed.
        {
            float weight = Mathf.Lerp(startWeight, endWeight, elapsedTime / blinkSpeed); // Calculate the current weight of the blend shape based on the elapsed time.
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                var blendShapeCount = renderer.sharedMesh.blendShapeCount; // Get the number of blend shapes in the mesh.
                if (blendShapeCount == 0) continue; // If there are no blend shapes, skip this renderer.

                var tgtIndices = GetBlinkIndices(renderer, blendShapeCount); // Get the indices of the blend shapes related to blinking.
                foreach (int num in tgtIndices)
                {
                    if (num == 0) continue; // Skip indices that are not valid blend shapes.
                    blinkWeight = weight; // set weight to blinkWeight to use in LateUpdate to override any timeline animation!
                }
            }

            elapsedTime += Time.deltaTime; // Increment the elapsed time by the time since the last frame.
            yield return null; // Wait for the next frame before continuing the animation loop.
        }
    }

    /*
    private IEnumerator Blink()
    {
        for (float value = blinkInterval; value >= 0; value -= 0.1f)
        {
            Debug.Log($"Blinking in {value} seconds..."); // Log the time until the next blink for debugging purposes.
            foreach (SkinnedMeshRenderer renderer in renderers)
            {
                var blendShapeCount = renderer.sharedMesh.blendShapeCount; // Get the number of blend shapes in the mesh.
                if (blendShapeCount == 0)
                {
                    Debug.Log($"Renderer {renderer.name} has no blendshapes."); // Log the absence of blend shapes for debugging purposes.
                }
                else
                {
                    var tgtIndices = GetBlinkIndices(renderer, blendShapeCount);// Get the indices of the blend shapes related to blinking.
                    foreach (int num in tgtIndices)
                    {
                        if (num == 0) continue; // Skip indices that are not valid blend shapes.
                        Debug.Log($"Blend shape {num}: {renderer.sharedMesh.GetBlendShapeName(num)}"); // Log the name of each blend shape for debugging purposes.
                        renderer.SetBlendShapeWeight(num, 100); // Set the weight of the blend shape to 100 to simulate a blink.
                    }
                }
            }
            
            float rand = UnityEngine.Random.Range(-randomization, randomization); // Reset the timer with some randomization.
            yield return new WaitForSeconds(rand); // Wait for a short time before the next check.
        }
    }*/

    private void GetRenderers(GameObject obj)
    {
        renderers = new SkinnedMeshRenderer[gameObjects.Length];
        for (int i = 0; i < gameObjects.Length; i++)
        {
            renderers[i] = gameObjects[i].GetComponent<SkinnedMeshRenderer>();
        }
    }

    private int[] GetBlinkIndices(SkinnedMeshRenderer renderer, int blendShapeCount)
    {
        int[] blendShapeIndices = new int[blendShapeCount]; // Create an array to hold the indices of the blend shapes.
        for (int i = 0; i < blendShapeCount; i++)
        {
            string name = renderer.sharedMesh.GetBlendShapeName(i); // Get the name of each blend shape (for debugging purposes).
            bool isBlinkShape = name.Contains("Blink", StringComparison.OrdinalIgnoreCase); // Check if the blend shape name contains "Blink" (case-insensitive).
            if (!isBlinkShape) continue; // If the blend shape is not related to blinking, skip it.

            blendShapeIndices[i] = i; // Store the index of the blend shape in the array.
        }

        return blendShapeIndices; // Return the array of blend shape indices.

    }
}

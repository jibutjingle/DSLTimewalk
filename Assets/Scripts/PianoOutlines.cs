using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoOutlines : MonoBehaviour
{
    [SerializeField] private List<GameObject> pianos = new List<GameObject>();
    [SerializeField] private GameObject arrowIndicator;

    /// <summary>
    /// Toggle the outline layer for the piano that is currently being played. Certain scenarios will trigger
    /// different layer toggling behavior
    /// </summary>
    /// <param name="pianoNum"></param>
    public void TogglePianoOutlines(int pianoNum)
    {
        LayerMask defaultLayer = LayerMask.NameToLayer("KeepOriginal");
        LayerMask outlineLayer = LayerMask.NameToLayer("Outline");

        foreach (GameObject piano in pianos)
        {
            // Last piano is played correctly -> return to default layer
            if (pianoNum == 6)
            {
                pianos[5].layer = defaultLayer;
                // Debug.Log("Piano 5 outline disabled");
                // return;
            }
            // Players took too long to play a piano -> reset all pianos to default and enable outline for first piano
            else if (pianoNum == 7)
            {
                foreach (GameObject p in pianos)
                {
                    p.layer = defaultLayer;
                    // Debug.Log("Piano outline disabled for all pianos");
                }
                pianos[0].layer = outlineLayer;
                // Debug.Log("Piano 0 outline enabled");
            }
            // Piano is played correctly -> enable outline for current piano and disable outline for previouse
            else if (piano == pianos[pianoNum])
            {
                pianos[pianoNum].layer = outlineLayer;
                pianos[pianoNum - 1].layer = defaultLayer;
                // Debug.Log("Piano " + (pianoNum - 1) + " outline disabled");
                // Debug.Log("Piano " + pianoNum + " outline enabled");
            }
        }
    }

    /// <summary>
    /// Toggle arrow indicator for the current piano being played. Certain scenarios will trigger different
    /// behaviors as the TogglePianoOutlines method.
    /// </summary>
    /// <param name="pianoNum"></param>
    /// <param name="Arrow"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void ToggleArrowIndicator(int pianoNum, Vector3 position, Quaternion rotation)
    {
        if (arrowIndicator == null)
        {
            Debug.LogError("Arrow GameObject is not assigned.");
            return;
        }

        foreach (GameObject piano in pianos)
        {
            // Last piano is played correctly -> disable arrow indicator
            // 6 is just a magic number 
            if (pianoNum == 6)
            {
                arrowIndicator.SetActive(false);
            }
            // Players took too long to play a piano -> disable arrow indicator, set position and rotation to first piano, set active
            // 7 is just a magic number
            else if (pianoNum == 7)
            {
                Debug.Log("Arrow7: " + arrowIndicator.name + " set to position: " + position + " and rotation: " + rotation);
                arrowIndicator.SetActive(false);
                arrowIndicator.transform.position = position;
                arrowIndicator.transform.rotation = rotation;
                arrowIndicator.SetActive(true);
                // Debug.Log("Piano 0 outline enabled");
            }
            // Piano is played correctly -> disable arrow indicator, set position and rotation to current piano, set active
            // This works the same as pianoNum == 7 case, but I kept the logic separate just in case for the future
            else if (piano == pianos[pianoNum])
            {
                Debug.Log("Arrow: " + arrowIndicator.name + " set to position: " + position + " and rotation: " + rotation);
                arrowIndicator.SetActive(false);
                arrowIndicator.transform.position = position;
                arrowIndicator.transform.rotation = rotation;
                arrowIndicator.SetActive(true);
                // Debug.Log("Piano " + (pianoNum - 1) + " outline disabled");
                // Debug.Log("Piano " + pianoNum + " outline enabled");
            }
        }
    }
}
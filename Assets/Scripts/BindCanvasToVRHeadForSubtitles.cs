using System.Collections;
using System.Collections.Generic;
using TMPro;    
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BindCanvasToVRHeadForSubtitles : MonoBehaviour
{
    public PlayableDirector director; // Assign in Inspector
    [HideInInspector]
    public TextMeshProUGUI XRCameraTMP;   // The GameObject you want to bind
    [HideInInspector]
    public string trackName = "Text Track"; // Name of the track in the Timeline

    void Start()
    {
        //Find the Text GO on the XR Camera Rig Canvas
        Camera mainCamera = Camera.main;
        

        if (director == null)
        {
            Debug.LogError("PlayableDirector or XR Camera TargetObject not assigned.");
            return;
        }
        else
        {
            XRCameraTMP = mainCamera.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        }

        // Get the TimelineAsset from the director
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        if (timeline == null)
        {
            Debug.LogError("PlayableDirector does not have a TimelineAsset.");
            return;
        }


        // Find the track by name
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                // Bind the GameObject to the track
                director.SetGenericBinding(track, XRCameraTMP);
                Debug.Log($"Bound {XRCameraTMP.name} to track {trackName}");
                break;
            }
        }

    }
}

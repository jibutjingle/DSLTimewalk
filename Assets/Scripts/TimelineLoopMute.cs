using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineLoopMute : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private string trackName;
    private TimelineAsset timeline;
    void Start()
    {
        timeline = director.playableAsset as TimelineAsset;
    }
    public void MuteTrack()
    {
        foreach (var track in timeline.GetOutputTracks())
        {
            // Debug.Log("Track: " + track.name);
            if (track.name == trackName)
            {
                track.muted = true;
                break;
            }
        }

        director.RebuildGraph();
        director.time = 0f;
    }

    void OnDestroy()
    {
        foreach (var track in timeline.GetOutputTracks())
        {
            // Debug.Log("Track: " + track.name);
            if (track.name == trackName)
            {
                track.muted = false;
                break;
            }
        }
    }
}

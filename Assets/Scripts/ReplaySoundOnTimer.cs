using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaySoundOnTimer : MonoBehaviour
{
    public AudioClip soundClip;       // Assign your sound clip in the Inspector
    public float interval = 17f;       // Time in seconds between plays (note: includes length of clip)
    private AudioSource audioSource;
    private float timer;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = soundClip;
        timer = interval;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            audioSource.Play();
            timer = interval; // Reset the timer
        }
    }
}

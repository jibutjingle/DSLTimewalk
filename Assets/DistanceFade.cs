using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DistanceFade : MonoBehaviour
{
    [Header("Reference Game Objects")]
    [SerializeField] private List<GameObject> proximityObjects;

    [Header("Search Parameters")]
    [SerializeField] private float range = 5f;

    private Camera activeCam;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // If there is no camera, no objects in the list or the object is too far away to be tracked return.
        if(!activeCam) return;

        if(proximityObjects.Count<1) return;

        if(!Physics.Raycast(activeCam.transform.position, activeCam.transform.forward, out var hit, range))
            return;

        TrackDistance(hit);
    }

    private void TrackDistance(RaycastHit hit)
    {
        Debug.Log("The {hit.transform.name} is {hit.distance} units away.");
    }
}

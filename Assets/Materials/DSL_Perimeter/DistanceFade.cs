using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DistanceFade : MonoBehaviour
{
    [Header("Reference Objects")]
    [SerializeField] private Material instanceMaterial;
    [SerializeField] private GameObject targetPlanks;


    [Header("Search Parameters")]
    [SerializeField] private float range = 5f;
    [SerializeField] private int targetLayer;

    private Camera activeCam;
    private Vector3 directionToTarget;

    public List<GameObject> closestObjects = new();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        // If there is no camera, no objects in the list or the object is too far away to be tracked return.
        activeCam = Camera.main;
        if(!activeCam) return;

        if(!targetPlanks) return;
        SetLayerRecursive(targetPlanks,targetLayer);
        closestObjects = CheckDistance(targetPlanks);

        
        
        if(!Physics.Raycast(activeCam.transform.position, activeCam.transform.forward, out var hit, range, targetLayer))
            return;

        TrackDistance(hit);

    }

    private void SetLayerRecursive(GameObject obj, int layer)
    {
        // Set the object and it's children layer to the supplied int.
        obj.layer = layer;
        foreach(Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, layer);
        }
    }

    private List<GameObject> CheckDistance(GameObject gameObjectToCheck)
    {
        List<GameObject> foundObjects = new();

        return foundObjects;
    }

    private void TrackDistance(RaycastHit hit)
    {
         
        Debug.Log($"The {this.transform.name} is {hit.distance} units away.");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    
    [Header("Rotation Speed (degrees per second)")]
    public float rotationSpeed = 90f;

    // Update is called once per frame
    void Update()
    {
        // Rotate around the object's local Y axis
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ResetArea : MonoBehaviour
{
    [SerializeField]
    private Transform resetPosition;
    private void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<Rigidbody>() != null)
        {
            Debug.Log("This object reset position:" + other.name);
            other.transform.localPosition = resetPosition.localPosition;
        }
    }
}

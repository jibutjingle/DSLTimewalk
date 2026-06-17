using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public float deadTime = 1.0f;
    private bool _deadTimeActive = false;
    public UnityEvent onPressed, onReleased;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Button" && !_deadTimeActive) 
        {
            onPressed?.Invoke();
            Debug.Log("I have been pressed");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Button" && !_deadTimeActive) 
        {
            onReleased?.Invoke();
            Debug.Log("I have been released");
            StartCoroutine(WaitForDeadTIme());
        
        }
    }
    IEnumerator WaitForDeadTIme() 
    {
        _deadTimeActive = true;
        yield return new WaitForSeconds(deadTime);
        _deadTimeActive = false;
    }
}

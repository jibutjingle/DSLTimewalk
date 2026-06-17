using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterial : MonoBehaviour
{
    [SerializeField]
    private Material originalMaterial;
    [SerializeField]
    private Material newMaterial;
    [SerializeField]
    private List<GameObject> gameObjectsToChange;

    public void ChangeToNewMaterial()
    {
        if (newMaterial != null)
        {
            foreach (GameObject go in gameObjectsToChange)
            {
                go.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }

    void OnDisable()
    {
        if (originalMaterial != null)
        {
            foreach (GameObject go in gameObjectsToChange)
            {
                go.GetComponent<Renderer>().material = originalMaterial;
            }
        }
    }
}

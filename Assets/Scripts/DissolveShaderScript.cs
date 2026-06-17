using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolveShaderScript : MonoBehaviour
{
    public MeshRenderer objectMesh;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.01f;
    public float refreshRate = 0.025f;
    private Material[] meshMaterials;
    [SerializeField]
    private GameObject gamePlay;
    private bool dissolve = true;
   

    void Start()
    {
        //dissolveInstruments = gamePlay.GetComponent<BeginTutorial>().dissolveInstruments;
        if (objectMesh != null)
        {
            meshMaterials = objectMesh.materials;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (gamePlay.GetComponent<BeginTutorial>().dissolveInstruments && dissolve)
        {
            VFXGraph.gameObject.SetActive(true); //enable the particle system
            StartCoroutine(DissolveCo());
            dissolve = false; // only needs to happen once
        }
        else return;
        

    }

    IEnumerator DissolveCo()
    {
        if (VFXGraph != null)
            VFXGraph.Play(); //particle effect
        if (meshMaterials.Length > 0)
        {
            float counter = 0;
            while (meshMaterials[0].GetFloat("_DissolveAmount") < 1)//shader dissolve
            {
                counter += dissolveRate;
                for (int i = 0; i < meshMaterials.Length; i++) 
                {
                    meshMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}

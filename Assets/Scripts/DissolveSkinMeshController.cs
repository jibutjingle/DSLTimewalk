using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class DissolveSkinMeshController : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMesh;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.005f;
    public float refreshRate = 0.025f;
    private Material[] skinnedMaterials;
    // Start is called before the first frame update
    void Start()
    {
        if(skinnedMesh != null)
        {
            skinnedMaterials = skinnedMesh.materials;
        }
    }
    public void DissolvePerson()
    {
        StartCoroutine(DissolveCo());
    }

    IEnumerator DissolveCo()
    {
        if (VFXGraph != null)
            VFXGraph.Play(); //particle effect
        if (skinnedMaterials.Length > 0)
        {
            float counter = skinnedMaterials[0].GetFloat("_DissolveAmount");
            while (counter < 1f)//shader dissolve
            {
                counter += dissolveRate *Time.deltaTime*60f;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                yield return null;
            }
        }
    }

    public void UnDissolvePerson()
    {
        
        StartCoroutine(UnDissolveCo());
    }

    IEnumerator UnDissolveCo()
    {
        if (VFXGraph != null)
            VFXGraph.Play(); //particle effect
        if (skinnedMaterials.Length > 0)
        {
            float counter = skinnedMaterials[0].GetFloat("_DissolveAmount");
            while (counter > 0f)//shader dissolve
            {
                //counter -=dissolveRate
                counter -= dissolveRate * Time.deltaTime*60f;
                for (int i = 0; i < skinnedMaterials.Length; i++)
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", counter);
                }
                //yield return new WaitForSeconds(refreshRate);
                yield return null;
            }
        }
    }
}

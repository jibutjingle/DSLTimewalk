using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepMaterialsForUnDissolve : MonoBehaviour
{
    public SkinnedMeshRenderer[] skinnedMeshList;
    private Material[] skinnedMaterials;

    // Start is called before the first frame update
    void Start()
    {
        
        if (skinnedMeshList != null)
        {
            foreach (var skinnedMesh in skinnedMeshList)
            {
                skinnedMaterials = skinnedMesh.materials;
            }
            
        }
        
        if (skinnedMaterials.Length > 0)
        {
            for (int i = 0; i < skinnedMaterials.Length; i++)
            {
                if (skinnedMaterials[i].HasProperty("_DissolveAmount"))
                {
                    skinnedMaterials[i].SetFloat("_DissolveAmount", 1f);
                }           
                
            }
        }

    }

}

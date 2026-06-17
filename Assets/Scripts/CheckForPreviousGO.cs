using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckForPreviousGO : MonoBehaviour
{
    [SerializeField]
    private string previousSceneName = "Scene0_tutorial";

    public List<string> previousGOsToCheck;

    // Start is called before the first frame update
    void Start()
    {
        //destroy previous scene's gamemanager
        Scene previousScene = SceneManager.GetSceneByName(previousSceneName);
        if (previousScene.isLoaded)
        {
            foreach (var name in previousGOsToCheck) 
            {
                GameObject targetObject = GameObject.Find(name);
                if (targetObject != null)
                {
                    Destroy(targetObject);
                }
            }
            
            
        }
    }
}

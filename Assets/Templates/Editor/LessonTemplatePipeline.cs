using UnityEngine.SceneManagement;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEditor.SearchService;

    public class LessonTemplatePipeline : ISceneTemplatePipeline
    {
        public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset)
        {
            return true;
        }

        public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName)
        {

        }

        public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, UnityEngine.SceneManagement.Scene scene, bool isAdditive, string sceneName)
        {
            EditorSceneSaveListener.setupNextSavedScene = true;
            UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene);
        }
    }

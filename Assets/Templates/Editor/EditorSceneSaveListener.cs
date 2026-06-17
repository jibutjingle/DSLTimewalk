#if Dreamscape
using Dreamscape.Config;
#endif
using UnityEditor;
using UnityEngine;

[UnityEditor.InitializeOnLoad]
static class EditorSceneSaveListener
{
    public static bool setupNextSavedScene = false;


    static EditorSceneSaveListener()
    {
        UnityEditor.SceneManagement.EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    static void OnSceneSaved(UnityEngine.SceneManagement.Scene scene)
    {
#if Dreamscape
        if (setupNextSavedScene)
        {

            setupNextSavedScene = false;

            //rvsdk scene list 
            var rvsdkConfig = ExperienceConfigSO.GetOrCreateConfig();

            //check scene name exist
            foreach(var s in rvsdkConfig.StartScenes)
            {
                if(s.SceneName == scene.name)
                {
                    Debug.LogError($"A Walkaround scene with name {scene.name} already exist");
                    return;
                }
            }

            SerializableScene serializableScene = new SerializableScene();
            serializableScene.SceneName = scene.name;
            rvsdkConfig.StartScenes.Add(serializableScene);
            ExperienceConfigSO.SaveConfig(rvsdkConfig);


            // Add new scene to build settings
            var original = EditorBuildSettings.scenes;
            bool alreadyAddedToBuildSettings = false;
            foreach(var s in original)
            {
                if (s.path == scene.path)
                {
                    alreadyAddedToBuildSettings = true;
                    break;
                }
            }

            if (!alreadyAddedToBuildSettings)
            {
                var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                System.Array.Copy(original, newSettings, original.Length);
                var sceneToAdd = new EditorBuildSettingsScene(scene.path, true);
                newSettings[newSettings.Length - 1] = sceneToAdd;
                EditorBuildSettings.scenes = newSettings;
            }
        }
#endif
    }
}


#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GrabSoundPlayer))]
public class GrabSoundPlayerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var grabSoundPlayer = (GrabSoundPlayer)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Auto-Assign Components From This GameObject"))
        {
            Undo.RecordObject(grabSoundPlayer, "Auto-Assign GrabSoundPlayer Components");
            grabSoundPlayer.AutoAssignComponents();
            EditorUtility.SetDirty(grabSoundPlayer);
        }
    }
}
#endif

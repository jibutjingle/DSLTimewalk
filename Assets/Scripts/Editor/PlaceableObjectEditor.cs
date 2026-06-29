#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// inspector button for PlaceableObject + optional DreamscapeGrabbable auto-assign
[CustomEditor(typeof(PlaceableObject))]
public class PlaceableObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var placeable = (PlaceableObject)target;

        EditorGUILayout.Space();
        if (GUILayout.Button("Auto-Assign Components From This GameObject"))
        {
            Undo.RecordObject(placeable, "Auto-Assign PlaceableObject Components");
            placeable.AutoAssignComponents();

            DreamscapeGrabbable grabbable = placeable.GetComponent<DreamscapeGrabbable>();
            if (grabbable != null)
            {
                Undo.RecordObject(grabbable, "Auto-Assign DreamscapeGrabbable Components");
                // also wires stateSync on the grabbable when both live on the piece
                grabbable.AutoAssignComponents();
                EditorUtility.SetDirty(grabbable);
            }

            EditorUtility.SetDirty(placeable);
        }
    }
}
#endif

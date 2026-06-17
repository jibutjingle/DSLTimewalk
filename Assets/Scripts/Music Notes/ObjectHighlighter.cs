using UnityEngine;

public class ObjectHighlighter : MonoBehaviour
{
    [Header("Highlight Colors")]
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private Renderer mr;

    void Awake()
    {
        mr = GetComponent<Renderer>();
    }

    /// <summary>
    /// Updates the highlight color depending on correctness.
    /// </summary>
    /// <param name="isCorrect">Whether the highlight should be correct (green) or wrong (red)</param>
    public void UpdateHighlightColor(bool isCorrect)
    {
        if (mr == null) return;

        Color targetColor = isCorrect ? correctColor : wrongColor;

#if UNITY_EDITOR
        // Check if the object is a prefab asset (not in scene)
        if (!mr.gameObject.scene.IsValid())
        {
            // Change sharedMaterial if it’s a prefab
            mr.sharedMaterial.color = targetColor;
        }
        else
        {
            // Change instance material if it’s in the scene
            mr.material.color = targetColor;
        }
#else
        // In builds, only use instance material
        mr.material.color = targetColor;
#endif
    }
}

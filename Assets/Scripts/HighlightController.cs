using UnityEngine;
using UnityEngine.UI;

public class HighlightController : MonoBehaviour
{
    [Header("Snap Point Reference")]
    public Transform snapPoint;       // The snap point this highlight belongs to

    [Header("Correct Data")]
    public GameObject correctObject;  // The correct object for this snap point
    public AudioClip correctAudio;    // The correct audio for this snap point

    [Header("Highlight Colors")]
    public Color defaultColor = Color.white;
    public Color correctColor = Color.green;
    public Color wrongColor = Color.red;

    private SpriteRenderer spriteRenderer;
    private MeshRenderer meshRenderer;
    private Image uiImage;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        uiImage = GetComponent<Image>();

        ResetColor();
    }

    /// <summary>
    /// Checks if the snapped object at this highlight's snapPoint is correct.
    /// </summary>
    public void CheckSnappedObject(GameObject snappedObj, Transform snappedTo)
    {
        if (snappedObj == null || snappedTo != snapPoint)
        {
            ResetColor();
            return;
        }

        // ✅ Compare object name properly
        bool objectMatch = false;
        if (correctObject != null)
            objectMatch = snappedObj.name.Replace("(Clone)", "").Trim() == correctObject.name;

        // ✅ Compare audio
        bool audioMatch = false;
        AudioSource audioSource = snappedObj.GetComponent<AudioSource>();
        if (audioSource != null && correctAudio != null)
            audioMatch = audioSource.clip == correctAudio;

        if (objectMatch && audioMatch)
        {
            Debug.Log($"[HighlightController] ✅ Correct object + audio at {snapPoint.name}");
            SetColor(correctColor);
        }
        else
        {
            Debug.Log($"[HighlightController] ❌ Wrong object or audio at {snapPoint.name}");
            SetColor(wrongColor);
        }
    }

    public void ResetColor()
    {
        SetColor(defaultColor);
    }

    private void SetColor(Color c)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = c;
            return;
        }

        if (meshRenderer != null && meshRenderer.material != null)
        {
            // ✅ Use material property block (doesn’t clone materials)
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(block);
            block.SetColor("_Color", c);
            meshRenderer.SetPropertyBlock(block);
            return;
        }

        if (uiImage != null)
        {
            uiImage.color = c;
            return;
        }

        Debug.LogWarning($"[HighlightController] No Renderer found on {gameObject.name}");
    }
}

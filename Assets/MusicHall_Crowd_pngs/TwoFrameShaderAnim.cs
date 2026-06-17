using UnityEngine;

public class TwoFrameUnlitTextureSwap : MonoBehaviour
{
    public Renderer targetRenderer;
    public Texture textureA;
    public Texture textureB;
    public float switchSpeed = 2f;

    private MaterialPropertyBlock block;
    private int baseMapID;
    private bool showingA = true;
    private float timer = 0f;

    void Awake()
    {
        block = new MaterialPropertyBlock();
        baseMapID = Shader.PropertyToID("_BaseMap");
    }

    void OnEnable()
    {
        // RANDOM SPEED (IMPORTANT)
        float randomOffset = Random.Range(-0.5f, 0.5f);
        switchSpeed = Mathf.Max(0.1f, switchSpeed + randomOffset);

        // RANDOM START TIME
        timer = Random.Range(0f, 1f / switchSpeed);

        // RANDOM START FRAME
        showingA = Random.value > 0.5f;

        ApplyTexture(showingA ? textureA : textureB);
    }

    void Update()
    {
        if (targetRenderer == null || textureA == null || textureB == null)
            return;

        timer += Time.deltaTime;

        if (timer >= 1f / switchSpeed)
        {
            timer = 0f;
            showingA = !showingA;
            ApplyTexture(showingA ? textureA : textureB);
        }
    }

    public void ResetToFirstFrame()
    {
        showingA = true;
        timer = 0f;
        ApplyTexture(textureA);
    }

    private void ApplyTexture(Texture tex)
    {
        if (targetRenderer == null || tex == null)
            return;

        targetRenderer.GetPropertyBlock(block);
        block.SetTexture(baseMapID, tex);
        targetRenderer.SetPropertyBlock(block);
    }
}

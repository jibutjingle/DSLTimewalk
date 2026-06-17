using UnityEngine;

public class AudienceCardsController : MonoBehaviour
{
    private TwoFrameUnlitTextureSwap[] audienceAnimations;

    void Awake()
    {
        audienceAnimations = GetComponentsInChildren<TwoFrameUnlitTextureSwap>(true);

        foreach (var anim in audienceAnimations)
        {
            if (anim != null)
                anim.enabled = false;
        }
    }

    public void StartAudienceAnimation()
    {
        foreach (var anim in audienceAnimations)
        {
            if (anim != null)
                anim.enabled = true;
        }
    }

   public void StopAudienceAnimation()
{
    foreach (var anim in audienceAnimations)
    {
        if (anim != null)
        {
            anim.ResetToFirstFrame();
            anim.enabled = false;
        }
    }
} }
   
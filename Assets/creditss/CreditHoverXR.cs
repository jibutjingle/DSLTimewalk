using UnityEngine;
// using UnityEngine.XR.Interaction.Toolkit;
// using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class CreditHoverXR : MonoBehaviour
{
    //     public XRSimpleInteractable interactable;
    //     public LocalCreditHover creditHover;
    //     public LocalCreditManager creditManager;

    //     private void Awake()
    //     {
    //         if (interactable == null)
    //             interactable = GetComponent<XRSimpleInteractable>();

    //         if (creditHover == null)
    //             creditHover = GetComponent<LocalCreditHover>();

    //         if (creditManager == null)
    //             creditManager = FindFirstObjectByType<LocalCreditManager>();
    //     }

    //     private void OnEnable()
    //     {
    //         if (interactable != null)
    //         {
    //             interactable.hoverEntered.AddListener(OnHoverEnter);
    //             interactable.hoverExited.AddListener(OnHoverExit);
    //         }
    //     }

    //     private void OnDisable()
    //     {
    //         if (interactable != null)
    //         {
    //             interactable.hoverEntered.RemoveListener(OnHoverEnter);
    //             interactable.hoverExited.RemoveListener(OnHoverExit);
    //         }
    //     }

    //     private void OnHoverEnter(HoverEnterEventArgs args)
    //     {
    //         if (creditManager != null && creditHover != null)
    //             creditManager.SelectCredit(creditHover);
    //     }

    //     private void OnHoverExit(HoverExitEventArgs args)
    //     {
    //         if (creditManager != null)
    //             creditManager.ClearSelection();
    //     }
}
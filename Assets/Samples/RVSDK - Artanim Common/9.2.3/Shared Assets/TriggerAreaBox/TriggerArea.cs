using RvSdk.Avatar;
using RvSdk.Component;
using RvSdk.Controller;
using RvSdk.Internal.Utils;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RvSdk.Samples
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(AvatarGenericTrigger), typeof(NetworkSyncedInteger))]
    public class TriggerArea : MonoBehaviour
    {
        [Header("Label")]
        [SerializeField] string Label;
        [SerializeField] TextMeshProUGUI TextLabel;
        [SerializeField] TextMeshProUGUI TextAvatarNumber;

        [Header("Background")]
        [SerializeField] Image ImageBackground;
        [SerializeField] Color ColorInactive;
        [SerializeField] Color ColorActive;

        [Header("Cabin")]
        [SerializeField] bool ShowCabin = true;
        [SerializeField] GameObject CabinTransform;

        [Header("Events")]
        [SerializeField] AvatarEvent OnActivated;
        [SerializeField] AvatarEvent OnDeactivated;

        private AvatarGenericTrigger AvatarGenericTrigger => GetComponent<AvatarGenericTrigger>();
        private NetworkSyncedInteger NetworkSyncedInteger => GetComponent<NetworkSyncedInteger>();
        private int PreviousValidAvatarCount = 0;

        private bool IsValidated = false;

        private void Update()
        {
            if (TextLabel) TextLabel.text = Label;
            if (ImageBackground) ImageBackground.color = IsValidated ? ColorActive : ColorInactive;
            if (CabinTransform) CabinTransform.SetActive(ShowCabin);

            // Only the server is modifies the ValidAvatarCount so we use a NetworkSyncedInteger to sync the value
            if (PreviousValidAvatarCount != AvatarGenericTrigger.ValidAvatarCount)
            {
                NetworkSyncedInteger.Value = AvatarGenericTrigger.ValidAvatarCount;
                PreviousValidAvatarCount = AvatarGenericTrigger.ValidAvatarCount;
            }
        }

        public void UpdateAvatarCount(int count)
        {
            if (TextAvatarNumber && GameController.Instance != null)
            {
                var countNeeded = Math.Min(AvatarGenericTrigger.MinAvatarCount, GameController.Instance.RuntimePlayers.Count);
                if (AvatarGenericTrigger.ActivateOnAllAvatars)
                    countNeeded = GameController.Instance.RuntimePlayers.Count;

                TextAvatarNumber.text = $"Avatar Required:\n{count} / {countNeeded}";
            }
        }

        public void OnValidated(AvatarController avatar)
        {
            IsValidated = true;
            if(ImageBackground) ImageBackground.color = ColorActive;

            OnActivated.Invoke(avatar);
        }

        public void OnInvalidated(AvatarController avatar)
        {
            IsValidated = false;
            if (ImageBackground) ImageBackground.color = ColorInactive;

            OnDeactivated.Invoke(avatar);
        }
    }
}
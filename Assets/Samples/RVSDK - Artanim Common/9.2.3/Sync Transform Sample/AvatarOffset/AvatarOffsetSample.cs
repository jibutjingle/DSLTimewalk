using RvSdk.Avatar;
using RvSdk.Controller;
using UnityEngine;

namespace RvSdk.Samples
{
    [RequireComponent(typeof(AvatarOffset))]
    [RequireComponent(typeof(Animator))]
    public class AvatarOffsetSample : MonoBehaviour
    {
        private const string TRIGGER_PLAY = "Play";
        private const string TRIGGER_STOP = "Stop";

        private AvatarOffset AvatarOffset => GetComponent<AvatarOffset>();
        private Animator Animator => GetComponent<Animator>();

        public void Start()
        {
            Animator.enabled = NetworkController.IsServer;
        }

        public void OnRegisterOffset(AvatarController avatar)
        {
            if (NetworkController.IsServer)
            {
                AvatarOffset.RegisterPlayer(avatar.PlayerId, resetToZero: true);

                Animator.ResetTrigger(TRIGGER_STOP);
                Animator.SetTrigger(TRIGGER_PLAY);
            }
        }

        public void OnUnregisterOffset(AvatarController avatar)
        {
            if (NetworkController.IsServer)
            {
                AvatarOffset.UnregisterPlayer();

                Animator.SetTrigger(TRIGGER_STOP);
            }
        }
    }
}
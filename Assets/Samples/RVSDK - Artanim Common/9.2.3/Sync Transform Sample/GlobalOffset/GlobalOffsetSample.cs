using RvSdk.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RvSdk.Samples
{
    [RequireComponent(typeof(GlobalOffset))]
    [RequireComponent(typeof(Animator))]
    public class GlobalOffsetSample : MonoBehaviour
    {
        private const string TRIGGER_PLAY = "Play";
        private const string TRIGGER_STOP = "Stop";

        private GlobalOffset GlobalOffset => GetComponent<GlobalOffset>();
        private Animator Animator => GetComponent<Animator>();

        public void Start()
        {
            Animator.enabled = NetworkController.IsServer;
        }

        public void OnRegisterOffset()
        {
            if (NetworkController.IsServer)
            {
                GlobalOffset.Register();
                Animator.ResetTrigger(TRIGGER_STOP);
                Animator.SetTrigger(TRIGGER_PLAY);
            }
        }

        public void OnUnregisterOffset()
        {
            if (NetworkController.IsServer)
            {
                GlobalOffset.UnregisterGlobalOffset();
                Animator.SetTrigger(TRIGGER_STOP);
            }
        }
    }
}
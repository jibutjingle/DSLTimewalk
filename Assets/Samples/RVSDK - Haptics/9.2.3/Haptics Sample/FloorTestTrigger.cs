using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RvSdk.Haptics.Samples
{
    [RequireComponent(typeof(HapticAudioEffect))]
    public class FloorTestTrigger : MonoBehaviour
    {
        [SerializeField] Image ImageButtonOn;
        [SerializeField] Image ImageButtonOff;

        [SerializeField] Color ColorOn;
        [SerializeField] Color ColorOff;

        private HapticAudioEffect HapticAudioEffect => GetComponent<HapticAudioEffect>();

        private bool EnableFloor
        {
            set
            {
                HapticAudioEffect.enabled = value;
                if (ImageButtonOn) ImageButtonOn.color = value ? ColorOn : ColorOff;
                if (ImageButtonOff) ImageButtonOff.color = value ? ColorOff : ColorOn;
            }
        }

        private void Start() { EnableFloor = false; }

        public void OnFloorOn() { EnableFloor = true; }

        public void OnFloorOff() { EnableFloor = false; }
    }
}
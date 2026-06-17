using UnityEngine;

namespace RvSdk.Haptics.Samples
{
    [RequireComponent(typeof(DmxDeviceTarget))]
    public class DmxTestTrigger : MonoBehaviour
    {
        private DmxDeviceTarget DmxDeviceTarget => GetComponent<DmxDeviceTarget>();

        private bool EnableFans
        {
            set
            {
                DmxDeviceTarget.enabled = value;
            }
        }

        private void Start() { EnableFans = false; }

        public void OnPlayerEnter() { EnableFans = true; }

        public void OnPlayerLeave() { EnableFans = false; }
    }
}
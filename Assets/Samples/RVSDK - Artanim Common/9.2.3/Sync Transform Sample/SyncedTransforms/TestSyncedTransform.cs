using RvSdk.Controller;
using RvSdk.Module;
using UnityEngine;

namespace RvSdk.Samples
{
    public class TestSyncedTransform : MonoBehaviour
    {
        public NetworkSyncedTransform NetworkSyncedTransform;

        private Vector3 RotationSpeed;
        private Vector3 TranslationSpeed = new Vector3(0.2f, 0, 0.2f);
        private float ScaleSpeed = 0.2f;
        private Transform TargetTransform;
        private float InitialScale = 1f;

        void Awake()
        {
            RotationSpeed = 60f * new Vector3(Random.Range(0.5f, 5f), Random.Range(0.5f, 5f), Random.Range(0.5f, 5f));
            TargetTransform = NetworkSyncedTransform.transform;
            InitialScale = TargetTransform.localScale.x;
        }

        void Update()
        {
            if (NetworkGate.IsServer)
            {
                TargetTransform.Rotate(Time.deltaTime * RotationSpeed, Space.Self);
                float sign = Mathf.Sign(Time.time % 10 - 5);
                TargetTransform.Translate(sign * Time.deltaTime * TranslationSpeed);
                
                float scale = 1 + Time.deltaTime * ScaleSpeed;
                TargetTransform.localScale *= sign > 0 ? scale : 1 / scale;
            }
        }
    }
}
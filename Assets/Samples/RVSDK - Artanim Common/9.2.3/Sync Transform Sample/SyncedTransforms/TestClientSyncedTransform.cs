using RvSdk.Controller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RvSdk.Samples
{
    public class TestClientSyncedTransform : MonoBehaviour
    {
        [field: SerializeField] Material DefaultMaterial { get; set; }
        [field: SerializeField] Material StaticObjectMaterial { get; set; }
        [field: SerializeField] MeshRenderer MeshRenderer { get; set; }
        [field: SerializeField] bool IsStatic { get; set; } = false;

        public NetworkSyncedTransform NetworkSyncedTransform;

        private Vector3 RotationSpeed;
        private Vector3 TranslationSpeed = new Vector3(0.2f, 0, 0.2f);
        private float ScaleSpeed = 0.2f;
        private Transform TargetTransform;

        void Start()
        {
            RotationSpeed = 60f * new Vector3(Random.Range(0.5f, 5f), Random.Range(0.5f, 5f), Random.Range(0.5f, 5f));
            TargetTransform = NetworkSyncedTransform.transform;

            MeshRenderer.material = IsStatic ? StaticObjectMaterial : DefaultMaterial;
        }

        void Update()
        {
            if (!IsStatic)
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
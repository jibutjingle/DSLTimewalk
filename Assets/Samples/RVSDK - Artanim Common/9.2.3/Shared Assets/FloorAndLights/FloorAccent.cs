using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RvSdk.Samples
{
    public class FloorAccent : MonoBehaviour
    {
        private const string RAILING_PREFIX = "hPole";
        private const float FAILING_MAX_VERTICAL = 0.9f;

        [SerializeField] Color FloorAccentColor;
        [SerializeField] Transform RailingRoot;
        
        private void OnEnable()
        {
            if(RailingRoot)
            {
                var accentRailings = RailingRoot.GetComponentsInChildren<Renderer>()
                    .Where(r => r.name.StartsWith(RAILING_PREFIX) && r.transform.localPosition.y < FAILING_MAX_VERTICAL);

                foreach (var renderer in accentRailings)
                    renderer.material.color = FloorAccentColor;
            }
        }
    }
}
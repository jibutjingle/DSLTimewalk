using RvSdk.Internal.Utils;
using RvSdk.Module;
using UnityEngine;

namespace RvSdk.Samples
{
    public class SyncedTransformSpawner : MonoBehaviour
    {
        public int NumTransforms = 10;
        public float MaxPositionOffset = 10f;
        public GameObject SyncedTransformTemplate;

        void Start()
        {
            //Spawn
            if (SyncedTransformTemplate)
            {
                for (var i = 0; i < NumTransforms; ++i)
                {
                    var instance = UnityUtils.InstantiatePrefab<TestSyncedTransform>(SyncedTransformTemplate, transform);
                    instance.NetworkSyncedTransform.Id = string.Format("SyncedTransform-{0}", i);

                    if (NetworkGate.IsServer)
                    {
                        //Position random
                        var pos = Random.insideUnitSphere * MaxPositionOffset;
                        pos.y += MaxPositionOffset;
                        instance.transform.position = pos;
                    }
                }

            }
        }

    }
}
using RvSdk.Internal.Utils;
using UnityEngine;

namespace RvSdk.Samples
{
    public class BallSpawner : MonoBehaviour
    {
        private const float SPAWN_RANGE = 0.2f;

        public GameObject BallTemplate;

        public Material BallMaterial;

        public void DoSpawn()
        {
            if (BallTemplate)
            {
                var renderer = UnityUtils.InstantiatePrefab<Renderer>(BallTemplate, transform);
                if (renderer)
                {
                    renderer.transform.localPosition = new Vector3(Random.Range(0f, SPAWN_RANGE), 0f, Random.Range(0f, SPAWN_RANGE));
                    renderer.material = BallMaterial;
                }
            }
        }

        public void DoReset()
        {
            UnityUtils.RemoveAllChildren(transform);
        }
    }
}

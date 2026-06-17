using RvSdk.Controller;
using RvSdk.Internal.Utils;
using RvSdk.Module;
using UnityEngine;

namespace RvSdk.Samples
{
    public class ClientSyncedTransformSpawner : MonoBehaviour
    {
        public string SyncedObjectResourcePath;

        public string SyncedObjectStaticResourcePath;

        void Start()
        {
            if (NetworkGate.IsClient)
            {
                if (GameController.Instance.CurrentSession == null)
                {
                    return;
                }

                var player = GameController.Instance.CurrentPlayer;
                if (player == null)
                {
                    return;
                }

                // First player only register the transform, and will be able to sync the transform
                if (player.Player.ComponentId == GameController.Instance.CurrentSession.Players[0].ComponentId)
                {
                    // Load the resource using the resource path
                    GameObject prefab1 = Resources.Load<GameObject>(SyncedObjectResourcePath);
                    GameObject prefab2 = Resources.Load<GameObject>(SyncedObjectStaticResourcePath);

                    // Spawn 4 cubes at positions (0,1,1), (0,1,-1), (1,1,0), (-1,1,0). Two will be statics, others will move randomly.
                    for (int i = 0; i < 4; i++)
                    {
                        var x = 1f;
                        var prefab = prefab1;
                        var prefabPathResource = SyncedObjectResourcePath;
                        if ((i == 0 || i == 3))
                        {
                            x = -1f;
                            prefab = prefab2;
                            prefabPathResource = SyncedObjectStaticResourcePath;
                        }

                        var z = (i < 2) ? 1f : -1f;
                        var position = new Vector3(x, 1f, z);
                        var instance = UnityUtils.InstantiatePrefab<TestClientSyncedTransform>(prefab, transform);
                        instance.NetworkSyncedTransform.Id = string.Format($"Client-{NetworkGate.NetworkGuid}-{i}");
                        instance.transform.position = position;

                        // Register the synced transform
                        instance.NetworkSyncedTransform.PrefabResourcePath = prefabPathResource;
                        instance.NetworkSyncedTransform.Register(true);
                    }

                }
            }
        }
    }

}

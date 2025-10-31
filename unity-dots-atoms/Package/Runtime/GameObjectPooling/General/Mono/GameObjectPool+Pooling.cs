using System.Collections.Generic;
using DotsAtoms.GameObjectPooling.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DotsAtoms.GameObjectPooling.Mono
{
    public partial class GameObjectPool
    {
        private class Pool
        {
            public GameObject Prefab;
            public readonly List<GameObject> PooledInstances = new();
#if UNITY_EDITOR
            public GameObject PoolParent;
#endif
        }


        private readonly Dictionary<GameObject, Pool> Pools = new();
        private readonly Dictionary<Hash128, Pool> PoolsByGuid = new();

        private readonly Dictionary<Object, Object>
            ProcessedPrewarmContainers = new(); // Container : Processed by parent


        private void PrewarmPools()
        {
            ProcessedPrewarmContainers.Clear();
            PrewarmPrefabsInList(PrewarmPrefabs, gameObject);
            ProcessedPrewarmContainers.Clear();
        }

        private bool DetectPrewarmAssetDuplicate(Object container, Object parentContainer)
        {
            if (ProcessedPrewarmContainers.TryGetValue(container, out var knownContainer)) {
                Debug.LogError(
                    $"Prewarm Prefabs '{container.name}' has already been added by '{knownContainer.name}'"
                );
                return true;
            }

            ProcessedPrewarmContainers.Add(container, parentContainer);

            return false;
        }

        private void PrewarmPrefabsInList(List<PrewarmConfig> prewarmPrefabs, Object container)
        {
            List<GameObjectPoolPrewarmConfig> subConfigs = new();

            foreach (var poolingConfig in prewarmPrefabs) {
                switch (poolingConfig.Prefab) {
                    case GameObjectPoolPrewarmConfig subConfig:
                        if (DetectPrewarmAssetDuplicate(subConfig, container)) continue;

                        subConfigs.Add(subConfig);
                        continue;


                    case GameObject prefab:
                        if (DetectPrewarmAssetDuplicate(prefab, container)) continue;

                        var prefabGuid = poolingConfig.PrefabGuid;
                        var count = poolingConfig.Count;
                        Debug.Log($"Prewarming {count} instances of {prefab.name} [{prefabGuid}]");

                        var pool = new Pool();
                        pool.Prefab = prefab;
#if UNITY_EDITOR
                        pool.PoolParent = new GameObject(prefab.name);
                        pool.PoolParent.transform.SetParent(transform, worldPositionStays: false);
#endif
                        Pools.Add(prefab, pool);
                        PoolsByGuid.Add(prefabGuid, pool);

                        for (var i = 0; i < count; i++) {
                            ReturnToPoolInternal(InstantiateFromPrefab(prefab), isInstantiating: true);
                        }

                        continue;

                    default:
                        Debug.LogError(
                            "Prewarm Prefabs contains entry that is neither a GameObject nor a GameObjectPoolPrewarmConfig",
                            container
                        );
                        continue;
                }
            }

            foreach (var config in subConfigs) {
                PrewarmPrefabsInList(config.PrewarmPrefabs, config);
            }
        }


        private PooledGameObject InstantiateFromPrefab(GameObject prefab)
        {
            var instance = Instantiate(prefab);
            var pooled = instance.GetComponent<PooledGameObject>() ?? instance.AddComponent<PooledGameObject>();
            pooled.InitializeWithPool(this, prefab);

#if UNITY_EDITOR
            instance.name = prefab.name;
#endif
            return pooled;
        }

        private GameObject InstantiateInternal(GameObject prefab, Hash128 prefabGuid)
        {
            if (!PoolsByGuid.TryGetValue(prefabGuid, out var pool)) {
                Debug.LogWarning(
                    $"Prefab {prefab.name}:{prefabGuid} is not pooled yet. Pooled prefabs used by entities with GameObjectViewPrefab must be added to the PrewarmConfig. If you don't need pre-warmed instances, set their number to 0 int the config.",
                    prefab
                );

                return InstantiateFromPrefab(prefab).gameObject;
            }

            return InstantiateFromPool(pool);
        }


        private GameObject InstantiateInternal(GameObject prefab)
        {
            if (!Pools.TryGetValue(prefab, out var pool)) {
                Debug.LogWarning(
                    $"Prefab {prefab.name} is not pooled yet. Consider adding it to Prewarm Prefabs.",
                    prefab
                );


                return InstantiateFromPrefab(prefab).gameObject;
            }

            return InstantiateFromPool(pool);
        }

        private GameObject InstantiateFromPool(Pool pool)
        {
            var pooledInstances = pool.PooledInstances;

            if (pooledInstances.Count > 0) {
                var index = pooledInstances.Count - 1;
                var instance = pooledInstances[index];
                pooledInstances.RemoveAt(index);

                instance.gameObject.SetActive(true);
#if UNITY_EDITOR
                instance.transform.SetParent(null, false);
#endif
                return instance;
            }

            return InstantiateFromPrefab(pool.Prefab).gameObject;
        }


        public void ReturnToPool(GameObject pooledObject) => pooledObject.ReturnToPool();


        internal void ReturnToPoolInternal(PooledGameObject pooledObject, bool isInstantiating = false)
        {
            var prefab = pooledObject.Prefab;

            if (!Pools.TryGetValue(prefab, out var pool)) {
                Debug.LogError($"Prefab {prefab.name} is not pooled by this pool.", prefab);
                return;
            }

            var pooledInstances = pool.PooledInstances;
            var instance = pooledObject.gameObject;

            pooledInstances.Add(instance);

            instance.SetActive(false);
#if UNITY_EDITOR
            instance.transform.SetParent(pool.PoolParent.transform, false);
#endif

            if (!isInstantiating) {
                pooledObject.OnReturnedToPool();
            }
        }
    }
}

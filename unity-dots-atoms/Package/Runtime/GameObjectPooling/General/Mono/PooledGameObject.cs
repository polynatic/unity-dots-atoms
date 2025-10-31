using DotsAtoms.GameObjectPooling.Interfaces;
using UnityEngine;

namespace DotsAtoms.GameObjectPooling.Mono
{
    /// <summary>
    /// Added to game objects when they are instantiated through a GameObjectPool.
    /// </summary>
    [DisallowMultipleComponent]
    internal class PooledGameObject : MonoBehaviour
    {
        internal GameObjectPool Pool;
        internal GameObject Prefab;

        private IPooledMonoBehaviour[] Behaviours;

        private void Awake() => Behaviours = GetComponentsInChildren<IPooledMonoBehaviour>();

        internal void InitializeWithPool(GameObjectPool pool, GameObject prefab)
        {
            Pool = pool;
            Prefab = prefab;
        }

        internal void OnReturnedToPool()
        {
            foreach (var behaviour in Behaviours) {
                behaviour.OnReturnedToPool();
            }
        }
    }
}

using DotsAtoms.GameObjectPooling.Mono;
using UnityEngine;

namespace DotsAtoms.GameObjectPooling.Extensions
{
    public static class GameObjectExtensions
    {
        public static void ReturnToPool(this GameObject gameObject)
        {
            var pooledObject = gameObject.GetComponent<PooledGameObject>();

            if (pooledObject == null) {
                Debug.LogError($"{gameObject.name} is not a pooled object.", gameObject);
                return;
            }

            pooledObject.Pool.ReturnToPoolInternal(pooledObject);
        }
    }
}

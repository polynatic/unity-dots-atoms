using Cysharp.Threading.Tasks;
using DotsAtoms.GameObjectViews.Interfaces;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DotsAtoms.GameObjectViews.Data
{
    public class GameObjectViewInterface<T> : IComponentData
    {
        public T Value;

        public class FromScene : MonoBehaviour, IGameObjectView
        {
            private T Instance;

            public void Awake()
            {
                try {
                    foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects()) {
                        if (root.GetComponentInChildren<T>() is { } instance) {
                            Instance = instance;
                            break;
                        }
                    }
                }
                catch {
                    Debug.LogException(new($"Can't find interface {typeof(T).Name} in scene"));
                }
            }

            public void OnViewAttached(in Entity entity, EntityCommandBuffer commands)
            {
                if (Instance == null) return;

                commands.AddComponent(entity, new GameObjectViewInterface<T> { Value = Instance });
            }

            public void OnViewDetached(in Entity entity, EntityCommandBuffer commands)
            {
                commands.RemoveComponent<GameObjectViewInterface<T>>(entity);
            }
        }
    }
}

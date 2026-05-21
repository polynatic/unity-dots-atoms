using DotsAtoms.GameObjectViews.Interfaces;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DotsAtoms.GameObjectViews.Data
{
    public partial class GameObjectViewLink<T> : IComponentData
    {
        public T Value;

        public GameObjectViewLink() { }

        public GameObjectViewLink(T value) => new GameObjectViewLink<T> { Value = value };


        /// <summary>
        /// Usage:
        ///
        /// 1. Create a new MonoBehaviour file and structure it like this:
        ///
        /// [assembly: RegisterGenericComponentType(typeof(GameObjectViewLink&lt;MyCustomStuffLink&gt;))]
        ///
        /// public class MyCustomStuffLink : GameObjectViewLink&lt;MyCustomStuff&gt;.FromScene
        /// {
        ///     [LinkInspector(typeof(MyCustomStuffLink))] public class Inspector : DefaultInspector { }
        /// }
        ///
        /// 2. Create a new prefab, add your MyCustomStuffLink component to it and store it somewhere in your assets.
        ///
        /// 3. In your baking SubScene, add a new Object and add a GameObjectViewPrefab to it. Link the Prefab field
        ///    to the prefab asset created in step 2.
        ///
        /// When the SubScene is loaded, the prefab will be instantiated and automatically search for an object with
        /// the generic type T in the scene hierarchy and if it finds one, an IComponentData of
        /// GameObjectViewLink&lt;MyCustomStuff&gt; will be added to the entity, allowing you to access the instance
        /// of MyCustomStuff in the scene from any system by simply doing SystemAPI.ManagedAPI.GetSingleton().
        ///
        /// The LinkInspector makes it possible to view if a compatible GameObject is found in your current scene.
        /// </summary>
        public partial class FromScene : MonoBehaviour, IGameObjectView
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
                    Debug.LogException(new($"GameObjectViewLink: Can't find instance of {typeof(T).Name} in scene"));
                }
            }

            public void OnViewAttached(in Entity entity, EntityCommandBuffer commands)
            {
                if (Instance == null) return;

                commands.AddComponent(entity, new GameObjectViewLink<T> { Value = Instance });
            }

            public void OnViewDetached(in Entity entity, EntityCommandBuffer commands)
            {
                commands.RemoveComponent<GameObjectViewLink<T>>(entity);
            }
        }
    }
}

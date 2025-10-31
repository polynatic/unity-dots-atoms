using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DotsAtoms.GameObjectViews.Mono
{
    /// <summary>
    /// A singleton that must be present on the current scene to enable GameObjectView usage. Without it, views cannot
    /// be instantiated. If you need more control over instantiation, for example for pooling or dependency injection,
    /// you can derive from this class, override InstantiateView() and DestroyView() and use this instead.
    /// </summary>
    [DisallowMultipleComponent]
    public class GameObjectViewContext : MonoBehaviour
    {
        /// <summary>
        /// Instantiate a GameObjectView from a given prefab.
        /// </summary>
        public virtual GameObject InstantiateView(GameObject prefab) => Instantiate(prefab);

        /// <summary>
        /// Destroy the given GameObjectView.
        /// </summary>
        public virtual void DestroyView(GameObjectView view) => Destroy(view.gameObject);


        /// <summary>
        /// Instantiate a GameObjectView from a given prefab.
        /// </summary>
        protected virtual GameObject InstantiateView(GameObject prefab, Hash128 prefabGuid) => InstantiateView(prefab);


        /// <summary>
        /// Used internally for GameObjectViewPrefab components for stable identifiers, because UnityObjectRef prefab
        /// references deviate from the original file during baking.
        /// </summary>
        internal GameObjectView InstantiateViewWithGuid(GameObject prefab, Hash128 prefabGuid)
        {
            var instance = InstantiateView(prefab, prefabGuid);
            var view = instance.GetComponent<GameObjectView>();
            view.Context = this;
            return view;
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/DotsAtoms/GameObjectViewContext/Plain", false, 10)]
        public static void CreateGameObjectViewContext(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectViewContext");
            gameObject.AddComponent<GameObjectViewContext>();

            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectView Context");
            Selection.activeObject = gameObject;
        }
#endif
    }
}

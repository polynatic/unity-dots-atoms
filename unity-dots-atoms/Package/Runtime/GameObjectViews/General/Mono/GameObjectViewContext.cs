using UnityEditor;
using UnityEngine;

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


        internal GameObjectView InstantiateViewInternal(GameObject prefab)
        {
            var instance = InstantiateView(prefab);
            var view = instance.GetComponent<GameObjectView>();
            view.Context = this;
            return view;
        }


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
    }
}

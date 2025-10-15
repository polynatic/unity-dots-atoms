using UnityEditor;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Mono
{
    /// <summary>
    /// A singleton that must be present on the current scene to enable GameObjectView usage. Without it, views cannot
    /// be instantiated. If you need more control over instantiation, for example for pooling or dependency injection,
    /// you can derive from this class, override InstantiateView() and DestroyView() and use this instead.
    /// </summary>
    public class GameObjectViewContext : MonoBehaviour
    {
        /// <summary>
        /// Instantiate a GameObjectView from a given prefab.
        /// </summary>
        public virtual GameObject InstantiateView(GameObject prefab)
        {
            var instance = Instantiate(prefab);
            var view = instance.GetComponent<GameObjectView>();
            view.Context = this;
            return instance;
        }

        /// <summary>
        /// Destroy the given GameObjectView.
        /// </summary>
        public virtual void DestroyView(GameObjectView view) => Destroy(view.gameObject);


        [MenuItem("GameObject/DotsAtoms/GameObjectView Context", false, 10)]
        static void CreateGameObjectViewContext(MenuCommand menu)
        {
            var go = new GameObject("GameObjectView Context");
            go.AddComponent<GameObjectViewContext>();

            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(go, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create GameObjectView Context");
            Selection.activeObject = go;
        }
    }
}

using DotsAtoms.GameObjectPooling.Mono;
using UnityEditor;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Mono
{
    /// <summary>
    /// Like GameObjectViewContext, but instead of directly instantiating and destroying objects, it will fetch and
    /// return all objects from a GameObjectPool. Make sure to assign the Pool field before the context is used. Also
    /// keep in mind that pooled objects don't automatically reset to their original state. Implement
    /// IPooledMonoBehaviour on all your MonoBehaviours that have custom logic to reset them.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameObjectViewContextPooled : GameObjectViewContext
    {
        [SerializeField] internal GameObjectPool Pool;

        public override GameObject InstantiateView(GameObject prefab) => Pool.InstantiatePooled(prefab);

        public override void DestroyView(GameObjectView view) => Pool.ReturnToPool(view.gameObject);


        [MenuItem("GameObject/DotsAtoms/GameObjectViewContext/Pooled", false, 10)]
        static void CreateGameObjectViewContextPooled(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectViewContext");
            var context = gameObject.AddComponent<GameObjectViewContextPooled>();
            var pool = FindAnyObjectByType<GameObjectPool>() ?? gameObject.AddComponent<GameObjectPool>();
            context.Pool = pool;


            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectViewContext (Pooled)");
            Selection.activeObject = gameObject;
        }
    }
}

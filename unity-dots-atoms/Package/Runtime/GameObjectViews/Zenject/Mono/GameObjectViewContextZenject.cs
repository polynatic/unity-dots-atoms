using DotsAtoms.GameObjectPooling.Zenject.Mono;
using DotsAtoms.GameObjectViews.Mono;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;

#endif

namespace DotsAtoms.GameObjectViews.Zenject.Mono
{
    /// <summary>
    /// Instantiates all objects from a Zenject DiContainer.
    /// </summary>
    public sealed class GameObjectViewContextZenject : GameObjectViewContext
    {
        [Inject] private DiContainer DiContainer;

        public override GameObject InstantiateView(GameObject prefab) => DiContainer.InstantiatePrefab(prefab);

#if UNITY_EDITOR
        [MenuItem("GameObject/DotsAtoms/GameObjectViewContext/Zenject", false, 10)]
        public static void CreateGameObjectViewContextZenject(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectViewContext");
            gameObject.AddComponent<GameObjectViewContextZenject>();

            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectViewContext (Zenject)");
            Selection.activeObject = gameObject;
        }


        [MenuItem("GameObject/DotsAtoms/GameObjectViewContext/Zenject + Pooled", false, 10)]
        public static void CreateGameObjectViewContextZenjectPooled(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectViewContext");
            var context = gameObject.AddComponent<GameObjectViewContextPooled>();
            var pool = FindAnyObjectByType<GameObjectPoolZenject>() ?? gameObject.AddComponent<GameObjectPoolZenject>();
            context.Pool = pool;


            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectViewContext (Zenject, Pooled)");
            Selection.activeObject = gameObject;
        }
#endif
    }
}

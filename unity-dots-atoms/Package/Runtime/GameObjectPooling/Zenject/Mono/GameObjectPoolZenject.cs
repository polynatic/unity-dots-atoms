using DotsAtoms.GameObjectPooling.Mono;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace DotsAtoms.GameObjectPooling.Zenject.Mono
{
    /// <summary>
    /// Instantiates all objects from a Zenject DiContainer.
    /// </summary>
    public sealed class GameObjectPoolZenject : GameObjectPool
    {
        [Inject] private DiContainer DiContainer;

        public override GameObject Instantiate(GameObject prefab) => DiContainer.InstantiatePrefab(prefab);


        [MenuItem("GameObject/DotsAtoms/GameObjectPool/Zenject", false, 10)]
        public static void CreateGameObjectPoolZenject(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectPool");
            gameObject.AddComponent<GameObjectPoolZenject>();

            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectPool (Zenject)");
            Selection.activeObject = gameObject;
        }
    }
}

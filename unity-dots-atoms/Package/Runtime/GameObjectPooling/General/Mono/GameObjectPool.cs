using System;
using System.Collections.Generic;
using DotsAtoms.GameObjectPooling.Extensions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;


namespace DotsAtoms.GameObjectPooling.Mono
{
    public partial class GameObjectPool : MonoBehaviour
    {
        [MenuItem("GameObject/DotsAtoms/GameObjectPool/Plain", false, 10)]
        public static void CreateGameObjectPool(MenuCommand menu)
        {
            var gameObject = new GameObject("GameObjectPool");
            gameObject.AddComponent<GameObjectPool>();

            // ensure proper parenting when right-clicking on an object
            GameObjectUtility.SetParentAndAlign(gameObject, menu.context as GameObject);
            Undo.RegisterCreatedObjectUndo(gameObject, "Create GameObjectPool");
            Selection.activeObject = gameObject;
        }

        private void Start() => PrewarmPools();


        public T InstantiatePooled<T>(T prefabComponent)
            where T : MonoBehaviour =>
            InstantiatePooled<T>(prefabComponent.gameObject);

        public T InstantiatePooled<T>(GameObject prefab)
            where T : MonoBehaviour =>
            InstantiateInternal(prefab).GetComponent<T>();

        public GameObject InstantiatePooled(GameObject prefab) => InstantiateInternal(prefab);


        #region Custom Instantiate/Destroy

        /// <summary>
        /// Override to implement custom Instantiate logic.
        /// </summary>
        public virtual GameObject Instantiate(GameObject prefab) => Object.Instantiate(prefab);

        /// <summary>
        /// Override to implement custom Destroy logic.
        /// </summary>
        public virtual void Destroy(GameObject gameObject) => Object.Destroy(gameObject);

        #endregion
    }
}

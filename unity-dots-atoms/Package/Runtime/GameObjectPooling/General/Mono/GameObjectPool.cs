using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace DotsAtoms.GameObjectPooling.Mono
{
    public partial class GameObjectPool : MonoBehaviour
    {
        private void Start() => PrewarmPools();


        public T InstantiatePooled<T>(T prefabComponent)
            where T : MonoBehaviour =>
            InstantiatePooled<T>(prefabComponent.gameObject);

        public T InstantiatePooled<T>(GameObject prefab)
            where T : MonoBehaviour =>
            InstantiateInternal(prefab).GetComponent<T>();


        public GameObject InstantiatePooled(GameObject prefab) => InstantiateInternal(prefab);

        internal GameObject InstantiatePooledByGuid(GameObject prefab, Hash128 prefabGuid) =>
            InstantiateInternal(prefab, prefabGuid);


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

#if UNITY_EDITOR
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
#endif
    }
}

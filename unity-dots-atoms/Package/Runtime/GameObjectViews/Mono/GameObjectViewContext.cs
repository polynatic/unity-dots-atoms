using UnityEditor;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Mono
{
    public class GameObjectViewContext : MonoBehaviour
    {
        internal GameObject InstantiatePrefab(GameObject prefab) => Instantiate(prefab);


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

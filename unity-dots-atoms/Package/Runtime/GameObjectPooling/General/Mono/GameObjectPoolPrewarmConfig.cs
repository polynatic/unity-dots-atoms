using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DotsAtoms.GameObjectPooling.Mono
{
    [CreateAssetMenu(
        fileName = "PoolingPrewarmConfig",
        menuName = "DotsAtoms/Pooling Prewarm Config",
        order = 0
    )]
    public class GameObjectPoolPrewarmConfig : ScriptableObject
    {
        [SerializeField] internal List<GameObjectPool.PrewarmConfig> PrewarmPrefabs;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var prewarmPrefab in PrewarmPrefabs) {
                MakeSureGuidIsCorrect(this, prewarmPrefab);
            }
        }


        /// <summary>
        /// Validates a PrewarmConfig and marks the owner as dirty if anything needed to be fixed.
        /// </summary>
        internal static void MakeSureGuidIsCorrect(Object owner, GameObjectPool.PrewarmConfig prewarmPrefab)
        {
            if (prewarmPrefab.Prefab == null) return;

            var path = AssetDatabase.GetAssetPath(prewarmPrefab.Prefab);

            if (string.IsNullOrWhiteSpace(path)) {
                Debug.LogError("Prefabs in a GameObjectPoolPrewarmConfig must be files and no scene objects.");
                prewarmPrefab.Prefab = null;
                prewarmPrefab.PrefabGuid = default;
                return;
            }

            var guid = Hash128.Parse(AssetDatabase.GUIDFromAssetPath(path).ToString());
            if (guid == prewarmPrefab.PrefabGuid) return;

            prewarmPrefab.PrefabGuid = guid;
            EditorUtility.SetDirty(owner);
        }
#endif
    }
}

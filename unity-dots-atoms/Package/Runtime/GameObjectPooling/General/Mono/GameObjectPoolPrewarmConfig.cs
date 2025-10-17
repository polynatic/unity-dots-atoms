using System.Collections.Generic;
using UnityEngine;

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
    }
}

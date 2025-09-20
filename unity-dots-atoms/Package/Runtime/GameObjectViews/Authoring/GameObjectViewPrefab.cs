using Unity.Entities;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Authoring
{
    /// <summary>
    /// References a GameObjectView to be used as prefab for the entity that is baked with this component. When the
    /// entity is spawned, the prefab will automatically be instantiated and associated with the entity. The transform
    /// will be copied to the game object according to the settings of the GameObjectView.
    /// </summary>
    public class GameObjectViewPrefab : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Assign a prefab that should be instantiated and used as a view for this entity.")]
        private GameObjectView Prefab;

        private class Baker : Baker<GameObjectViewPrefab>
        {
            public override void Bake(GameObjectViewPrefab authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(
                    entity,
                    new Data.GameObjectView.Prefab(authoring.Prefab.gameObject)
                );
            }
        }
    }
}

using Unity.Entities;

namespace DotsAtoms.GameObjectViews.Interfaces
{
    public interface IGameObjectView
    {
        void OnGameObjectViewInitialized(EntityManager entityManager, Entity entity, EntityCommandBuffer commands);
    }
}

using Cysharp.Threading.Tasks;
using Unity.Entities;

namespace DotsAtoms.GameObjectViews.Interfaces
{
    public interface IGameObjectView
    {
        /// <summary>
        /// Called when the view is attached to an entity. Provides a command buffer from
        /// EndSimulationEntityCommandBufferSystem for avoiding an immediate structural change.
        /// 
        /// For performance reasons, avoid calling EntityManager functions directly from this callback, as it will
        /// create a full sync point and complete all jobs even unrelated to the data you read. Instead, create a system
        /// and query only the exact data you need and update your view game object from there.
        /// </summary>
        void OnViewAttached(in Entity entity, EntityCommandBuffer commands) { }

        /// <summary>
        /// Called when the view is detached from an entity. Provides a command buffer from
        /// EndSimulationEntityCommandBufferSystem for avoiding an immediate structural change.
        /// After the view has been detached, it will be destroyed. If you want to delay destruction, implement
        /// OnViewDestroy and return a task that waits for the condition when it can be destroyed.
        /// 
        /// For performance reasons, avoid calling EntityManager functions directly from this callback, as it will
        /// create a full sync point and complete all jobs even unrelated to the data you read. Instead, create a system
        /// and query only the exact data you need and update your view game object from there.
        /// </summary>
        void OnViewDetached(in Entity entity, EntityCommandBuffer commands) { }

        /// <summary>
        /// Called before the view game object is destroyed or otherwise becomes unavailable (pooling) after the entity
        /// has been detached. Return a task to delay the view destruction, like for example waiting for a particle
        /// system on the view to finish all particles.
        /// </summary>
        UniTask OnViewWillDestroy() => default;
    }
}

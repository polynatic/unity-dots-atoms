using DotsAtoms.GameObjectViews.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.GameObjectViews.Systems
{
    using EcbSystemType = EndSimulationEntityCommandBufferSystem;


    [UpdateInGroup(typeof(StructuralChangePresentationSystemGroup))]
    [UpdateBefore(typeof(GameObjectViewUpdateTransforms))]
    [UpdateBefore(typeof(GameObjectViewUpdatePhysicsVelocity))]
    [CreateAfter(typeof(GameObjectViewInstantiate))]
    public partial struct GameObjectViewDestroy : ISystem
    {
        private EcbSystemType.Singleton EcbSystem;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EcbSystemType.Singleton>();
            state.RequireForUpdate<GameObjectView.Singleton>();
            EcbSystem = GetSingleton<EcbSystemType.Singleton>();
        }


        public void OnUpdate(ref SystemState state)
        {
            var singleton = GetSingleton<GameObjectView.Singleton>();
            var commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var transforms = singleton.Transforms;
            var entities = singleton.Entities;
            var rigidBodies = singleton.RigidBodies;

            foreach (
                var (gameObjectView, entity)
                in Query<RefRO<GameObjectView>>()
                   .WithNone<GameObjectView.IsAlive>()
                   .WithEntityAccess()
            ) {
                var index = entities.IndexOf(entity);
                transforms.RemoveAtSwapBack(index);
                entities.RemoveAtSwapBack(index);

                rigidBodies.Remove(gameObjectView.ValueRO.GameObject.GetComponent<Rigidbody>());
                commands.RemoveComponent<GameObjectView>(entity);

                Object.Destroy(gameObjectView.ValueRO.GameObject);
            }
        }
    }
}

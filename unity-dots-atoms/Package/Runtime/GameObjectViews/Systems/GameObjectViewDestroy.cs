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
    [UpdateAfter(typeof(GameObjectViewUpdateTransforms))]
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

            foreach (var (gameObjectView, entity) in Query<RefRO<GameObjectView>>()
                                                     .WithNone<GameObjectView.IsAlive>()
                                                     .WithEntityAccess()
                    ) {
                Object.Destroy(gameObjectView.ValueRO.GameObject);

                var index = entities.IndexOf(entity);
                transforms.RemoveAtSwapBack(index);
                entities.RemoveAtSwapBack(index);
                commands.RemoveComponent<GameObjectView>(entity);
            }
        }
    }
}

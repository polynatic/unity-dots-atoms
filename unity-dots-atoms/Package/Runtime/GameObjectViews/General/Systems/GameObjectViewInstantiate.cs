using DotsAtoms.GameObjectViews.Data;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.GameObjectViews.Systems
{
    using EcbSystemType = EndSimulationEntityCommandBufferSystem;

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(VariableRateSimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [CreateAfter(typeof(EcbSystemType))]
    public partial struct GameObjectViewInstantiate : ISystem
    {
        private EcbSystemType.Singleton EcbSystem;

        private int FramesWithoutSingleton;
        private const int FramesWithoutSingletonWarning = 60;


        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EcbSystemType.Singleton>();
            EcbSystem = GetSingleton<EcbSystemType.Singleton>();
        }


        public void OnDestroy(ref SystemState state)
        {
            if (!TryGetSingleton<GameObjectView.Singleton>(out var singleton)) return;

            singleton.Transforms.Dispose();
            singleton.Entities.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (!TryCreateSingleton(ref state, ref FramesWithoutSingleton, out var gameObjectViewSingleton)) return;

            var commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            ref var singleton = ref gameObjectViewSingleton.ValueRW;

            foreach (
                var (gameObjectView, entity)
                in Query<RefRO<GameObjectView.Prefab>>()
                   .WithNone<GameObjectView>()
                   .WithEntityAccess()
            ) {
                var instantiated = gameObjectView.ValueRO.Instantiate(singleton.Context);
                var gameObject = instantiated.GameObject;
                var hasRigidBody = gameObjectView.ValueRO.HasRigidBody;

                singleton.Transforms.Add(gameObject.transform);
                singleton.Entities.Add(entity);

                if (hasRigidBody) {
                    singleton.RigidBodies.Add(gameObject.GetComponent<Rigidbody>(), entity);
                }

                commands.AddComponent(entity, instantiated);
                commands.AddComponent<GameObjectView.IsAlive>(entity);
                commands.RemoveComponent<GameObjectView.Prefab>(entity);
                commands.SetComponentEnabled<GameObjectView.OnViewAttached>(entity, true);

                instantiated.View.OnViewAttached(entity, commands);
            }
        }

        private bool TryCreateSingleton(
            ref SystemState state,
            ref int frameCounter,
            out RefRW<GameObjectView.Singleton> singleton
        )
        {
            if (TryGetSingletonRW(out singleton)) return true;

            var context = Object.FindFirstObjectByType<Mono.GameObjectViewContext>();

            if (context == null) {
                if (frameCounter == -1) return false;
                if (frameCounter++ < FramesWithoutSingletonWarning) return false;

                Debug.LogWarning($"{nameof(GameObjectViewInstantiate)} requires a GameObjectViewContext");
                FramesWithoutSingleton = -1; // stop trying
                return false;
            }

            state.EntityManager.CreateSingleton(
                new GameObjectView.Singleton {
                    Transforms = new(128),
                    Entities = new(Allocator.Persistent),
                    RigidBodies = new(128, Allocator.Persistent),
                    Context = context,
                }
            );

            return TryGetSingletonRW(out singleton);
        }
    }
}

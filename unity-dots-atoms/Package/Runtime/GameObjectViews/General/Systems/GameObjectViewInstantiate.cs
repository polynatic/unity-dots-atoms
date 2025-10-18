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
        private GameObjectView.Singleton GameObjectViewSingleton;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EcbSystemType.Singleton>();
            state.RequireForUpdate<GameObjectView.Singleton>();
            EcbSystem = GetSingleton<EcbSystemType.Singleton>();

            var context = Object.FindFirstObjectByType<Mono.GameObjectViewContext>();

            if (context == null) {
                Debug.LogWarning($"{nameof(GameObjectViewInstantiate)} requires a GameObjectViewContext");
                return;
            }

            GameObjectViewSingleton = new() {
                Transforms = new(128),
                Entities = new(Allocator.Persistent),
                RigidBodies = new(128, Allocator.Persistent),
                Context = context,
            };

            state.EntityManager.CreateSingleton(GameObjectViewSingleton);
        }


        public void OnDestroy(ref SystemState state)
        {
            if (!TryGetSingleton(out GameObjectViewSingleton)) return;

            GameObjectViewSingleton.Transforms.Dispose();
            GameObjectViewSingleton.Entities.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            var transforms = GameObjectViewSingleton.Transforms;
            var entities = GameObjectViewSingleton.Entities;
            var rigidBodies = GameObjectViewSingleton.RigidBodies;
            var context = GameObjectViewSingleton.Context.Value;

            foreach (
                var (gameObjectView, entity)
                in Query<RefRO<GameObjectView.Prefab>>()
                   .WithNone<GameObjectView>()
                   .WithEntityAccess()
            ) {
                var instantiated = gameObjectView.ValueRO.Instantiate(context);
                var gameObject = instantiated.GameObject;
                var hasRigidBody = gameObjectView.ValueRO.HasRigidBody;

                transforms.Add(gameObject.transform);
                entities.Add(entity);

                if (hasRigidBody) {
                    rigidBodies.Add(gameObject.GetComponent<Rigidbody>(), entity);
                }

                commands.AddComponent(entity, instantiated);
                commands.AddComponent<GameObjectView.IsAlive>(entity);
                commands.RemoveComponent<GameObjectView.Prefab>(entity);
                commands.SetComponentEnabled<GameObjectView.OnViewAttached>(entity, true);

                instantiated.View.OnViewAttached(entity, commands);
            }
        }
    }
}

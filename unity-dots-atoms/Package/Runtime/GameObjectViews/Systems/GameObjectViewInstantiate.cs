using DotsAtoms.GameObjectViews.Data;
using DotsAtoms.GameObjectViews.Interfaces;
using DotsAtoms.GameObjectViews.Mono;
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

            var context = Object.FindFirstObjectByType<GameObjectViewContext>();

            if (context == null) {
                Debug.LogWarning($"{nameof(GameObjectViewInstantiate)} requires a GameObjectViewContext");
                return;
            }

            GameObjectViewSingleton = new() {
                Transforms = new(100),
                Entities = new(Allocator.Persistent),
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
            var context = GameObjectViewSingleton.Context.Value;

            foreach (var (gameObjectView, entity) in Query<RefRO<GameObjectView.Prefab>>()
                                                     .WithNone<GameObjectView>()
                                                     .WithEntityAccess()) {
                var instantiated = gameObjectView.ValueRO.Instantiate(context);
                transforms.Add(instantiated.GameObject.transform);
                entities.Add(entity);
                commands.AddComponent(entity, instantiated);
                commands.AddComponent<GameObjectView.IsAlive>(entity);
                commands.RemoveComponent<GameObjectView.Prefab>(entity);

                foreach (var view in instantiated.GameObject.GetComponentsInChildren<IGameObjectView>()) {
                    view.OnGameObjectViewInitialized(state.EntityManager, entity, commands);
                }
            }
        }
    }
}

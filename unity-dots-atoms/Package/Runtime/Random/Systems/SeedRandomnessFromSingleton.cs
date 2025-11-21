using DotsAtoms.Random.Data;
using Unity.Burst;
using Unity.Burst.Intrinsics;
using Unity.Collections;
using Unity.Entities;
using static Unity.Entities.ComponentType;
using static Unity.Entities.SystemAPI;
using EcbSystem = Unity.Entities.EndFixedStepSimulationEntityCommandBufferSystem;

namespace DotsAtoms.Random.Systems
{
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [CreateAfter(typeof(EcbSystem))] // for singleton
    public partial struct SeedRandomnessFromSingleton : ISystem
    {
        private EntityQuery Query;
        private EcbSystem.Singleton EcbSystem;
        private Randomness.Singleton.Query RandomnessSingleton;


        public void OnCreate(ref SystemState state)
        {
            MakeSureRandomnessSingletonExists(ref state);

            Query = new EntityQueryBuilder(Allocator.Temp)
                    .WithAll<Randomness.SeedFromSingleton>()
                    .Build(ref state);

            EcbSystem = GetSingleton<EcbSystem.Singleton>();
            RandomnessSingleton.UseSystemState(ref state);

            state.RequireForUpdate<EcbSystem.Singleton>();
            state.RequireForUpdate(Query);
        }

        private void MakeSureRandomnessSingletonExists(ref SystemState state)
        {
            if (TryGetSingletonEntity<Randomness.Singleton>(out _)) return;

            var entity = state.EntityManager.CreateEntity(
                ReadWrite<Randomness>(),
                ReadWrite<Randomness.Singleton>()
            );
            var seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            SetComponent(entity, Randomness.WithSeed(seed));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RandomnessSingleton.Update(ref state);

            state.Dependency = new Job {
                EntityType = GetEntityTypeHandle(),
                RandomnessSingleton = RandomnessSingleton,
                Commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged),
            }.Schedule(Query, state.Dependency);
        }


        [BurstCompile]
        private struct Job : IJobChunk
        {
            public Randomness.Singleton.Query RandomnessSingleton;
            public EntityCommandBuffer Commands;

            [ReadOnly] public EntityTypeHandle EntityType;

            [BurstCompile]
            public void Execute(
                in ArchetypeChunk chunk,
                int unfilteredChunkIndex,
                bool useEnabledMask,
                in v128 chunkEnabledMask
            )
            {
                var randomness = RandomnessSingleton.GetRandomRW().NextRandomness();
                var entities = chunk.GetNativeArray(EntityType);

                for (var i = 0; i < chunk.Count; i++) {
                    var entity = entities[i];
                    Commands.AddComponent(entity, randomness.NextRandomness());
                    Commands.RemoveComponent<Randomness.SeedFromSingleton>(entity);
                }
            }
        }
    }
}

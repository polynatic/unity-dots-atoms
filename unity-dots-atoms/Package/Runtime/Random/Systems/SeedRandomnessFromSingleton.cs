using DotsAtoms.Random.Data;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
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
                    .WithAll<
                        Randomness,
                        Randomness.SeedFromSingleton
                    >()
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
                ComponentType.ReadWrite<Randomness>(),
                ComponentType.ReadWrite<Randomness.Singleton>()
            );
            var seed = (uint)UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            SetComponent(entity, Randomness.WithSeed(seed));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) =>
            new Job {
                Random = RandomnessSingleton.GetRandomRW(ref state).NextRandom(),
                Commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged),
            }.Schedule(Query);


        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public Unity.Mathematics.Random Random;
            public EntityCommandBuffer Commands;

            [BurstCompile]
            public void Execute(in Entity entity, ref Randomness random)
            {
                random = Random.NextRandomness();
                Commands.RemoveComponent<Randomness.SeedFromSingleton>(entity);
            }
        }
    }


    public partial struct MyRandomSystem : ISystem
    {
        private Randomness.Singleton.Query Randomness; // Store query for singleton

        public void OnCreate(ref SystemState state)
        {
            Randomness.UseSystemState(ref state); // Initialize query for singleton
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            ref var random = ref Randomness.GetRandomRW(ref state); // Get Random value ref of singleton
            var randomInt = random.NextInt();
            
            
            // Use "child" randomness to enable jobs, because you can't pass a ref var.
            // This doesn't work with parallel jobs, because threads would modify the RNG state in unpredictable orders.
            new MyJob { Random = random.NextRandom() }.Schedule();
        }

        private partial struct MyJob : IJobEntity
        {
            public Unity.Mathematics.Random Random;

            public void Execute(Entity entity)
            {
                var randomInt = Random.NextInt();
            }
        }
    }
}

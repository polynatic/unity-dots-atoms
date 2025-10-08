using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using static DotsAtoms.Lifetime.Data.Lifetime;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.Lifetime.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct InitializeCreatedAtTime : ISystem
    {
        private EntityQuery Query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            Query = state.GetEntityQuery(
                new EntityQueryBuilder(Allocator.Temp).WithDisabledRW<CreatedAt>()
            );
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new Job { ElapsedTime = Time.ElapsedTime }.Schedule(Query);


        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public double ElapsedTime;

            void Execute(in Entity entity, ref CreatedAt createdAt, EnabledRefRW<CreatedAt> createdAtEnabled)
            {
                createdAt.Value = ElapsedTime;
                createdAtEnabled.ValueRW = true;
            }
        }
    }
}

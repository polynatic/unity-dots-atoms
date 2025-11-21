using DotsAtoms.Random.Data;
using Unity.Burst;
using Unity.Entities;

namespace DotsAtoms.Random.Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct RandomnessChaosUpdate : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new Job().Schedule();

        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public void Execute(ref Randomness randomness) => randomness.Value.NextUInt();
        }
    }
}

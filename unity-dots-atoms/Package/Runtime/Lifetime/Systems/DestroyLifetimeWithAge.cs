using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using static DotsAtoms.Lifetime.Data.Lifetime;
using static Unity.Entities.SystemAPI;
using EcbSystem = Unity.Entities.EndFixedStepSimulationEntityCommandBufferSystem;

namespace DotsAtoms.Lifetime.Systems
{
    /// <summary>
    /// Destroys entities when their maximum age has been reached.
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [CreateAfter(typeof(EcbSystem))] // for singleton
    public partial struct DestroyLifetimeWithAge : ISystem
    {
        private EcbSystem.Singleton EcbSystem;

        // ReSharper disable once Unity.Entities.SingletonMustBeRequested : Not calling GetSingleton in Update
        public void OnCreate(ref SystemState state) => EcbSystem = GetSingleton<EcbSystem.Singleton>();


        [BurstCompile]
        public void OnUpdate(ref SystemState state) =>
            new Job {
                ElapsedTime = Time.ElapsedTime,
                Commands = EcbSystem.CreateCommandBuffer(state.WorldUnmanaged),
            }.Schedule();


        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            public double ElapsedTime;
            public EntityCommandBuffer Commands;

            public void Execute(in Entity entity, in CreatedAt createdAt, in DestroyWithAge destroyWithAge)
            {
                if (ElapsedTime < createdAt.Value + destroyWithAge.Value) return;

                Commands.DestroyEntity(entity);
            }
        }
    }
}

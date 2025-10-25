using DotsAtoms.Physics.Forces.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;

namespace DotsAtoms.Physics.Forces.Systems
{
    [UpdateInGroup(typeof(BeforePhysicsSystemGroup))]
    [BurstCompile]
    public partial struct ApplyImpulses : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new Job().ScheduleParallel();

        private partial struct Job : IJobEntity
        {
            public void Execute(
                in LocalTransform transform,
                in PhysicsMass mass,
                ref PhysicsVelocity physicsVelocity,
                ref DynamicBuffer<ApplyImpulse> impulses
            )
            {
                var velocity = physicsVelocity; // register copy

                foreach (var impulse in impulses.AsNativeArray()) {
                    velocity.ApplyImpulse(
                        mass,
                        transform.Position,
                        transform.Rotation,
                        impulse.Impulse,
                        impulse.Point
                    );
                }

                impulses.Clear();

                physicsVelocity = velocity;
            }
        }
    }
}

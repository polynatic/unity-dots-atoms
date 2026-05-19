using DotsAtoms.GameObjectViews.Data;
using Extensions.Entities;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.GameObjectViews.Systems
{
    [UpdateInGroup(typeof(StructuralChangePresentationSystemGroup))]
    [CreateAfter(typeof(GameObjectViewInstantiate))]
    [UpdateAfter(typeof(GameObjectViewUpdateTransforms))]
    [BurstCompile]
    public partial struct GameObjectViewUpdatePhysicsVelocity : ISystem
    {
        private ComponentLookup<PhysicsVelocity> LookupPhysicsVelocity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameObjectView.Singleton>();

            LookupPhysicsVelocity.UseSystemStateReadOnly(ref state);
        }

        public void OnUpdate(ref SystemState state)
        {
            LookupPhysicsVelocity.Update(ref state);

            state.Dependency.Complete();

            var singleton = GetSingleton<GameObjectView.Singleton>();

            foreach (var kv in singleton.RigidBodies) {
                var entity = kv.Value;
                var rigidBody = kv.Key.Value;
                var physicsVelocity = LookupPhysicsVelocity[entity];
                rigidBody.linearVelocity = physicsVelocity.Linear;
                rigidBody.angularVelocity = physicsVelocity.Angular;
            }
        }
    }
}

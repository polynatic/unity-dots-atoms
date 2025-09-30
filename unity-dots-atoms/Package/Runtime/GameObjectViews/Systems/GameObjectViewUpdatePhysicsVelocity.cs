using DotsAtoms.GameObjectViews.Data;
using Extensions.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Jobs;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.GameObjectViews.Systems
{
    [UpdateInGroup(typeof(StructuralChangePresentationSystemGroup))]
    [CreateAfter(typeof(GameObjectViewInstantiate))]
    [UpdateAfter(typeof(GameObjectViewUpdateTransforms))]
    [BurstCompile]
    public partial struct GameObjectViewUpdatePhysicsVelocity : ISystem
    {
        private GameObjectView.Singleton GameObjectViewSingleton;
        private ComponentLookup<PhysicsVelocity> LookupPhysicsVelocity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameObjectView.Singleton>();

            LookupPhysicsVelocity.UseSystemStateReadOnly(ref state);

            TryGetSingleton(out GameObjectViewSingleton);
        }

        public void OnUpdate(ref SystemState state)
        {
            LookupPhysicsVelocity.Update(ref state);

            state.Dependency.Complete();

            var rigidBodies = GameObjectViewSingleton.RigidBodies;

            foreach (var kv in rigidBodies) {
                ref var entity = ref kv.Value;
                var rigidBody = kv.Key.Value;
                var physicsVelocity = LookupPhysicsVelocity[entity];
                rigidBody.linearVelocity = physicsVelocity.Linear;
                rigidBody.angularVelocity = physicsVelocity.Angular;
            }
        }
    }
}

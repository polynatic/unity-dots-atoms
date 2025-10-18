using DotsAtoms.GameObjectViews.Data;
using Extensions.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine.Jobs;
using static Unity.Entities.SystemAPI;

namespace DotsAtoms.GameObjectViews.Systems
{
    [UpdateInGroup(typeof(StructuralChangePresentationSystemGroup))]
    [CreateAfter(typeof(GameObjectViewInstantiate))]
    [BurstCompile]
    public partial struct GameObjectViewUpdateTransforms : ISystem
    {
        private GameObjectView.Singleton GameObjectViewSingleton;
        private ComponentLookup<LocalToWorld> LookupLocalTransform;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameObjectView.Singleton>();

            LookupLocalTransform.UseSystemStateReadOnly(ref state);

            TryGetSingleton(out GameObjectViewSingleton);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            LookupLocalTransform.Update(ref state);

            state.Dependency = new Job {
                    LookupLocalToWorld = LookupLocalTransform,
                    Entities = GameObjectViewSingleton.Entities,
                }
                .Schedule(GameObjectViewSingleton.Transforms, state.Dependency);
        }

        [BurstCompile]
        private partial struct Job : IJobParallelForTransform
        {
            [ReadOnly, NativeDisableParallelForRestriction]
            public ComponentLookup<LocalToWorld> LookupLocalToWorld;

            [ReadOnly, NativeDisableParallelForRestriction]
            public NativeList<Entity> Entities;

            [BurstCompile]
            public void Execute(int index, TransformAccess transform)
            {
                var entity = Entities[index];
                if (!LookupLocalToWorld.HasComponent(entity)) return;

                var entityTransform = LookupLocalToWorld[entity];
                transform.SetPositionAndRotation(entityTransform.Position, entityTransform.Rotation);
            }
        }
    }
}

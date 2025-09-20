using DotsAtoms.GameObjectViews.Data;
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
        private ComponentLookup<LocalToWorld> TypeHandleLocalTransform;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameObjectView.Singleton>();

            TryGetSingleton(out GameObjectViewSingleton);
            TypeHandleLocalTransform = GetComponentLookup<LocalToWorld>(isReadOnly: true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            TypeHandleLocalTransform.Update(ref state);
            state.Dependency = new Job {
                    LookupLocalToWorld = TypeHandleLocalTransform,
                    Entities = GameObjectViewSingleton.Entities,
                }
                .Schedule(GameObjectViewSingleton.Transforms, state.Dependency);

            state.Dependency.Complete();
        }

        [BurstCompile]
        private partial struct Job : IJobParallelForTransform
        {
            [ReadOnly, NativeDisableParallelForRestriction]
            public ComponentLookup<LocalToWorld> LookupLocalToWorld;

            [ReadOnly, NativeDisableParallelForRestriction]
            public NativeList<Entity> Entities;

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

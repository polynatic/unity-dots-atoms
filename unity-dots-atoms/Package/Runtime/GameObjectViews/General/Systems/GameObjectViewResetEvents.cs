using DotsAtoms.GameObjectViews.Data;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;

namespace DotsAtoms.GameObjectViews.Systems
{
    /// <summary>
    /// Resets all event components that can be used from inside the GameObjectViewUpdateGroup.
    /// </summary>
    [UpdateInGroup(typeof(UpdatePresentationSystemGroup))]
    [UpdateAfter(typeof(GameObjectViewUpdateGroup))]
    public partial struct GameObjectViewResetEvents : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state) => new Job().ScheduleParallel();

        [BurstCompile]
        private partial struct Job : IJobEntity
        {
            [BurstCompile]
            public void Execute(EnabledRefRW<GameObjectView.OnViewAttached> onViewAttached)
            {
                onViewAttached.ValueRW = false;
            }
        }
    }
}

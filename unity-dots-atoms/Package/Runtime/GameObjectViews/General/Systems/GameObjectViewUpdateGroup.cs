using Unity.Entities;
using Unity.Rendering;

namespace DotsAtoms.GameObjectViews.Systems
{
    /// <summary>
    /// Place all systems that update GameObjectViews with data from entities into this group.
    /// </summary>
    [UpdateInGroup(typeof(UpdatePresentationSystemGroup))]
    public partial class GameObjectViewUpdateGroup : ComponentSystemGroup { }
}

using Unity.Entities;

namespace DotsAtoms.Lifetime.Data
{
    public static partial class Lifetime
    {
        /// <summary>
        /// Stores the time the entity was created. Disable this component in Bakers or when adding to an entity to let it
        /// automatically initialize by the InitializeCreatedAtTime system. Set enabled to false to let it automatically
        /// be filled by the InitializeCreatedAtTime system.
        ///
        /// If you need this data on the frame that the entity is created, create/instantiate this component before the
        /// InitializeCreatedAtTime system runs and schedule the system that requires it after InitializeCreatedAtTime runs.
        /// </summary>
        public struct CreatedAt : IComponentData, IEnableableComponent
        {
            public double Value;
        }
    }
}

using Unity.Entities;
using Unity.Mathematics;

namespace DotsAtoms.Physics.Forces.Data
{
    /// <summary>
    /// Add append to this buffer from parallel jobs to efficiently apply impulses to the entity with the buffer.
    /// Impulses are applied before the PhysicsSystemGroup runs on each simulation frame.
    /// </summary>
    [InternalBufferCapacity(0)]
    public struct ApplyImpulse : IBufferElementData
    {
        /// <summary>
        /// Impulse direction and strength.
        /// </summary>
        public float3 Impulse;

        /// <summary>
        /// Point in world space from which the impulse is applied.
        /// </summary>
        public float3 Point;
    }
}

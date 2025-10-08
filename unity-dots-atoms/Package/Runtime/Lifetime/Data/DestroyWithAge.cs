using Unity.Entities;

namespace DotsAtoms.Lifetime.Data
{
    public static partial class Lifetime
    {
        /// <summary>
        /// When this component is present alongside a CreatedAt component, the entity will automatically be destroyed
        /// when the given age value has been reached.
        /// </summary>
        public struct DestroyWithAge : IComponentData
        {
            public double Value;

            public DestroyWithAge(double value) => Value = value;
        }
    }
}

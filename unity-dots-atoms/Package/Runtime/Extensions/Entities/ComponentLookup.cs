using Unity.Entities;

namespace Extensions.Entities
{
    public static class ComponentLookupExtensions
    {
        public static void UseSystemStateReadWrite<T>(
            ref this ComponentLookup<T> lookup,
            ref SystemState state
        )
            where T : unmanaged, IComponentData =>
            lookup = state.GetComponentLookup<T>(isReadOnly: false);

        public static void UseSystemStateReadOnly<T>(
            ref this ComponentLookup<T> lookup,
            ref SystemState state
        )
            where T : unmanaged, IComponentData =>
            lookup = state.GetComponentLookup<T>(isReadOnly: true);

        public static void UseSystemStateReadWrite<T>(
            ref this BufferLookup<T> lookup,
            ref SystemState state
        )
            where T : unmanaged, IBufferElementData =>
            lookup = state.GetBufferLookup<T>(isReadOnly: false);

        public static void UseSystemStateReadOnly<T>(
            ref this BufferLookup<T> lookup,
            ref SystemState state
        )
            where T : unmanaged, IBufferElementData =>
            lookup = state.GetBufferLookup<T>(isReadOnly: true);
    }
}

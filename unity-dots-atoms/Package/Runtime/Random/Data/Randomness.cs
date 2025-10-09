using Unity.Collections;
using Unity.Entities;

namespace DotsAtoms.Random.Data
{
    /// <summary>
    /// Provides a source of randomness for an entity.
    /// </summary>
    public struct Randomness : IComponentData
    {
        public Unity.Mathematics.Random Value;

        public static Randomness WithSeed(uint seed) => new Randomness { Value = new(seed) };


        /// <summary>
        /// Tag component to seed the Randomness component from the singleton. The tag will be automatically removed
        /// once the Randomness is seeded. Useful for entities that are not seeded by a spawner without Randomness.
        /// </summary>
        public struct SeedFromSingleton : IComponentData { }

        /// <summary>
        /// Tag component to distinguish the global randomness singleton entity. Direct usage should be avoided and
        /// instead hierarchical randomness should be preferred, where spawners have their own Randomness and pass it
        /// on to spawned entities via NextRandomness. This allows for unlimited scaling and usage in multithreaded jobs.
        /// </summary>
        public struct Singleton : IComponentData
        {
            /// <summary>
            /// Convenience wrapper to access the singleton Random value.
            /// </summary>
            public struct Query
            {
                private EntityQuery SingletonQuery;

                public void UseSystemState(ref SystemState state) =>
                    SingletonQuery = new EntityQueryBuilder(Allocator.Temp)
                                     .WithAllRW<Randomness, Singleton>()
                                     .Build(ref state);

                /// <summary>
                /// Get the singletons Random value for writing. Any changes due to calling random generation functions will
                /// automatically be applied to the original component. Don't forget to store it in a ref variable, or
                /// changes will not be applied!
                /// </summary>
                public ref Unity.Mathematics.Random GetRandomRW(ref SystemState state)
                {
                    var randomness = SingletonQuery.GetSingletonRW<Randomness>();
                    return ref randomness.ValueRW.Value;
                }
            }
        }
    }

    public static class RandomnessExtensions
    {
        /// <summary>
        /// Derive a new Randomness component from an existing one. Useful for creating hierarchical randomness.
        /// </summary>
        public static Randomness NextRandomness(this ref Randomness random) => random.Value.NextRandomness();

        /// <summary>
        /// Derive a new Randomness component from existing Random data. Useful for creating hierarchical randomness.
        /// </summary>
        public static Randomness NextRandomness(this ref Unity.Mathematics.Random random) =>
            new() { Value = new(random.NextUInt()) };

        /// <summary>
        /// Derive a new Random seeded from existing Random data. Useful for creating hierarchical randomness.
        /// </summary>
        public static Unity.Mathematics.Random NextRandom(this ref Unity.Mathematics.Random random) =>
            new(random.NextUInt());
    }
}

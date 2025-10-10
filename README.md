️⚠️ PRERELEASE ⚠️

This is used in some of my projects. Use anything with caution or for inspiration. Currently, neither features nor API
will remain sort of stable, as I'm figuring out how to improve it and figure out my requirements.

# DOTS Atoms for Unity

Small composable bits and systems to be used with Unity DOTS.

## Installation

* Open the Unity package manager (Window → PackageManager)
* Press the small plus sign (+) in the top left
* Select \[*Add package from git URL...*\]
* Enter the following URL and press *Add*

      https://github.com/polynatic/unity-dots-atoms.git?path=/unity-dots-atoms/Package#v0.0.1

You can install a different version by checking the Releases page on Github and using a different version at the end of
the package URL.

### Requires installed packages

* Entities
* Entities Graphics
* Burst
* Collections
* Mathematics
* Transforms

## Usage

tbd...

### GameObjectViews

1. Add GamObjectViewContext to exactly one GameObject in the scene, or create with right click.
2. Create a GameObject that will be used as the view, add the GameObjectView component to it and save it as a prefab.
3. Create an authoring GameObject for an entity and add the GameObjectViewPrefab component to it.
4. Drag the view prefab created in step 2 into the Prefab field of the GameObjectViewPrefab component.

When the entity we created in step 4 is instantiated, the view prefab GameObject will be instantiated automatically and
transforms will be applied to it. To access the GameObject view from a system,
you can query the GameObjectView component on the entity that holds a reference to the instance.

### Randomness

DotsAtoms encourages hierarchical randomness as a pattern, as it has no limitations, is efficient and works without any
drawbacks from multithreaded jobs, because there is no shared data access. Additionally, a single world seed makes it
possible to create deterministic randomness easily. The pattern works as follows:

There exists one _Randomness_ singleton with the initial seed for the whole world. Every entity that requires randomness
has their own _Randomness_ component, and systems working per entity should use their local components, not the
singleton. When instantiating new entities that require _Randomness_, the _Randomness_ of the
"parent" is passed on as a seed to the newly instantiated "child". Entities that don't get their _Randomness_
seeded by a parent spawner entity get it seeded from the world singleton.
This establishes a seeding hierarchy

    World Randomness Singleton
      -> Seeds Entity without Randomness Parent
        -> Seeds Instantiated Child Randomness
          -> and so on...

#### Usage

Add the _Randomness_ component to an entity that requires randomness.

```csharp
// Example baking code for Randomness on an entity

public class Baker : Baker<MyAuthoringComponent>
{
    public override void Bake(MyAuthoringComponent authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent<Randomness>(entity);
    }
}
```

In systems, query the _Randomness_ of each entity as usual. Make sure to use RefRW and ref in jobs, otherwise
the same random values will be generated over and over.

```csharp
// Example for accessing per entity Randomness in a system

foreach (var random in Query<RefRW<Randomness>>()) {
    var randomInt = random.ValueRW.Value.NextInt();
}
```

```csharp
// Example for accessing Randomness in a job

private partial struct Job : IJobEntity
{
    public void Execute(in Entity entity, ref Randomness random)
    {
        var randomInt = randomness.Value.NextInt();
    }
}
```

If an entity is spawned by another entity, the _Randomness_ needs to be passed on as seed to the newly spawned
entity by the spawner. This can also happen in parallel jobs, as each entity has their own _Randomness_.

```csharp
// Example for spawning from an entity

// var commands = Some EntityCommandBuffer ...;

foreach (var (spawner, randomness) in Query<RefRO<Spawner>, RefRW<Randomness>>()) {
    var instance = commands.Instantiate(spawner.ValueRO.Prefab);
    commands.SetComponent(instance, randomness.ValueRW.NextRandomness()); // pass on randomness
}
```

Entities that are not spawned from a "parent" need to be seeded by the singleton. This can be done by adding a
_Randomness.SeedFromSingleton_ component. It will be replaced with a seeded _Randomness_ component when after the

```csharp
// Example baking code for Randomness that is seeded by the global Randomness singleton

public class Baker : Baker<MyAuthoringComponent>
{
    public override void Bake(MyAuthoringComponent authoring)
    {
        var entity = GetEntity(TransformUsageFlags.None);
        AddComponent<Randomness.SeedFromSingleton>(entity);
    }
}
```

The singleton cannot be used in parallel jobs which makes this a little less efficient. Also, this _Randomness_ is only
seeded after the _SeedRandomnessFromSingleton_ system ran, so you need to be aware of that and schedule your systems
after. It also creates structural changes to replace the _SeedFromSingleton_ component which reduces efficiency even
more.
Because of this, it's recommended to use per entity _Randomness_ where possible, as it doesn't have these limitations
and is most efficient.

In case you need to access the singleton directly, a convenience method is provided.

```csharp
// Example for accessing the singleton directly

public partial struct MyRandomSystem : ISystem
{
    private Randomness.Singleton.Query Randomness; // Store query for singleton

    public void OnCreate(ref SystemState state)
    {
        Randomness.UseSystemState(ref state); // Initialize query for singleton
    }

    public void OnUpdate(ref SystemState state)
    {
        ref var random = ref Randomness.GetRandomRW(ref state); // Get Mathematics.Random ref of singleton
        var randomInt = random.NextInt(); // Generate something random


        // Use "child" randomness to enable jobs, because you can't pass a ref var.
        // This doesn't work with parallel jobs, because threads would modify the RNG state in unpredictable orders.
        new MyJob { Random = random.NextRandom() }.Schedule();
    }

    private partial struct MyJob : IJobEntity
    {
        public Unity.Mathematics.Random Random;

        public void Execute(Entity entity)
        {
            var randomInt = Random.NextInt(); // Generate something random
        }
    }
}
```

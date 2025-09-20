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
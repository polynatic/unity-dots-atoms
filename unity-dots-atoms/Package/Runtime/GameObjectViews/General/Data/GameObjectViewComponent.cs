using Unity.Entities;

namespace DotsAtoms.GameObjectViews.Data
{
    /// <summary>
    /// Add a reference to any UnityEngine.Object as component to an entity.<br/>
    /// <br/>
    /// Don't forget to register the generic type before using it. Example:<br/>
    /// [assembly: RegisterGenericComponentType(typeof(GameObjectViewComponent&lt;MyType&gt;))
    /// </summary>
    public struct GameObjectViewComponent<T> : IComponentData where T : UnityEngine.Object
    {
        private readonly UnityObjectRef<T> Reference;

        public GameObjectViewComponent(UnityObjectRef<T> reference) => Reference = reference;

        public T Value => Reference.Value;
    }
}

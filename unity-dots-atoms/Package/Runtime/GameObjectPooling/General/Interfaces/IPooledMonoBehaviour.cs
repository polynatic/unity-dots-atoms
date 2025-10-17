namespace DotsAtoms.GameObjectPooling.Interfaces
{
    /// <summary>
    /// Add to all MonoBehaviours that require some additional logic when the object is being pooled, like for example
    /// resetting some values to their original state.
    /// </summary>
    public interface IPooledMonoBehaviour
    {
        /// <summary>
        /// Called when the GameObject, that has this MonoBehaviour, was returned to the GameObjectPool.
        /// </summary>
        public void OnReturnedToPool();
    }
}

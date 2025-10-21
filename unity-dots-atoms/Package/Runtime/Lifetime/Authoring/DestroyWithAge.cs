using Unity.Entities;
using UnityEngine;

namespace DotsAtoms.Lifetime.Authoring
{
    public class DestroyWithAge : MonoBehaviour
    {
        [SerializeField, Min(0)] private double Age = 1;

        private class Baker : Baker<DestroyWithAge>
        {
            public override void Bake(DestroyWithAge authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent<Data.Lifetime.CreatedAt>(entity);
                SetComponentEnabled<Data.Lifetime.CreatedAt>(entity, false);
                AddComponent(entity, new Data.Lifetime.DestroyWithAge(authoring.Age));
            }
        }
    }
}

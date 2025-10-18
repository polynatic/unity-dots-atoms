using System.Diagnostics.Contracts;
using DotsAtoms.GameObjectViews.Mono;
using Unity.Entities;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Data
{
    public partial struct GameObjectView
    {
        public struct Prefab : IComponentData
        {
            private UnityObjectRef<GameObject> Reference;

            // ReSharper disable once InconsistentNaming
            public readonly bool HasRigidBody;

            public Prefab(GameObject gameObject)
            {
                Reference = new() { Value = gameObject };
                HasRigidBody = gameObject.GetComponent<Rigidbody>();
            }

            [Pure]
            public GameObjectView Instantiate(GameObjectViewContext context)
            {
                var view = InstantiateInternal(context);
                return new() {
                    GameObjectRef = view.gameObject,
                    GameObjectViewRef = view,
                };
            }

            private Mono.GameObjectView InstantiateInternal(GameObjectViewContext context)
            {
                var prefab = Reference.Value.gameObject;
                var view = context.InstantiateViewInternal(prefab);
#if UNITY_EDITOR
                view.gameObject.name = prefab.name;
#endif
                return view;
            }
        }
    }
}

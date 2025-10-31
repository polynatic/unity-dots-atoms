using System.Diagnostics.Contracts;
using DotsAtoms.GameObjectViews.Mono;
using Unity.Collections;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using Hash128 = UnityEngine.Hash128;

namespace DotsAtoms.GameObjectViews.Data
{
    public partial struct GameObjectView
    {
        public struct Prefab : IComponentData
        {
            private UnityObjectRef<GameObject> Reference;
            private Hash128 ReferenceGUID;

            public readonly bool HasRigidBody;

            internal Prefab(GameObject gameObject)
            {
                Reference = new() { Value = gameObject };
                HasRigidBody = gameObject.GetComponent<Rigidbody>();

#if UNITY_EDITOR
                var path = AssetDatabase.GetAssetPath(gameObject);
                var guid = AssetDatabase.GUIDFromAssetPath(path);
                ReferenceGUID = Hash128.Parse(guid.ToString());
#else
                ReferenceGUID = default;
#endif
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
                var view = context.InstantiateViewWithGuid(prefab, ReferenceGUID);
#if UNITY_EDITOR
                view.gameObject.name = prefab.name;
#endif
                return view;
            }
        }
    }
}

using System.Diagnostics.Contracts;
using DotsAtoms.GameObjectViews.Mono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Jobs;

namespace DotsAtoms.GameObjectViews.Data
{
    public struct GameObjectView : ICleanupComponentData
    {
        private UnityObjectRef<GameObject> GameObjectRef;
        private UnityObjectRef<Mono.GameObjectView> GameObjectViewRef;

        public GameObject GameObject => GameObjectRef.Value.gameObject;
        public Mono.GameObjectView View => GameObjectViewRef.Value;


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
                var gameObject = InstantiateInternal(context);
                return new() {
                    GameObjectRef = gameObject,
                    GameObjectViewRef = gameObject.GetComponent<Mono.GameObjectView>(),
                };
            }

            private GameObject InstantiateInternal(GameObjectViewContext context)
            {
                var prefab = Reference.Value.gameObject;
                var gameObject = context.InstantiatePrefab(prefab);
#if UNITY_EDITOR
                gameObject.name = prefab.name;
#endif
                return gameObject;
            }
        }

        /// <summary>
        /// As long as this component is on an entity with a GameObjectView, the GameObjectView will not be cleaned up.
        /// </summary>
        public struct IsAlive : IComponentData { }

        public struct Singleton : IComponentData
        {
            public TransformAccessArray Transforms;

            public NativeList<Entity> Entities;
            public UnityObjectRef<GameObjectViewContext> Context;

            /// <summary>
            /// This map contains all references to views with RigidBodies and the entities from where they update the velocity from.
            /// </summary>
            public NativeHashMap<UnityObjectRef<Rigidbody>, Entity> RigidBodies;
        }
    }
}

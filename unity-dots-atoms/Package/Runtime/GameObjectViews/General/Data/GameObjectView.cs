using System.Diagnostics.Contracts;
using DotsAtoms.GameObjectViews.Mono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Jobs;

namespace DotsAtoms.GameObjectViews.Data
{
    public partial struct GameObjectView : ICleanupComponentData
    {
        private UnityObjectRef<GameObject> GameObjectRef;
        private UnityObjectRef<Mono.GameObjectView> GameObjectViewRef;

        public GameObject GameObject => GameObjectRef.Value.gameObject;
        public Mono.GameObjectView View => GameObjectViewRef.Value;

        /// <summary>
        /// Enabled on the frame the GameObjectView has been attached to the entity.
        /// </summary>
        public struct OnViewAttached : IComponentData, IEnableableComponent { }

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
            /// This map contains all references to views with RigidBodies and the entities from where they update
            /// the velocity from.
            /// </summary>
            public NativeHashMap<UnityObjectRef<Rigidbody>, Entity> RigidBodies;
        }
    }
}

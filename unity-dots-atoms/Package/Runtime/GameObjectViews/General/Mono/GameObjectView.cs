using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DotsAtoms.GameObjectViews.Interfaces;
using Unity.Entities;
using UnityEngine;

namespace DotsAtoms.GameObjectViews.Mono
{
    /// <summary>
    /// Designates this game object as a GameObjectView for an entity. Add this to a GameObject that should be used
    /// as a prefab and then assign this prefab to the GameObjectViewPrefab of a baked game object.
    /// </summary>
    public class GameObjectView : MonoBehaviour
    {
        private IGameObjectView[] ViewComponents = { };


        /// <summary>
        /// The context that instantiated this view.
        /// </summary>
        internal GameObjectViewContext Context;

        private void Awake()
        {
            ViewComponents = GetComponentsInChildren<IGameObjectView>(includeInactive: true);
        }


        public void OnViewAttached(EntityManager entityManager, in Entity entity, EntityCommandBuffer commands)
        {
            foreach (var view in ViewComponents) {
                view.OnViewAttached(entityManager, entity, commands);
            }
        }

        public void OnViewDetached(EntityManager entityManager, in Entity entity, EntityCommandBuffer commands)
        {
            var tasks = new List<UniTask>();
            foreach (var view in ViewComponents) {
                view.OnViewDetached(entityManager, entity, commands);
                tasks.Add(view.OnViewWillDestroy());
            }

            DestroyAfterTasks(tasks).Forget();
        }

        private async UniTaskVoid DestroyAfterTasks(List<UniTask> tasks)
        {
            await tasks;
            Context.DestroyView(this);
        }
    }
}

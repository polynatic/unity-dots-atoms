using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DotsAtoms.GameObjectPooling.Interfaces;
using DotsAtoms.GameObjectViews.Interfaces;
using UnityEngine;
using static Cysharp.Threading.Tasks.UniTask;

namespace DotsAtoms.GameObjectViews.Mono
{
    public class GameObjectViewDestroyAwaitTrails : MonoBehaviour, IGameObjectView, IPooledMonoBehaviour
    {
        private TrailRenderer[] Trails;

        private void Awake()
        {
            Trails = GetComponentsInChildren<TrailRenderer>();
        }

        public async UniTask OnViewWillDestroy()
        {
            var tasks = new List<UniTask>(Trails.Length);

            foreach (var trail in Trails) {
                trail.emitting = false;

                if (trail.positionCount > 0) {
                    tasks.Add(WaitForTrailToFinish(trail));
                }
            }

            if (tasks.Count > 0) {
                await tasks;
            }
        }

        private async UniTask WaitForTrailToFinish(TrailRenderer trail)
        {
            while (trail.positionCount > 0) {
                await NextFrame(cancellationToken: destroyCancellationToken);
            }
        }

        public void OnReturnedToPool()
        {
            foreach (var trail in Trails) {
                trail.emitting = true;
            }
        }
    }
}

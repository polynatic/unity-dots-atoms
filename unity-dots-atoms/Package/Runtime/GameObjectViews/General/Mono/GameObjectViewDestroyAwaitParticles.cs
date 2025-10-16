using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DotsAtoms.GameObjectViews.Interfaces;
using Unity.Entities;
using UnityEngine;
using static Cysharp.Threading.Tasks.UniTask;

namespace DotsAtoms.GameObjectViews.Mono
{
    public class GameObjectViewDestroyAwaitParticles : MonoBehaviour, IGameObjectView
    {
        private ParticleSystem[] ParticleSystems;

        private void Awake()
        {
            ParticleSystems = GetComponentsInChildren<ParticleSystem>();
            var subEmitters = new HashSet<ParticleSystem>();

            // filter all particle systems that are sub emitters
            foreach (var system in ParticleSystems) {
                var subEmittersModule = system.subEmitters;

                if (!subEmittersModule.enabled) continue;

                for (var i = 0; i < subEmittersModule.subEmittersCount; i++) {
                    var subEmitter = subEmittersModule.GetSubEmitterSystem(i);
                    if (!subEmitter) continue;

                    subEmitters.Add(subEmitter);
                }
            }

            if (subEmitters.Count == 0) return;

            ParticleSystems = ParticleSystems.Except(subEmitters).ToArray();
        }

        public void OnViewAttached(EntityManager entityManager, in Entity entity, EntityCommandBuffer commands) { }
        public void OnViewDetached(EntityManager entityManager, in Entity entity, EntityCommandBuffer commands) { }

        public async UniTask OnViewWillDestroy()
        {
            var tasks = new List<UniTask>(ParticleSystems.Length);

            foreach (var system in ParticleSystems) {
                system.Stop();

                if (system.IsAlive(withChildren: true)) {
                    tasks.Add(WaitForParticleSystemToFinish(system));
                }
            }

            if (tasks.Count > 0) {
                await tasks;
            }
        }

        private async UniTask WaitForParticleSystemToFinish(ParticleSystem system)
        {
            while (system.IsAlive(withChildren: true)) {
                await NextFrame(cancellationToken: destroyCancellationToken);
            }
        }
    }
}

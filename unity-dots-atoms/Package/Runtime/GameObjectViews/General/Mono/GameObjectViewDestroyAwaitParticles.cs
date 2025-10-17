using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DotsAtoms.GameObjectPooling.Interfaces;
using DotsAtoms.GameObjectViews.Interfaces;
using Unity.Entities;
using UnityEngine;
using static Cysharp.Threading.Tasks.UniTask;

namespace DotsAtoms.GameObjectViews.Mono
{
    public class GameObjectViewDestroyAwaitParticles : MonoBehaviour, IGameObjectView, IPooledMonoBehaviour
    {
        private ParticleSystem[] ParticleSystems;
        private bool[] PlayOnAwake;

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

            if (subEmitters.Count > 0) {
                ParticleSystems = ParticleSystems.Except(subEmitters).ToArray();
            }

            // remember all play on awake settings to restore play state after pooling
            PlayOnAwake = new bool[ParticleSystems.Length];

            for (var i = 0; i < ParticleSystems.Length; i++) {
                PlayOnAwake[i] = ParticleSystems[i].main.playOnAwake;
            }
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

        public void OnReturnedToPool()
        {
            for (var i = 0; i < ParticleSystems.Length; i++) {
                if (!PlayOnAwake[i]) continue;

                ParticleSystems[i].Play();
            }
        }
    }
}

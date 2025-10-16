using DotsAtoms.GameObjectViews.Mono;
using UnityEngine;
using Zenject;

namespace DotsAtoms.GameObjectViews.Zenject.Mono
{
    public class GameObjectViewContextZenject : GameObjectViewContext
    {
        [Inject] private DiContainer DiContainer;

        public override GameObject InstantiateView(GameObject prefab) => DiContainer.InstantiatePrefab(prefab);
    }
}

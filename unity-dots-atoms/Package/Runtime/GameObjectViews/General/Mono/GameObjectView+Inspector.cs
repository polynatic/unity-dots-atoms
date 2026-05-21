#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace DotsAtoms.GameObjectViews.Mono
{
    public partial class GameObjectView
    {
        [CustomEditor(typeof(GameObjectView))]
        public class Inspector : Editor
        {
            internal const float PrimaryFontSize = 18;
            internal const float SecondaryFontSize = 13;

            public override VisualElement CreateInspectorGUI()
            {
                var inspector = new VisualElement();

                var label = new Label("Game Object View");
                label.style.fontSize = PrimaryFontSize;
                label.style.color = new Color(1f, 0.50f, 0f);
                inspector.Add(label);

                return inspector;
            }
        }
    }
}

#endif

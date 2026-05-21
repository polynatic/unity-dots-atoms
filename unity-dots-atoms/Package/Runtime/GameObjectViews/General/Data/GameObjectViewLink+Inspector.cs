using System;

#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
#endif


namespace DotsAtoms.GameObjectViews.Data
{
#if UNITY_EDITOR

    public partial class GameObjectViewLink<T>
    {
        [RequireComponent(typeof(Mono.GameObjectView))]
        public partial class FromScene
        {
            public class DefaultInspector : Editor
            {
                public override VisualElement CreateInspectorGUI()
                {
                    var myInspector = new VisualElement();

                    var objectInScene = FindAnyObjectByType(typeof(T));
                    var isFoundInCurrentScene = objectInScene != null;

                    var group = new VisualElement();
                    group.style.flexDirection = FlexDirection.Row;

                    var label = new Label("Game Object Link [");
                    label.style.fontSize = Mono.GameObjectView.Inspector.PrimaryFontSize;
                    label.style.color = new Color(1f, 0.50f, 0f);
                    group.Add(label);

                    label = new(typeof(T).Name);
                    label.style.fontSize = Mono.GameObjectView.Inspector.PrimaryFontSize;
                    label.style.color = isFoundInCurrentScene ? new Color(0.65f, 1f, 0f) : new(1f, 0.25f, 0.28f);
                    group.Add(label);

                    label = new("]");
                    label.style.fontSize = Mono.GameObjectView.Inspector.PrimaryFontSize;
                    label.style.color = new Color(1f, 0.50f, 0f);
                    group.Add(label);

                    myInspector.Add(group);


                    label = isFoundInCurrentScene
                        ? new("Is found in current scene hierarchy.")
                        : new("Is NOT found in current scene hierarchy.");
                    label.style.fontSize = Mono.GameObjectView.Inspector.SecondaryFontSize;
                    myInspector.Add(label);


                    if (isFoundInCurrentScene) {
                        myInspector.AddManipulator(new Clickable(() => EditorGUIUtility.PingObject(objectInScene)));
                    }

                    return myInspector;
                }
            }
        }
    }

    public class LinkInspector : CustomEditor
    {
        public LinkInspector(Type inspectedType) : base(inspectedType) { }
    }

#else
    public partial class GameObjectViewLink<T>
    {
        public partial class FromScene
        {
            public class DefaultInspector { }
        }
    }

    public class LinkInspector : Attribute
    {
        public LinkInspector(Type inspectedType) { }
    }

#endif
}

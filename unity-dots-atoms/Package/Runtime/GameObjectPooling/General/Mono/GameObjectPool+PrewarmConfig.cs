using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.EditorGUI;
using Object = UnityEngine.Object;

namespace DotsAtoms.GameObjectPooling.Mono
{
    public partial class GameObjectPool
    {
        [SerializeField] private List<PrewarmConfig> PrewarmPrefabs;

        [Serializable]
        public struct PrewarmConfig
        {
            public Object Prefab;
            public int Count;
        }


#if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(PrewarmConfig))]
        private class PrewarmConfigDrawer : PropertyDrawer
        {
            private const int CountFieldWidth = 42;

            private static GUIStyle _RightAlignedNumberField;

            private static GUIStyle RightAlignedNumberField {
                get {
                    if (_RightAlignedNumberField != null) return _RightAlignedNumberField;

                    _RightAlignedNumberField = new GUIStyle(EditorStyles.numberField);
                    _RightAlignedNumberField.alignment = TextAnchor.MiddleRight;
                    return _RightAlignedNumberField;
                }
            }


            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                BeginProperty(position, label, property);

                position = PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                var originalIndentLevel = indentLevel;
                indentLevel = 0;

                var propertyPrefab = property.FindPropertyRelative(nameof(PrewarmConfig.Prefab));
                var isGameObject = propertyPrefab.objectReferenceValue is GameObject;
                var countWidth = isGameObject ? position.width - CountFieldWidth : position.width;


                // Prefab
                var newObject = ObjectField(
                    new(position.x, position.y, countWidth - 2, position.height),
                    propertyPrefab.objectReferenceValue,
                    typeof(Object),
                    false
                );

                if (newObject is null or GameObject or GameObjectPoolPrewarmConfig) {
                    propertyPrefab.objectReferenceValue = newObject;
                } else {
                    Debug.LogError("Prefab field only supports GameObject or GameObjectPoolPrewarmConfig");
                }

                // Count
                if (isGameObject) {
                    var propertyCount = property.FindPropertyRelative(nameof(PrewarmConfig.Count));
                    propertyCount.intValue = IntField(
                        new(position.x + countWidth + 2, position.y, CountFieldWidth - 2, position.height),
                        propertyCount.intValue,
                        RightAlignedNumberField
                    );
                }

                indentLevel = originalIndentLevel;
                EndProperty();
            }
        }
#endif
    }
}

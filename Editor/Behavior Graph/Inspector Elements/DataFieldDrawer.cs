using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    [CustomPropertyDrawer(typeof(DataField), true)]
    public class DataFieldDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("Value");

            if (valueProperty != null)
                EditorGUI.PropertyField(position, valueProperty, label, true);
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("Value");

            if (valueProperty != null)
                return EditorGUI.GetPropertyHeight(valueProperty);
            else
            {
                return 25;
            }
        }
    }
}

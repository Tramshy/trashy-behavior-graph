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

            EditorGUI.PropertyField(position, valueProperty, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("Value");

            return EditorGUI.GetPropertyHeight(valueProperty);
        }
    }
}

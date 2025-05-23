using System;
using System.Reflection;
using UnityEditor;

namespace BehaviorGraph.GraphEditor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(BehaviorGraphInspectSO), editorForChildClasses: true)]
    public class InspectorSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // Note: Can't draw fields here due to how editor scripts work.
            //serializedObject.Update();

            //SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            //if (scriptProperty != null)
            //{
            //    EditorGUI.BeginDisabledGroup(true);
            //    EditorGUILayout.PropertyField(scriptProperty);
            //    EditorGUI.EndDisabledGroup();
            //}

            //Type targetType = target.GetType();
            //FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            
            //foreach (FieldInfo field in fields)
            //{
            //    if ((field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(DataField<>)) || field.FieldType.IsEnum)
            //    {
            //        SerializedProperty property = serializedObject.FindProperty(field.Name);

            //        if (property != null)
            //            EditorGUILayout.PropertyField(property, true);
            //    }
            //}

            BehaviorGraphInspectSO behaviorGraphInspect = (BehaviorGraphInspectSO)target;

            if (behaviorGraphInspect.FieldOverrides.Count > 0)
                EditorGUILayout.HelpBox("Overridden values will display as the final value it had before end of runtime.\nEditing of overridden values will not have any impact", MessageType.Info);

            //serializedObject.ApplyModifiedProperties();

        }
    }
}

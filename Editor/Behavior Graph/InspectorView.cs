using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static BehaviorGraph.GraphEditor.BehaviorGraphView;

namespace BehaviorGraph.GraphEditor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public static BehaviorGraphInspectSO CurrentData { get; private set; }

        private static InspectorView _thisView;

        private static List<string> _currentLinkableFields = new List<string>();

        public InspectorView()
        {
            _thisView = this;

            CurrentData = null;
        }

        public void UpdateInspectorView(BehaviorGraphInspectSO viewUpdate)
        {
            Clear();

            _currentLinkableFields = new List<string>();

            if (viewUpdate == null)
                return;

            CurrentData = viewUpdate;

            Editor editor = Editor.CreateEditor(viewUpdate);

            var so = new SerializedObject(viewUpdate);
            InspectorElement.FillDefaultInspector(contentContainer, so, editor);
            contentContainer.Bind(so);
            // Adding the inspector like this is necessary for editor scripts to work on it; however, then fields won't split up as PropertyFields.
            // In custom editor script, simply don't draw any fields and this works great.
            IMGUIContainer container = new IMGUIContainer(() => editor.OnInspectorGUI());
            Add(container);

            foreach (var field in contentContainer.Children())
            {
                field.RegisterCallback<MouseUpEvent>(evt =>
                {
                    if (!(field is PropertyField))
                        return;

                    var dropData = DragAndDrop.GetGenericData("BlackboardField");
                    var trimmedName = field.name.Remove(0, 14);

                    if (dropData == null || !_currentLinkableFields.Contains(trimmedName))
                        return;

                    (dropData as BlackboardData).Field.RemoveFromClassList("field-highlight");
                    (dropData as BlackboardData).Field.RemoveFromClassList("field-remove-highlight");
                    var bindingData = (dropData as BlackboardData).FieldBinding;

                    for (int i = 0; i < viewUpdate.FieldOverrides.Count; i++)
                    {
                        if (viewUpdate.FieldOverrides[i].NodeFieldName == trimmedName)
                        {
                            if (viewUpdate.FieldOverrides[i].ComponentFieldOverrideName == bindingData.ComponentFieldOverrideName)
                            {
                                viewUpdate.FieldOverrides.RemoveAt(i);

                                DragAndDrop.AcceptDrag();
                                DragAndDrop.SetGenericData("BlackboardField", null);
                                UpdateAllowedLinks();

                                return;
                            }

                            viewUpdate.FieldOverrides.RemoveAt(i);

                            break;
                        }
                    }

                    viewUpdate.FieldOverrides.Add(new BehaviorGraphInspectSO.FieldBindingData()
                    {
                        ComponentFieldOverrideName = bindingData.ComponentFieldOverrideName,
                        NodeFieldName = trimmedName
                    });

                    DragAndDrop.AcceptDrag();
                    DragAndDrop.SetGenericData("BlackboardField", null);
                    UpdateAllowedLinks();
                });
            }

            UpdateAllowedLinks();
        }

        public static void UpdateAllowedLinks()
        {
            _currentLinkableFields = new List<string>();

            var dropData = DragAndDrop.GetGenericData("BlackboardField");

            if (_thisView == null || CurrentData == null)
                return;

            if (dropData == null)
            {
                foreach (var c in _thisView.Children())
                {
                    c.RemoveFromClassList("field-highlight");
                    c.RemoveFromClassList("field-remove-highlight");
                }

                return;
            }

            foreach (var field in CurrentData.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (field.Name == "Transitions" || field.Name == "FieldOverrides" ||
                    !field.FieldType.IsGenericType || field.FieldType.GetGenericTypeDefinition() != typeof(DataField<>))
                    continue;

                if (CanLinkFields(field.FieldType.GetField("Value").FieldType, (dropData as BlackboardData).FieldType))
                    _currentLinkableFields.Add(field.Name);
            }

            List<string> linkedFieldsToHighlightForRemoval = new List<string>();

            foreach (var overrideData in CurrentData.FieldOverrides)
            {
                if (overrideData.ComponentFieldOverrideName != (dropData as BlackboardData).FieldBinding.ComponentFieldOverrideName)
                    continue;

                linkedFieldsToHighlightForRemoval.Add(overrideData.NodeFieldName);
            }

            foreach (var c in _thisView.Children())
            {
                if (!(c is PropertyField))
                    continue;

                c.RemoveFromClassList("field-remove-highlight");

                // Remove 14 first characters to get rid of: PropertyField:.
                var trimmedName = c.name.Remove(0, 14);

                // Skip Script field.
                if (trimmedName == "m_Script")
                    continue;

                if (linkedFieldsToHighlightForRemoval.Contains(trimmedName))
                {
                    c.AddToClassList("field-remove-highlight");

                    continue;
                }

                if (_currentLinkableFields.Contains(trimmedName))
                    c.AddToClassList("field-highlight");
                else
                {
                    c.RemoveFromClassList("field-highlight");
                    c.RemoveFromClassList("field-remove-highlight");
                }
            }
        }

        public static bool CanLinkFields(Type fieldType, Type droppedType)
        {
            if (fieldType == null || droppedType == null)
                return false;

            return fieldType.IsAssignableFrom(droppedType);
        }
    }
}

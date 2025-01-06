using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorGraphInspectSO : ScriptableObject
    {
        [SerializeField] private string _uniqueID;

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_uniqueID))
            {
                _uniqueID = Guid.NewGuid().ToString();
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }

        public string UniqueID
        {
            get
            {
#if UNITY_EDITOR
                return _uniqueID;
#else
                throw new System.InvalidOperationException("UniqueID is editor-only and cannot be accessed at runtime.");
#endif
            }
        }

        // Would be great to add drag and drop feature instead of manually typing, but Unity hates making things easy.
        public FieldBindingData[] FieldOverrides = new FieldBindingData[0];

        public void SetUpFields(GameObject objectHoldingPanel, Type componentType)
        {
            if (!(objectHoldingPanel.GetComponent(componentType) is BehaviorData component))
                throw new Exception($"No BehaviorData component found on: {objectHoldingPanel.name}");

            foreach (FieldBindingData data in FieldOverrides)
            {
                var nodeField = GetType().GetField(data.NodeFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var componentField = componentType.GetField(data.ComponentFieldOverrideName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                try
                {
                    var value = componentField.GetValue(component);
                    var newData = value;
                    nodeField.SetValue(this, newData);
                    componentField.SetValue(component, newData);
                }
                catch
                {
                    Debug.LogError($"Value not able to be associated between: {nodeField} and {componentField}, for game object: {objectHoldingPanel.name}\nNode: {this.name}");

                    continue;
                }
            }
        }

        [Serializable]
        public class FieldBindingData
        {
            public string NodeFieldName;
            public string ComponentFieldOverrideName;
        }
    }
}

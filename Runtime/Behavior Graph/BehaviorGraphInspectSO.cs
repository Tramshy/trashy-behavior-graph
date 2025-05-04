using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorGraphInspectSO : ScriptableObject
    {
        [SerializeField, HideInInspector] private string _uniqueID;

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

        /// <summary>
        /// Do not set UniqueID, unless it is a part of the Deep Clone process.
        /// </summary>
        public string UniqueID { get => _uniqueID; set => _uniqueID = value; }

        [HideInInspector] public List<FieldBindingData> FieldOverrides = new List<FieldBindingData>();

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
                    Debug.LogError($"Value not able to be associated between: {nodeField} and {componentField}, for game object: {objectHoldingPanel.name}\nNode/Transition: {this.name}");

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

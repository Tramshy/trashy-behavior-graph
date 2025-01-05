using System;
using System.Reflection;
using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorGraphInspectSO : ScriptableObject
    {
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

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    public class NodeInstancesSerializer : ScriptableObject
    {
        [HideInInspector, SerializeField] public NodeInstanceManager NodeInstances;

        public void SetUpNodeInstances()
        {
            NodeInstances = new NodeInstanceManager();
        }

        public void ClearEmptyInstances()
        {
            List<NodeInstanceManager.Instance> toRemove = new List<NodeInstanceManager.Instance>();

            NodeInstances.instances.ForEach((i) =>
            {
                if (i.InspectObject == null)
                    toRemove.Add(i);
            });

            toRemove.ForEach((r) => NodeInstances.instances.Remove(r));
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
    }
}

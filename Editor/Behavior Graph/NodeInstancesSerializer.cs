using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    public class NodeInstancesSerializer : ScriptableObject
    {
        /*[HideInInspector]*/ public NodeInstanceManager NodeInstances;

#if UNITY_EDITOR
        public void SetUpNodeInstances()
        {
            NodeInstances = new NodeInstanceManager();
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}

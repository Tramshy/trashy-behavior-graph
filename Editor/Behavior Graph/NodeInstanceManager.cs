using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    [Serializable]
    public class NodeInstanceManager
    {
        [SerializeField] internal List<Instance> instances = new List<Instance>();

        [Serializable]
        internal class Instance
        {
            public Instance(string id, BehaviorGraphInspectSO inspect)
            {
                ID = id;
                InspectObject = inspect;
            }

            public string ID;
            public BehaviorGraphInspectSO InspectObject;
        }

        public void AddToInstances(BehaviorGraphInspectSO node, string uniqueID)
        {
            instances.Add(new Instance(uniqueID, node));
        }

        public void RemoveFromInstances(string id)
        {
            Instance toRemove = null;

            foreach (var instance in instances)
            {
                if (instance.ID == id)
                {
                    toRemove = instance;

                    break;
                }
            }

            if (toRemove != null)
                instances.Remove(toRemove);
        }

        public BehaviorGraphInspectSO GetNodeInstance(string id)
        {
            BehaviorGraphInspectSO toReturn = null;

            foreach (var instance in instances)
            {
                if (instance.ID == id)
                {
                    toReturn = instance.InspectObject;

                    break;
                }
            }

            return toReturn;
        }
    }
}

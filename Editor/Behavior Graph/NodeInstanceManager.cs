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
        [SerializeField] private List<Instance> _instances = new List<Instance>();

        [Serializable]
        private class Instance
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
            _instances.Add(new Instance(uniqueID, node));
        }

        public void RemoveFromInstances(string id)
        {
            Instance toRemove = null;

            foreach (var instance in _instances)
            {
                if (instance.ID == id)
                    toRemove = instance;
            }

            if (toRemove != null)
                _instances.Remove(toRemove);
        }

        public BehaviorGraphInspectSO GetNodeInstance(string id)
        {
            BehaviorGraphInspectSO toReturn = null;

            foreach (var instance in _instances)
            {
                if (instance.ID == id)
                    toReturn = instance.InspectObject;
            }

            return toReturn;
        }
    }
}

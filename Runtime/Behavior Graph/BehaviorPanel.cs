using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    [CreateAssetMenu(fileName = "New Panel", menuName = "Behavior Graph/Behavior Panel")]
    public class BehaviorPanel : ScriptableObject
    {
        [HideInInspector] public List<Node> PanelNodes = new List<Node>();
        [HideInInspector] public Node StartingNode;

        [HideInInspector] public string Identifier = "";
        [SerializeField] private string _dataComponentName;

        [HideInInspector, NonSerialized] public bool HasLoaded = false; 

        private string _previousName;
        private Type _component;
        [Tooltip("Inherits: BehaviorData")]
        public Type MonoBehaviorDataComponent
        {
            get
            {
                if (_previousName == _dataComponentName && _component != null)
                    return _component;

                if (_dataComponentName == "")
                    return null;

                string search = _dataComponentName.Contains(",Assembly-CSharp") ? _dataComponentName : _dataComponentName + ",Assembly-CSharp";
                Type t = Type.GetType(search);

                if (t == null)
                    throw new Exception("DataComponentName does not match any types");

                if (!typeof(BehaviorData).IsAssignableFrom(t))
                    throw new Exception("DataComponentName does not match the name of any types, which inherit BehaviorData");

                _previousName = _dataComponentName;
                _component = t;

                return t;
            }
        }

        public void SetIdentifierCallbackReference(string reference)
        {
            if (Identifier != "")
                Debug.LogWarning($"GUID reference of {name} (ID: {GetInstanceID()}), has updated.\nPrevious GUID reference was: {Identifier}");

            Identifier = reference;
        }
    }
}


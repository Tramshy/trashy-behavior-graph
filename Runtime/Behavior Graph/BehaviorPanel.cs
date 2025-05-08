using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BehaviorGraph
{
    [CreateAssetMenu(fileName = "New Panel", menuName = "Behavior Graph/Behavior Panel")]
    public class BehaviorPanel : ScriptableObject
    {
        [HideInInspector] public List<Node> PanelNodes = new List<Node>();
        [HideInInspector] public Node StartingNode;

#if UNITY_EDITOR
        [SerializeField] private MonoScript _behaviorDataComponent;
#endif

        [HideInInspector] public string Identifier = "";
        [SerializeField, HideInInspector] private string _dataComponentName = "";
        public string DataComponentName { get => _dataComponentName; set => _dataComponentName = value; }

        [HideInInspector, NonSerialized] public bool HasLoaded = false; 

        private string _previousName;
        private Type _component;
        [Tooltip("Inherits: BehaviorData")]
        public Type MonoBehaviorDataComponent
        {
            get
            {
                if (_previousName == DataComponentName && _component != null)
                    return _component;

                if (DataComponentName == "")
                    return null;

                string search = DataComponentName.Contains(",Assembly-CSharp") ? DataComponentName : DataComponentName + ",Assembly-CSharp";
                Type t = Type.GetType(search);

                if (t == null)
                    throw new Exception("DataComponentName does not match any types");

                if (!typeof(BehaviorData).IsAssignableFrom(t))
                    throw new Exception("DataComponentName does not match the name of any types, which inherit BehaviorData");

                _previousName = DataComponentName;
                _component = t;

                return t;
            }
        }

#if UNITY_EDITOR
        public void SetBehaviorData()
        {
            if (_behaviorDataComponent == null)
            {
                _dataComponentName = "";

                Debug.Log("Behavior data unlinked.");

                return;
            }

            Type scriptClass = _behaviorDataComponent.GetClass();

            if (scriptClass == null || !typeof(BehaviorData).IsAssignableFrom(scriptClass))
                Debug.LogError("Script not linked!\nMake sure the script inherits from BehaviorData.");
            else
            {
                _dataComponentName = scriptClass.Name;

                Debug.Log("Linked successfully!");
            }
        }
#endif

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            HasLoaded = false;
        }

        public void SetIdentifierCallbackReference(string reference)
        {
            if (Identifier != "")
                Debug.LogWarning($"GUID reference of {name} (ID: {GetInstanceID()}), has updated.\nPrevious GUID reference was: {Identifier}");

            Identifier = reference;
        }

        /// <summary>
        /// Gets a Node from this Panel, based on its name.
        /// </summary>
        /// <param name="nodeName">The name of the Node. If the name is not unique, this method gets the first Node found in the list which uses that name.</param>
        /// <returns>Returns null if no Nodes with this name are found.</returns>
        public Node GetNode(string nodeName)
        {
            if (nodeName.Length == 0)
                return null;

            for (int i = 0; i < PanelNodes.Count; i++)
            {
                if (PanelNodes[i].GetNodeName() == nodeName)
                    return PanelNodes[i];
            }

            return null;
        }
    }
}


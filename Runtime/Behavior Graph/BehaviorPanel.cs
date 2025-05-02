using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BehaviorGraph
{
    [CreateAssetMenu(fileName = "New Panel", menuName = "Behavior Graph/Behavior Panel")]
    public class BehaviorPanel : ScriptableObject
    {
        [HideInInspector] public List<Node> PanelNodes = new List<Node>();
        [HideInInspector] public Node StartingNode;

        [HideInInspector] public string Identifier = "";
        [SerializeField] private string _dataComponentName;
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
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

// Use the scriptable objects for saving, it's way better, but too late now ig.
namespace BehaviorGraph.GraphEditor
{
    public class BehaviorGraphEditor : EditorWindow
    {
        [SerializeField] private VisualTreeAsset m_VisualTreeAsset = default;
        public static EditorWindow WND;
        private MiniMap _miniMap;

        public static BehaviorPanel CurrentBehaviorPanel { get; private set; }
        public static BehaviorGraphSaveData CurrentData { get; private set; }

        public static BehaviorGraphView ThisGraphView;
        public static InspectorView ThisInspectorView;

        private static NodeInstancesSerializer _serializer;

        [MenuItem("Window/Behavior Graph Editor")]
        public static void OpenWindow()
        {
            WND = GetWindow<BehaviorGraphEditor>();

            BehaviorGraphNode.OnNodeSelected = InspectorSelectionUpdate;
            BehaviorGraphEdge.OnEdgeSelected = InspectorSelectionUpdate;

            SerializeDataInitialization.CreateAssetIfNeeded();

            UpdateGraphViewData();
        }

        private void OnEnable()
        {
            _serializer = Resources.Load<NodeInstancesSerializer>("Instance Serializer");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            m_VisualTreeAsset.CloneTree(root);

            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.tramshy.trashy-behavior-graph/Editor/Behavior Graph/BehaviorGraphEditor.uss");
            root.styleSheets.Add(styleSheet);

            ThisGraphView = root.Q<BehaviorGraphView>();
            ThisInspectorView = root.Q<InspectorView>();

            _miniMap = new MiniMap();
            _miniMap.SetPosition(new Rect(20, 35, 200, 150));
            ThisGraphView.Add(_miniMap);
        }

        private void OnGUI()
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.Backspace)
                {
                    if (ThisGraphView == null || ThisGraphView.selection.Count <= 0)
                        return;

                    ThisGraphView.DeleteSelection();
                    Event.current.Use();
                }

                if (e.control && e.keyCode == KeyCode.S)
                    SaveNodes();
            }
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            System.Object obj = EditorUtility.InstanceIDToObject(instanceID);

            BehaviorPanel selected = obj as BehaviorPanel;

            if (selected != null)
            {
                OpenWindow();

                return true;
            }

            return false;
        }

        private void OnSelectionChange()
        {
            SaveNodes();
            ClearGraphView();
            UpdateGraphViewData();
            Repaint();

            _miniMap.UpdatePresenterPosition();
        }

        private void ClearGraphView()
        {
            ThisGraphView.ClearView();
        }

        private static void UpdateGraphViewData()
        {
            BehaviorPanel selected = Selection.activeObject as BehaviorPanel;
            CurrentBehaviorPanel = selected == null ? null : selected;

            ThisGraphView.SetUpView();

            InspectorSelectionUpdate(null);

            if (selected != null)
            {
                WND.titleContent = new GUIContent(selected.name);

                // This creates one empty node in the list for some reason, it doesn't seem to cause any issues and can't be interacted with.
                if (selected.Identifier.Length != 36)
                {
                    DebugNode newStart = (DebugNode)CreateNewDataNode(typeof(DebugNode));
                    newStart.Message.Value = "Hello World!";

                    BehaviorGraphNode visualNode = new BehaviorGraphNode(newStart, newStart.GetType().Name, true, new List<BehaviorGraphPort>(), new List<BehaviorGraphConnection>());
                    visualNode.SetPosition(new Rect(150, 280, 1, 1));
                    visualNode.name = newStart.GetType().Name + " - Element";

                    BehaviorGraphNode[] nodes = { visualNode };

                    CurrentData = new BehaviorGraphSaveData(nodes);

                    selected.PanelNodes.Add(newStart);
                    selected.StartingNode = newStart;
                    selected.SetIdentifierCallbackReference(CurrentData.Identifier);

                    ThisGraphView.LoadNodesVisual(CurrentData);

                    SaveNodes();

                    return;
                }

                LoadNodes();

                return;
            }

            WND.titleContent = new GUIContent("Behavior Graph Editor");
        }

        public static void InspectorSelectionUpdate(BehaviorGraphInspectSO objectUpdate)
        {
            ThisInspectorView.UpdateInspectorView(objectUpdate);
        }

        public static Node CreateNewDataNode(Type type)
        {
            var instance = CreateInstance(type) as Node;
            instance.name = type.Name;
            AssetDatabase.AddObjectToAsset(instance, CurrentBehaviorPanel);
            AssetDatabase.SaveAssets();

            _serializer.NodeInstances.AddToInstances(instance, instance.UniqueID);

            return instance;
        }

        public static NodeTransitionObject CreateNewDataTransition(Type transition, Node dataNodeCallingTransition)
        {
            var instance = CreateInstance(transition) as NodeTransitionObject;
            instance.name = transition.Name;
            instance.NodeCallingTransition = dataNodeCallingTransition;
            AssetDatabase.AddObjectToAsset(instance, CurrentBehaviorPanel);
            AssetDatabase.SaveAssets();

            _serializer.NodeInstances.AddToInstances(instance, instance.UniqueID);

            return instance;
        }
        public static void DeleteDataNode(Node node)
        {
            CurrentBehaviorPanel.PanelNodes.Remove(node);
            _serializer.NodeInstances.RemoveFromInstances(node.UniqueID);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public static void DeleteDataTransition(NodeTransitionObject transition)
        {
            _serializer.NodeInstances.RemoveFromInstances(transition.UniqueID);
            AssetDatabase.RemoveObjectFromAsset(transition);
            AssetDatabase.SaveAssets();
        }

        public static void UpdateStartNode(Node newStart)
        {
            if (!CurrentBehaviorPanel.PanelNodes.Contains(newStart))
                throw new Exception("Fatal Error.\nStarting node does not exist within list of nodes.");

            CurrentBehaviorPanel.StartingNode = newStart;
        }

        private static void SaveNodes()
        {
            if (CurrentBehaviorPanel == null || CurrentData == null)
                return;

            Rect pos = new Rect();

            _serializer.Save();

            foreach (BehaviorGraphNode node in CurrentData.Nodes)
            {
                pos = node.GetPosition();
                node.XPosition = pos.x;
                node.YPosition = pos.y;

                node.Ports.Clear();
                node.LinkNodes.Clear();
                node.ThisNode.Transitions.Clear();

                int i = 0;

                foreach (var child in node.outputContainer.Children())
                {
                    BehaviorGraphPort p = child as BehaviorGraphPort;

                    if (p == null)
                        continue;

                    node.Ports.Add(p);

                    Edge[] temp = p.connections.ToArray();
                    // Will only ever have one edge since all BehaviorGraphPort have a capacity of single.
                    Edge connection = temp.Length > 0 ? temp[0] : null;

                    if (connection != null)
                        node.LinkNodes.Add(new BehaviorGraphConnection(i, (connection.input.node as BehaviorGraphNode).Identifier));

                    i++;
                }

                foreach (BehaviorGraphPort p in node.Ports)
                {
                    var transition = p.Transition;

                    Edge[] temp = p.connections.ToArray();
                    Node nextNode = temp.Length > 0 ? (temp[0].input.node as BehaviorGraphNode).ThisNode : null;

                    if (nextNode == null)
                        continue;

                    NodeTransition nodeTransition = new NodeTransition(transition, nextNode);
                    node.ThisNode.Transitions.Add(nodeTransition);
                }
            }

            GraphSave.SaveDataAsJSON(CurrentData, CurrentData.Identifier);

            foreach (Node node in CurrentBehaviorPanel.PanelNodes)
            {
                if (node == null)
                    continue;

                EditorUtility.SetDirty(node);

                node.Transitions.ForEach((t) => EditorUtility.SetDirty(t.TransitionCondition));
            }

            EditorUtility.SetDirty(CurrentBehaviorPanel);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void LoadNodes()
        {
            if (CurrentBehaviorPanel == null)
                return;

            CurrentData = GraphSave.LoadDataFromGUID(CurrentBehaviorPanel.Identifier);
            List<BehaviorGraphNode> newNodes = new List<BehaviorGraphNode>();

            Rect pos = new Rect();

            foreach (BehaviorGraphNode node in CurrentData.Nodes)
            {
                BehaviorGraphNode newNode = new BehaviorGraphNode(node.ThisNode, node.Name, node.IsStartNode, node.Ports, node.LinkNodes, node.Identifier);

                pos.Set(node.XPosition, node.YPosition, 1, 1);
                newNode.SetPosition(pos);
                newNodes.Add(newNode);
            }

            CurrentData.Nodes.ForEach((node) =>
            {
                foreach (BehaviorGraphConnection link in node.LinkNodes)
                {
                    newNodes.ForEach((n) =>
                    {
                        if (link.ConnectedPortIdentifierCallback == n.Identifier)
                            link.ConnectedPort = n;
                    });

                    if (link.ConnectedPort == null)
                        throw new Exception("Fatal Error.\nNo connected port identified with GUID.");
                }
            });

            CurrentData.Nodes = newNodes;

            ThisGraphView.LoadNodesVisual(CurrentData);
        }

        private void OnDisable()
        {
            SaveNodes();

            BehaviorGraphNode.OnNodeSelected -= InspectorSelectionUpdate;
            BehaviorGraphEdge.OnEdgeSelected -= InspectorSelectionUpdate;

            CurrentBehaviorPanel = null;
            CurrentData = null;
            _miniMap = null;
        }
    }
}

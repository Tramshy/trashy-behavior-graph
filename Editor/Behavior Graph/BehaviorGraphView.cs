using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using System.Linq;
using System.Data;
using System.Reflection;

namespace BehaviorGraph.GraphEditor
{
    public class BehaviorGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<BehaviorGraphView, UxmlTraits> { }

        public class BlackboardData
        {
            public BehaviorGraphInspectSO.FieldBindingData FieldBinding;
            public BehaviorGraphField Field;
            public Type FieldType;
        }

        public Blackboard GraphBlackboard;

        public BehaviorGraphView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.tramshy.trashy-behavior-graph/Editor/Behavior Graph/BehaviorGraphEditor.uss");
            styleSheets.Add(styleSheet);

            focusable = true;
            Focus();

            GraphBlackboard = new Blackboard(this) { title = "Variables" };
            GraphBlackboard.SetPosition(new Rect(10, 200, 200, 300));

            Add(GraphBlackboard);

            graphViewChanged = DeleteNodeVisual;
        }

        private static readonly Color _baseMiniMapElementColor = new Color(0.9f, 0.9f, 0.9f, 0.5f);
        private static readonly string _startName = "[Start] - ";

        private static NodeSearchWindow _searchWindow;

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where((endPort) => endPort.direction != startPort.direction && endPort.node != startPort.node).ToList();
        }

        public void ClearView()
        {
            graphViewChanged -= DeleteNodeVisual;
            DeleteElements(graphElements);
            graphViewChanged = DeleteNodeVisual;
        }

        public void SetUpView()
        {
            float height = BehaviorGraphEditor.WND.position.height;

            GraphBlackboard.SetPosition(new Rect(10, height - 310, 200, 300));
            GraphBlackboard.title = "Variables";
            GraphBlackboard.Clear();

            if (BehaviorGraphEditor.CurrentBehaviorPanel == null)
                return;

            var component = BehaviorGraphEditor.CurrentBehaviorPanel.MonoBehaviorDataComponent;

            if (component == null)
            {
                GraphBlackboard.title = "No Associated Data Found";
                GraphBlackboard.Add(new Label("Please add\nMonoBehaviorDataComponent to\nassociated BehaviorPanel\nthrough SO asset"));
                GraphBlackboard.addItemRequested = (b) => Debug.Log("Add MonoBehaviorDataComponent to Scriptable Object to see all available fields.");

                SetUpSearchWindow();

                return;
            }

            GraphBlackboard.addItemRequested = (b) => Debug.Log("Add field to MonoBehaviorDataComponent to see field in Blackboard, and for use in BehaviorGraph.");

            ScrollView scrollView = new ScrollView();
            scrollView.style.flexGrow = 1;
            scrollView.style.height = new Length(100, LengthUnit.Percent);

            foreach (var field in component.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (!(field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(DataField<>)))
                    continue;

                BehaviorGraphField f = new BehaviorGraphField() { text = field.Name };

                var fieldType = field.FieldType.GetField("Value").FieldType;
                f.typeText = fieldType.Name;

                f.RegisterCallback<MouseDownEvent>(evt =>
                {
                    if (InspectorView.CurrentData == null)
                        return;

                    evt.StopImmediatePropagation(); // STOP any bubbling that might grab the whole element.
                    evt.PreventDefault(); // Prevent default behavior like dragging, selecting, focusing.

                    var dropData = DragAndDrop.GetGenericData("BlackboardField");

                    if (dropData != null)
                        (dropData as BlackboardData).Field.RemoveFromClassList("field-highlight");

                    f.AddToClassList("field-highlight");

                    DragAndDrop.PrepareStartDrag();
                    DragAndDrop.SetGenericData("BlackboardField", new BlackboardData()
                    {
                        FieldBinding = new BehaviorGraphInspectSO.FieldBindingData() { ComponentFieldOverrideName = field.Name },
                        Field = f,
                        FieldType = fieldType
                    });
                    DragAndDrop.StartDrag("Dragging BlackboardField");

                    InspectorView.UpdateAllowedLinks();
                });

                scrollView.Add(f);
            }

            GraphBlackboard.Add(scrollView);

            SetUpSearchWindow();
        }

        private void SetUpSearchWindow()
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Initialize();
            nodeCreationRequest = (c) => SearchWindow.Open(new SearchWindowContext(c.screenMousePosition), _searchWindow);
        }

        public void SetNodeVisualName(string name, BehaviorGraphNode node)
        {
            if (node.IsStartNode && !node.Name.Contains(_startName))
                name = _startName + name;

            node.Name = name;
            node.title = name;
        }

        private void SetUpVisualNode(BehaviorGraphNode node, string baseName)
        {
            var textField = new TextField()
            {
                name = "Rename",
                value = baseName
            };

            textField.RegisterValueChangedCallback((evt) => RenameNodeVisual(node, evt.newValue));
            Button renameButton = new Button(() => UpdateNodeVisualTitle(node)) { text = "Rename" };

            node.titleContainer.Add(textField);
            node.titleContainer.Add(renameButton);

            GenerateNewInPort(node);

            BehaviorGraphPort[] newPorts = new BehaviorGraphPort[node.Ports.Count];
            int i = 0;

            foreach (BehaviorGraphPort p in node.Ports)
            {
                newPorts[i] = RegenerateOutPort(node, p.Transition, p.TransitionDataGUID);

                i++;
            }

            for (int j = 0; j < newPorts.Length; j++)
            {
                node.Ports[j] = newPorts[j];
            }
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (BehaviorGraphEditor.CurrentBehaviorPanel == null)
                return;

            Vector2 screenPosition = BehaviorGraphEditor.WND.position.position;
            evt.menu.AppendAction("Create Nodes", (menuAction) => SearchWindow.Open(new SearchWindowContext(screenPosition + menuAction.eventInfo.mousePosition), _searchWindow));

            if (selection.Count > 0)
            {
                evt.menu.AppendAction("Delete", (menuAction) => DeleteSelection());

                if (selection.Count == 1)
                {
                    BehaviorGraphNode thisNode = selection[0] as BehaviorGraphNode;

                    evt.menu.AppendSeparator();

                    if (thisNode != null)
                    {
                        var transitions = TypeCache.GetTypesDerivedFrom<NodeTransitionObject>();

                        foreach (Type transition in transitions)
                        {
                            if (transition == typeof(NodeTransitionObject) || transition.IsAbstract)
                                continue;

                            bool shouldContinue = false;

                            foreach (BehaviorGraphPort p in thisNode.Ports)
                            {
                                if (p.Transition.GetType() == transition && !typeof(IAllowMultiTransitionElement).IsAssignableFrom(transition))
                                    shouldContinue = true;
                            }

                            if (shouldContinue)
                                continue;

                            string menuName = transition.Name;

                            if (typeof(IBaseBehaviorElement).IsAssignableFrom(transition))
                                menuName = $"Base Transitions/{menuName}";
                            else
                            {
                                menuName = $"Custom Transitions/{menuName}";
                            }

                            evt.menu.AppendAction(menuName, (menuAction) => GenerateNewOutPort(thisNode, transition));
                        }

                        evt.menu.AppendSeparator();

                        evt.menu.AppendAction("Remove Empty Output Ports", (menuAction) => RemoveEmptyPorts(thisNode));
                        evt.menu.AppendAction("Set as Starting Node", (menuAction) =>
                        {
                            UpdateStartNodeVisual(thisNode);
                            BehaviorGraphEditor.UpdateStartNode(thisNode.ThisNode);
                        });
                    }
                }
            }
        }

        public void UpdateStartNodeVisual(BehaviorGraphNode newStart)
        {
            foreach (BehaviorGraphNode n in BehaviorGraphEditor.CurrentData.Nodes)
            {
                if (n.IsStartNode)
                {
                    n.IsStartNode = false;
                    n.elementTypeColor = _baseMiniMapElementColor;
                    string newName = n.title.Substring(_startName.Length);

                    n.title = newName;
                    n.Name = newName;

                    break;
                }
            }

            newStart.IsStartNode = true;
            newStart.elementTypeColor = Color.yellow;

            SetNodeVisualName(newStart.Name, newStart);
        }

        private void UpdateNodeVisualTitle(BehaviorGraphNode thisNode)
        {
            SetNodeVisualName(thisNode.Name, thisNode);
        }

        private void RenameNodeVisual(BehaviorGraphNode thisNode, string newName)
        {
            thisNode.Name = newName;
        }

        public void AddNewNode(Type type, Rect spawnPos)
        {
            if (BehaviorGraphEditor.CurrentBehaviorPanel == null)
                throw new Exception("No behavior panel selected");

            var instance = BehaviorGraphEditor.CreateNewDataNode(type);
            var nodeToAdd = CreateNewVisualNode(instance, spawnPos);

            AddElement(nodeToAdd);
            
            // Data for behavior
            BehaviorGraphEditor.CurrentBehaviorPanel.PanelNodes.Add(instance);

            // Data for graph
            BehaviorGraphEditor.CurrentData.Nodes.Add(nodeToAdd);
        }

        private BehaviorGraphNode CreateNewVisualNode(Node node, Rect spawnPos)
        {
            var nodeToAdd = new BehaviorGraphNode(node, node.GetType().Name, false, new List<BehaviorGraphPort>(), new List<BehaviorGraphConnection>());
            nodeToAdd.SetPosition(spawnPos);
            nodeToAdd.name = node.GetType().Name + " - Element";

            SetUpVisualNode(nodeToAdd, nodeToAdd.Name);
            nodeToAdd.RefreshExpandedState();
            
            return nodeToAdd;
        }

        private GraphViewChange DeleteNodeVisual(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove == null)
                return graphViewChange;
            
            foreach (var e in graphViewChange.elementsToRemove)
            {
                if (e is BehaviorGraphNode n && n.ThisNode == InspectorView.CurrentData)
                {
                    BehaviorGraphEditor.InspectorSelectionUpdate(null);

                    break;
                }

                if (e is BehaviorGraphEdge d && d.ThisInspectSO == InspectorView.CurrentData)
                {
                    BehaviorGraphEditor.InspectorSelectionUpdate(null);

                    break;
                }
            }

            if (graphViewChange.elementsToRemove.OfType<BehaviorGraphNode>().Any())
            {
                if (!EditorUtility.DisplayDialog("Delete selected node?", "This action cannot be undone.", "Yes", "No"))
                {
                    graphViewChange.elementsToRemove.Clear();

                    return graphViewChange;
                }
            }

            BehaviorGraphNode startNode = null;

            foreach (var element in graphViewChange.elementsToRemove)
            {
                BehaviorGraphEdge e = element as BehaviorGraphEdge;
                
                if (e != null)
                {
                    List<BehaviorGraphConnection> toRemove = new List<BehaviorGraphConnection>();

                    (e.output.node as BehaviorGraphNode).LinkNodes.ForEach((n) =>
                    {
                        if (n.EdgeBetweenNodes == e)
                            toRemove.Add(n);
                    });

                    toRemove.ForEach((n) => (e.output.node as BehaviorGraphNode).LinkNodes.Remove(n));

                    //Debug.Log((e.output.node as BehaviorGraphNode).ThisNode.Transitions.Contains((e.output as BehaviorGraphPort).TransitionData) + "\n" + (e.output as BehaviorGraphPort).TransitionData);
                    (e.output.node as BehaviorGraphNode).ThisNode.RemoveFromTransitions((e.output as BehaviorGraphPort).TransitionDataGUID);

                    continue;
                }

                BehaviorGraphNode n = element as BehaviorGraphNode;

                if (n == null)
                    continue;

                if (n.IsStartNode)
                {
                    startNode = n;

                    Debug.LogWarning("Cannot delete starting node.");

                    continue;
                }

                n.Ports.ForEach((p) => BehaviorGraphEditor.DeleteDataTransition(p.Transition));
                n.Ports.Clear();

                BehaviorGraphEditor.DeleteDataNode(n.ThisNode);
                BehaviorGraphEditor.CurrentData.Nodes.Remove(n);
            }

            if (startNode != null)
                graphViewChange.elementsToRemove.Remove(startNode);

            return graphViewChange;
        }

        private Port GenerateNewOutPort(BehaviorGraphNode node, Type transition)
        {
            var instance = BehaviorGraphEditor.CreateNewDataTransition(transition, node.ThisNode);
            BehaviorGraphPort newOutPort = BehaviorGraphPort.Create(instance);
            node.Ports.Add(newOutPort);
            node.outputContainer.Add(newOutPort);
            node.RefreshPorts();

            return newOutPort;
        }

        private BehaviorGraphPort RegenerateOutPort(BehaviorGraphNode node, NodeTransitionObject transition, string guid)
        {
            BehaviorGraphPort newOutPort = BehaviorGraphPort.Create(transition, guid);
            node.outputContainer.Add(newOutPort);
            node.RefreshPorts();

            return newOutPort;
        }

        private Port GenerateNewInPort(BehaviorGraphNode node)
        {
            Port newInPort = node.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(NodeTransitionObject));
            newInPort.portName = "In";
            node.inputContainer.Add(newInPort);
            node.RefreshPorts();

            return newInPort;
        }

        private void RemoveEmptyPorts(BehaviorGraphNode node)
        {
            List<BehaviorGraphPort> toRemove = new List<BehaviorGraphPort>();

            foreach (var element in node.outputContainer.Children())
            {
                BehaviorGraphPort port = element as BehaviorGraphPort;

                if (port != null && !port.connected)
                    toRemove.Add(port);
            }

            foreach (BehaviorGraphPort p in toRemove)
            {
                node.outputContainer.Remove(p);
                node.Ports.Remove(p);

                BehaviorGraphEditor.DeleteDataTransition(p.Transition);

                AssetDatabase.RemoveObjectFromAsset(p.Transition);
                AssetDatabase.SaveAssets();
            }

            node.RefreshPorts();
        }

        public void LoadNodesVisual(BehaviorGraphSaveData data)
        {
            foreach (BehaviorGraphNode node in data.Nodes)
            {
                string newName = node.Name;

                if (node.IsStartNode)
                {
                    if (newName.StartsWith(_startName))
                        newName = node.Name.Substring(_startName.Length);

                    node.elementTypeColor = Color.yellow;

                    SetNodeVisualName(node.Name, node);
                }

                SetUpVisualNode(node, newName);
            }

            // Another loop to set up connections after Inputs have been created for all nodes.
            foreach (BehaviorGraphNode node in data.Nodes)
            {
                foreach (var connection in node.LinkNodes)
                {
                    Port connectionIn = (connection.ConnectedPort).inputContainer.Children().ToArray()[0] as Port;

                    if (connectionIn == null)
                    {
                        Debug.LogError("Fatal Error.\nError encountered when connecting ports from save. Either no Input port exists or multiple elements exist in Input Container. Attempting fix...");

                        connectionIn = GenerateNewInPort(connection.ConnectedPort);
                    }

                    BehaviorGraphEdge edge = (node.outputContainer.Children().ToArray()[connection.OutPortConnectedTo] as Port).ConnectTo<BehaviorGraphEdge>(connectionIn);
                    connection.EdgeBetweenNodes = edge;
                    AddElement(edge);
                }

                AddElement(node);
                node.RefreshExpandedState();
            }
        }
    }
}

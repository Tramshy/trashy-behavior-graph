using System;
using System.Collections.Generic;
using UnityEditor;

namespace BehaviorGraph.GraphEditor
{
    [Serializable]
    public class BehaviorGraphNode : UnityEditor.Experimental.GraphView.Node
    {
        public BehaviorGraphNode(Node node, string name, bool start, List<BehaviorGraphPort> ports, List<BehaviorGraphConnection> linkNodes, string guid = "")
        {
            ThisNode = node;

            XPosition = GetPosition().x;
            YPosition = GetPosition().y;

            Name = name;
            title = name;

            Identifier = guid.Length < 36 ? Guid.NewGuid().ToString() : guid;

            IsStartNode = start;

            Ports = ports;
            LinkNodes = linkNodes;

            GUID = node.UniqueID;
        }

        public Node ThisNode;
        public string GUID;

        public float XPosition, YPosition;

        public string Name;
        public string Identifier;

        public bool IsStartNode;

        public List<BehaviorGraphPort> Ports;
        public List<BehaviorGraphConnection> LinkNodes;

        [NonSerialized] public static Action<Node> OnNodeSelected;

        public override void OnSelected()
        {
            base.OnSelected();

            OnNodeSelected?.Invoke(ThisNode);
        }
    }
}

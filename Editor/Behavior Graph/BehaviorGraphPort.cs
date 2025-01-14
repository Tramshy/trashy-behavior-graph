using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

namespace BehaviorGraph.GraphEditor
{
    [Serializable]
    public class BehaviorGraphPort : Port
    {
        protected BehaviorGraphPort(Orientation portOrientation, Direction portDirection, Capacity portCapacity, Type type) : base(portOrientation, portDirection, portCapacity, type) { }

        public string TransitionDataGUID;

        public NodeTransitionObject Transition;
        public string GUID;

        public static BehaviorGraphPort Create(NodeTransitionObject transition, string data = "")
        {
            BehaviorGraphPort port = new BehaviorGraphPort(Orientation.Horizontal, Direction.Output, Capacity.Single, typeof(NodeTransitionObject))
            {
                name = transition.GetType().Name,
                portName = transition.GetType().Name
            };

            port.Transition = transition;
            port.TransitionDataGUID = data;

            port.m_EdgeConnector = new EdgeConnector<BehaviorGraphEdge>(new DefaultEdgeConnectorListener(port));
            port.AddManipulator(port.m_EdgeConnector);

            port.GUID = transition.UniqueID;

            return port;
        }
    }

    public class DefaultEdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphViewChange m_GraphViewChange;

        private List<Edge> m_EdgesToCreate;

        private List<GraphElement> m_EdgesToDelete;

        private BehaviorGraphPort _calling;

        public DefaultEdgeConnectorListener(BehaviorGraphPort calling)
        {
            m_EdgesToCreate = new List<Edge>();
            m_EdgesToDelete = new List<GraphElement>();
            m_GraphViewChange.edgesToCreate = m_EdgesToCreate;

            _calling = calling;
        }

        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
        }

        public void OnDrop(GraphView graphView, Edge edge)
        {
            m_EdgesToCreate.Clear();
            m_EdgesToCreate.Add(edge);
            m_EdgesToDelete.Clear();
            if (edge.input.capacity == Capacity.Single)
            {
                foreach (Edge connection in edge.input.connections)
                {
                    if (connection != edge)
                    {
                        m_EdgesToDelete.Add(connection);
                    }
                }
            }

            if (edge.output.capacity == Capacity.Single)
            {
                foreach (Edge connection2 in edge.output.connections)
                {
                    if (connection2 != edge)
                    {
                        m_EdgesToDelete.Add(connection2);
                    }
                }
            }

            if (m_EdgesToDelete.Count > 0)
            {
                graphView.DeleteElements(m_EdgesToDelete);
            }

            List<Edge> edgesToCreate = m_EdgesToCreate;
            if (graphView.graphViewChanged != null)
            {
                edgesToCreate = graphView.graphViewChanged(m_GraphViewChange).edgesToCreate;
            }

            foreach (Edge item in edgesToCreate)
            {
                graphView.AddElement(item);
                edge.input.Connect(item);
                edge.output.Connect(item);
            }

            Node thisNode = (_calling.node as BehaviorGraphNode).ThisNode, nextNode = (edge.input.node as BehaviorGraphNode).ThisNode;
            NodeTransitionObject transition = _calling.Transition;

            var temp = new NodeTransition(transition, nextNode);
            _calling.TransitionDataGUID = temp.GUID;
            thisNode.Transitions.Add(temp);
        }
    }
}

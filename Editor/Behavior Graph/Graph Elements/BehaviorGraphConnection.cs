using System;

namespace BehaviorGraph.GraphEditor
{
    [Serializable]
    public class BehaviorGraphConnection
    {
        public BehaviorGraphConnection(int portIndex, string connectedIdentifier)
        {
            OutPortConnectedTo = portIndex;
            ConnectedPortIdentifierCallback = connectedIdentifier;
        }

        public int OutPortConnectedTo;
        public string ConnectedPortIdentifierCallback;

        [NonSerialized] public BehaviorGraphNode ConnectedPort;
        [NonSerialized] public BehaviorGraphEdge EdgeBetweenNodes;
    }
}

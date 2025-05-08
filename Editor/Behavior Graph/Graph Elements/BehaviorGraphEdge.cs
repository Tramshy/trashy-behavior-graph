using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    public class BehaviorGraphEdge : Edge
    {
        [NonSerialized] public static Action<NodeTransitionObject> OnEdgeSelected;

        [NonSerialized] public BehaviorGraphInspectSO ThisInspectSO;

        public override void OnSelected()
        {
            base.OnSelected();

            BehaviorGraphPort inputPort = output as BehaviorGraphPort;

            if (inputPort == null)
            {
                Debug.LogWarning($"No {typeof(BehaviorGraphPort).Name} found connected to edge, this could cause problems.");

                return;
            }

            ThisInspectSO = inputPort.Transition;
            OnEdgeSelected?.Invoke(inputPort.Transition);
        }
    }
}

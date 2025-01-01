using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public abstract class Node : BehaviorGraphInspectSO
    {
        [HideInInspector] public List<NodeTransition> Transitions = new List<NodeTransition>();

        public enum Statuses { Success, Running, Failure }

        private Statuses _current = Statuses.Running;
        public Statuses CurrentStatus { get => _current; protected set => _current = value; }

        public abstract void OnNodeStart();

        public abstract void OnNodeUpdate();

        /// <summary>
        /// Called on Failure or success status to clean up.
        /// </summary>
        public abstract void ResetNode();
    }

    [Serializable]
    public class NodeTransition
    {
        public NodeTransition(NodeTransitionObject transitionObject, Node next)
        {
            TransitionCondition = transitionObject;
            NextInLine = next;
        }

        public NodeTransitionObject TransitionCondition;

        public Node NextInLine;
    }
}

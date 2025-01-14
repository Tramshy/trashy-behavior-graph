using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public abstract class Node : BehaviorGraphInspectSO
    {
        [HideInInspector, SerializeField] public List<NodeTransition> Transitions = new List<NodeTransition>();

        public virtual void RemoveFromTransitions(string guid)
        {
            NodeTransition toRemove = null;

            foreach (var t in Transitions)
            {
                if (t.GUID == guid)
                {
                    toRemove = t;

                    break;
                }
            }

            Transitions.Remove(toRemove);
        }

        public enum Statuses { Success, Running, Failure }

        [NonSerialized] private Statuses _current = Statuses.Running;
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
            GUID = Guid.NewGuid().ToString();

            TransitionCondition = transitionObject;
            TGUID = transitionObject.UniqueID;
            NextInLine = next;
            NGUID = next.UniqueID;
        }

        public string GUID;

        public NodeTransitionObject TransitionCondition;
        public string TGUID;

        public Node NextInLine;
        public string NGUID;
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using static System.TimeZoneInfo;

namespace BehaviorGraph
{
    public abstract class Node : BehaviorGraphInspectSO
    {
        [HideInInspector, SerializeField] public List<NodeTransition> Transitions = new List<NodeTransition>();
        [HideInInspector, NonSerialized] public List<NodeTransition> UpdateTransitions = new List<NodeTransition>();
        
        [HideInInspector, NonSerialized] public List<TriggerTransition> Triggers = new List<TriggerTransition>();

        /// <summary>
        /// The name of this Node. This is set when you edit a name in the graph view.
        /// </summary>
        [HideInInspector, SerializeField] private string _nodeName;

        /// <summary>
        /// FOR INTERNAL USE ONLY. Do not call this from user code.
        /// </summary>
        public void SetNodeName_Internal(string newName)
        {
            _nodeName = newName;
        }

        public string GetNodeName()
        {
            return _nodeName;
        }

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

        /// <summary>
        /// Get a Transition from this Node based on its Type.
        /// </summary>
        /// <param name="transitionType">Since there can be multiple transitions of the same Type, this method will get the first found transition of this Type.</param>
        /// <returns>Returns null if the Type is invalid, or if the Node does not have any Transitions of that Type.</returns>
        public NodeTransitionObject GetTransition(Type transitionType)
        {
            if (!typeof(NodeTransitionObject).IsAssignableFrom(transitionType))
                return null;

            for (int i = 0; i < Transitions.Count; i++)
            {
                if (Transitions[i].TransitionCondition.GetType() == transitionType)
                    return Transitions[i].TransitionCondition;
            }

            return null;
        }

        /// <summary>
        /// Get a Transition from this Node based on its index.
        /// </summary>
        /// <returns>Returns null if index is out of bounds.</returns>
        public NodeTransitionObject GetTransition(int index)
        {
            if (index < 0 || index > Transitions.Count)
                return null;

            return Transitions[index].TransitionCondition;
        }

        public NodeTransitionObject[] GetTransitions(Type transitionType)
        {
            if (!typeof(NodeTransitionObject).IsAssignableFrom(transitionType))
                return null;

            List<NodeTransitionObject> transitionsToReturn = new List<NodeTransitionObject>();

            for (int i = 0; i < Transitions.Count; i++)
            {
                if (Transitions[i].TransitionCondition.GetType() == transitionType)
                    transitionsToReturn.Add(Transitions[i].TransitionCondition);
            }

            return transitionsToReturn.Count > 0 ? transitionsToReturn.ToArray() : null;
        }

        /// <summary>
        /// Get a Trigger from this Node based on its Type.
        /// </summary>
        /// <returns>Returns null if the Type is invalid, or if the Node does not have any Triggers of that Type.</returns>
        public TriggerTransition GetTrigger(Type triggerType)
        {
            if (!typeof(TriggerTransition).IsAssignableFrom(triggerType))
                return null;

            for (int i = 0; i < Triggers.Count; i++)
            {
                if (Triggers[i].GetType() == triggerType)
                    return Triggers[i];
            }

            return null;
        }

        /// <summary>
        /// Get a Trigger from this Node based on its index.
        /// </summary>
        /// <returns>Returns null if index is out of bounds.</returns>
        public TriggerTransition GetTrigger(int index)
        {
            if (index < 0 || index > Triggers.Count)
                return null;

            return Triggers[index];
        }

        public TriggerTransition[] GetTriggers(Type triggerType)
        {
            if (!typeof(TriggerTransition).IsAssignableFrom(triggerType))
                return null;

            List<TriggerTransition> triggersToReturn = new List<TriggerTransition>();

            for (int i = 0; i < Triggers.Count; i++)
            {
                if (Triggers[i].GetType() == triggerType)
                    triggersToReturn.Add(Triggers[i]);
            }

            return triggersToReturn.Count > 0 ? triggersToReturn.ToArray() : null;
        }
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

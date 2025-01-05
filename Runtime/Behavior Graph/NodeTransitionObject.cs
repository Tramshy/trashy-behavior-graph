using UnityEngine;

namespace BehaviorGraph
{
    public abstract class NodeTransitionObject : BehaviorGraphInspectSO
    {
        [HideInInspector] public Node NodeCallingTransition;

        public abstract bool Condition();

        /// <summary>
        /// Called when a node containing this transition switches to a different node using a different condition.
        /// </summary>
        public virtual void ConditionExit()
        {
            // No-op
        }
    }
}

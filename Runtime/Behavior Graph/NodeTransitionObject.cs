using System;
using UnityEngine;

namespace BehaviorGraph
{
    public abstract class NodeTransitionObject : BehaviorGraphInspectSO
    {
        [HideInInspector] public Node NodeCallingTransition;

        public virtual void ConditionEnter()
        {
            // No-op
        }

        public abstract bool Condition();

        public virtual void ConditionExit()
        {
            // No-op
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public abstract class TriggerTransition : NodeTransitionObject
    {
        internal BehaviorTree thisTree;

        internal Node nextNode;

        public bool CallTrigger()
        {
            bool cond = IsInverted.Value ? !Condition() : Condition();

            if (cond)
                thisTree.SwitchCurrentNode(nextNode);

            return cond;
        }
    }
}

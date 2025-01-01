using UnityEngine;

namespace BehaviorGraph
{
    public abstract class NodeTransitionObject : BehaviorGraphInspectSO
    {
        [HideInInspector] public Node NodeCallingTransition;

        public abstract bool Condition();
    }
}

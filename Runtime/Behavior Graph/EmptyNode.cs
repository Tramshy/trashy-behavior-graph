using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class EmptyNode : Node, IBaseBehaviorElement
    {
        public override void OnNodeStart()
        {
            CurrentStatus = Statuses.Success;
        }

        public override void OnNodeUpdate()
        {
            // No-op
        }

        public override void ResetNode()
        {
            // No-op
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    [BaseBehaviorElement]
    public class EmptyNode : Node
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

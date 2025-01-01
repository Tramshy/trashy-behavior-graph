using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class BoolCompare : NodeTransitionObject, IBaseBehaviorElement, IAllowMultiTransitionElement
    {
        public DataField<bool> ToCheck;

        public override bool Condition()
        {
            return ToCheck.Value;
        }
    }
}

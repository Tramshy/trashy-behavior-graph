using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class BoolCompare : NodeTransitionObject, IBaseBehaviorElement, IAllowMultiTransitionElement
    {
        public DataField<bool> ToCheck;

        private enum CheckOption { True, False }
        [SerializeField] private CheckOption _checkTrueOrFalse;

        public override bool Condition()
        {
            if (_checkTrueOrFalse == CheckOption.True)
                return ToCheck.Value;
            else
            {
                return !ToCheck.Value;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    [BaseBehaviorElement, AllowMultipleTransition]
    public class BoolCompare : NodeTransitionObject
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

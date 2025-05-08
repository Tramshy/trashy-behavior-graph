using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    [BaseBehaviorElement, AllowMultipleTransition]
    public class WaitFor : NodeTransitionObject
    {
        public DataField<float> ToWait;

        public DataField<bool> ShouldResetOnExit;

        [NonSerialized] private float _passedTime;

        public override bool Condition()
        {
            _passedTime += Time.deltaTime;

            if (_passedTime >= ToWait.Value)
            {
                _passedTime = 0;

                return true;
            }

            return false;
        }

        public override void ConditionExit()
        {
            if (ShouldResetOnExit.Value)
                _passedTime = 0;
        }
    }
}

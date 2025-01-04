using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class WaitFor : NodeTransitionObject, IBaseBehaviorElement
    {
        public DataField<float> ToWait;

        private float _passedTime;

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
    }
}

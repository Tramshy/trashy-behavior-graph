using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class RngTransition : NodeTransitionObject, IBaseBehaviorElement, IAllowMultiTransitionElement
    {
        [Tooltip("Will return true if this number or higher is gotten using Random.Range.")]
        public DataField<int> Chance;
        public DataField<int> Min, Max;

        [Tooltip("Leave 0 to check every frame, otherwise input amount of time, in seconds, between checks.")]
        public DataField<float> TimeBetweenRandomChecks;

        [System.NonSerialized] private float _timePassed;

        public override bool Condition()
        {
            if (TimeBetweenRandomChecks.Value != 0)
            {
                _timePassed += Time.deltaTime;

                if (_timePassed >= TimeBetweenRandomChecks.Value)
                {
                    _timePassed = 0;

                    return Random.Range(Min.Value, Max.Value) >= Chance.Value;
                }
            }

            return Random.Range(Min.Value, Max.Value) >= Chance.Value;
        }

        public override void ConditionExit()
        {
            _timePassed = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    [BaseBehaviorElement, AllowMultipleTransition]
    public class DistanceCompare : NodeTransitionObject
    {
        public DataField<Vector3> A;
        public DataField<Vector3> B;

        public DataField<float> TargetDistance;

        private enum CompareOptions { GreaterThan, LessThan }
        [SerializeField] private CompareOptions _compareOption;

        public override bool Condition()
        {
            float distance = Vector3.Distance(A.Value, B.Value);

            if (_compareOption == CompareOptions.GreaterThan)
                return distance >= TargetDistance.Value;
            else
            {
                return distance <= TargetDistance.Value;
            }
        }
    }
}

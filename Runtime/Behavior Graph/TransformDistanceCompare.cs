using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public class TransformDistanceCompare : NodeTransitionObject, IBaseBehaviorElement, IAllowMultiTransitionElement
    {
        public DataField<Transform> A;
        public DataField<Transform> B;

        public DataField<float> TargetDistance;

        public enum CompareOptions { GreaterThan, LessThan }
        [SerializeField] public CompareOptions _compareOption;

        public override bool Condition()
        {
            float distance = Vector3.Distance(A.Value.position, B.Value.position);

            if (_compareOption == CompareOptions.GreaterThan)
                return distance >= TargetDistance.Value;
            else
            {
                return distance <= TargetDistance.Value;
            }
        }
    }
}

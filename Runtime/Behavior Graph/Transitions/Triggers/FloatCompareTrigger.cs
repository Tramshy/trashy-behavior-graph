using UnityEngine;

namespace BehaviorGraph
{
    [BaseBehaviorElement, AllowMultipleTransition]
    public class FloatCompareTrigger : TriggerTransition
    {
        public DataField<float> A;
        public DataField<float> B;

        private enum CompareOptions { GreaterThan, LessThan }
        [SerializeField] private CompareOptions _compareOption;

        public override bool Condition()
        {
            if (_compareOption == CompareOptions.GreaterThan)
                return A.Value >= B.Value;
            else
            {
                return A.Value <= B.Value;
            }
        }
    }
}

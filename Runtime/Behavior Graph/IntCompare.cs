using UnityEngine;

namespace BehaviorGraph
{
    public class IntCompare : NodeTransitionObject, IBaseBehaviorElement, IAllowMultiTransitionElement
    {
        public DataField<int> A;
        public DataField<int> B;

        private enum CompareOptions { Equal, LessThan, GreaterThan, NotEqual, EqualOrLessThan, EqualOrGreaterThan }
        [SerializeField] private CompareOptions _compareOption;

        public override bool Condition()
        {
            switch (_compareOption)
            {
                case CompareOptions.Equal:
                    return A.Value == B.Value;

                case CompareOptions.LessThan:
                    return A.Value < B.Value;

                case CompareOptions.GreaterThan:
                    return A.Value > B.Value;

                case CompareOptions.NotEqual:
                    return A.Value != B.Value;

                case CompareOptions.EqualOrLessThan:
                    return A.Value <= B.Value;

                case CompareOptions.EqualOrGreaterThan:
                    return A.Value >= B.Value;
            }

            return false;
        }
    }
}
using System;
using UnityEngine;

namespace BehaviorGraph
{
    public class WaitNode : Node, IBaseBehaviorElement
    {
        public DataField<float> TimeToWait;

        public DataField<bool> ShouldRestartOnExit;

        [NonSerialized] private float _passedTime;

        public override void OnNodeStart()
        {
            // No-op
        }

        public override void OnNodeUpdate()
        {
            _passedTime += Time.deltaTime;

            if (_passedTime >= TimeToWait.Value)
            {
                CurrentStatus = Statuses.Success;

                _passedTime = 0;

                return;
            }
        }

        public override void ResetNode()
        {
            CurrentStatus = Statuses.Running;

            if (ShouldRestartOnExit.Value)
                _passedTime = 0;
        }
    }
}

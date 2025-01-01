using UnityEngine;
using TrashyTools;

namespace BehaviorGraph
{
    public class WaitNode : Node, IBaseBehaviorElement
    {
        public DataField<float> Time;
        [HideInInspector] public  Timer Timer;

        public override void OnNodeStart()
        {
            CurrentStatus = Statuses.Running;

            Timer = Timer.CreateTimer(() =>
            {
                CurrentStatus = Statuses.Success;
                ResetNode();
            }, Time.Value, false);
        }

        public override void OnNodeUpdate()
        {
            // No-op
        }

        public override void ResetNode()
        {
            Timer?.StopTimer();
            Timer = null;
        }
    }
}

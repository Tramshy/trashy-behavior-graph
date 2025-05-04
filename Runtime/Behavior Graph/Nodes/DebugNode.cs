using UnityEngine;

namespace BehaviorGraph
{
    public class DebugNode : Node, IBaseBehaviorElement
    {
        public DataField<string> Message;

        [SerializeField] public DataField<bool> ShouldLoop;

        public override void OnNodeStart()
        {
            Debug.Log(Message.Value);

            CurrentStatus = Statuses.Success;
        }

        public override void OnNodeUpdate()
        {
            if (!ShouldLoop.Value)
                return;

            Debug.Log(Message.Value);
        }

        public override void ResetNode()
        {
            // No-op
        }
    }
}

using UnityEngine;

namespace BehaviorGraph
{
    public class DebugNode : Node, IBaseBehaviorElement
    {
        public DataField<string> Message;

        [SerializeField] private DataField<bool> _shouldLoop;

        public override void OnNodeStart()
        {
            Debug.Log(Message.Value);

            CurrentStatus = Statuses.Success;
        }

        public override void OnNodeUpdate()
        {
            if (!_shouldLoop.Value)
                return;

            Debug.Log(Message.Value);
        }

        public override void ResetNode()
        {
            // No-op
        }
    }
}

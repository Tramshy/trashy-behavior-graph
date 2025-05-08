namespace BehaviorGraph
{
    [BaseBehaviorElement]
    public class NodeFailed : NodeTransitionObject
    {
        public override bool Condition()
        {
            return NodeCallingTransition.CurrentStatus == Node.Statuses.Failure;
        }
    }
}

namespace BehaviorGraph
{
    public class NodeFailed : NodeTransitionObject, IBaseBehaviorElement
    {
        public override bool Condition()
        {
            return NodeCallingTransition.CurrentStatus == Node.Statuses.Failure;
        }
    }
}

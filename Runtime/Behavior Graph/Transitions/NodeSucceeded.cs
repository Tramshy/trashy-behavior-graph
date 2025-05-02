namespace BehaviorGraph
{
    public class NodeSucceeded : NodeTransitionObject, IBaseBehaviorElement
    {
        public override bool Condition()
        {
            return NodeCallingTransition.CurrentStatus == Node.Statuses.Success;
        }
    }
}

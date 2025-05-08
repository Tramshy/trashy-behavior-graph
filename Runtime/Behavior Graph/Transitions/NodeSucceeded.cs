namespace BehaviorGraph
{
    [BaseBehaviorElement]
    public class NodeSucceeded : NodeTransitionObject
    {
        public override bool Condition()
        {
            return NodeCallingTransition.CurrentStatus == Node.Statuses.Success;
        }
    }
}

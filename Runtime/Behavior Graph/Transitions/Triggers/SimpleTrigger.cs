namespace BehaviorGraph
{
    // This Trigger will simply swap Nodes when the CallTrigger method is called.
    [BaseBehaviorElement, AllowMultipleTransition]
    public class SimpleTrigger : TriggerTransition
    {
        public override bool Condition()
        {
            return true;
        }
    }
}

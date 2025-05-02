namespace BehaviorGraph
{
    public class SwitchPanel : Node, IBaseBehaviorElement
    {
        public DataField<BehaviorPanel> NewPanel;
        public DataField<BehaviorTree> ThisTree;

        public override void OnNodeStart()
        {
            ThisTree.Value.SwitchBehaviorPanel(NewPanel.Value);

            CurrentStatus = Statuses.Success;
        }

        public override void OnNodeUpdate()
        {

        }

        public override void ResetNode()
        {
            CurrentStatus = Statuses.Running;
        }
    }
}

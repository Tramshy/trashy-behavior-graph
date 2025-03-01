using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorTree : MonoBehaviour
    {
        [SerializeField] private BehaviorPanel _panel;
        public BehaviorPanel Panel { get => _panel; private set
            {
                if (value.MonoBehaviorDataComponent != null && !value.HasLoaded)
                {
                    foreach (Node n in value.PanelNodes)
                    {
                        n.SetUpFields(gameObject, Panel.MonoBehaviorDataComponent);
                        n.Transitions.ForEach((t) => t.TransitionCondition.SetUpFields(gameObject, Panel.MonoBehaviorDataComponent));
                    }

                    value.HasLoaded = true;
                }

                CurrentNode = value.StartingNode;
                _panel = value;
            }
        }

        private Node _currentNode;
        public Node CurrentNode { get => _currentNode; private set
            {
                if (_currentNode != null)
                {
                    _currentNode.ResetNode();

                    _currentNode.Transitions.ForEach((t) => t.TransitionCondition.ConditionExit());
                }

                _currentNode = value;
                _currentNode.OnNodeStart();
                _currentNode.Transitions.ForEach((t) => t.TransitionCondition.ConditionEnter());
            }
        }

        private void Start()
        {
            Panel = _panel;
        }

        public void SwitchBehaviorPanel(BehaviorPanel panel)
        {
            Panel = panel;
        }

        private void SwitchCurrentNode(Node newNode)
        {
            if (newNode == null)
            {
                CurrentNode = null;

                return;
            }

            CurrentNode = newNode;
        }

        private void Update()
        {
            if (CurrentNode == null)
                return;

            CurrentNode.OnNodeUpdate();

            NodeTransition t = null;

            for (int i = 0; i < CurrentNode.Transitions.Count; i++)
            {
                t = CurrentNode.Transitions[i];

                bool cond = t.TransitionCondition.IsInverted.Value ? !t.TransitionCondition.Condition() : t.TransitionCondition.Condition();

                if (cond)
                {
                    SwitchCurrentNode(t.NextInLine);

                    break;
                }
            }
        }
    }
}


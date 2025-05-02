using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorTree : MonoBehaviour
    {
        [SerializeField] private BehaviorPanel _panel;
        public BehaviorPanel Panel { get => _panel; private set
            {
                var panelToSwitch = value;

                if (panelToSwitch.MonoBehaviorDataComponent != null)
                {
                    if (!value.HasLoaded)
                    {
                        foreach (Node n in value.PanelNodes)
                        {
                            n.SetUpFields(gameObject, value.MonoBehaviorDataComponent);
                            n.Transitions.ForEach((t) => t.TransitionCondition.SetUpFields(gameObject, value.MonoBehaviorDataComponent));
                        }
                    }
                    else
                    {
                        panelToSwitch = ScriptableObjectDeepClone.DeepClone(value);

                        foreach (Node n in panelToSwitch.PanelNodes)
                        {
                            n.SetUpFields(gameObject, panelToSwitch.MonoBehaviorDataComponent);
                            n.Transitions.ForEach((t) => t.TransitionCondition.SetUpFields(gameObject, panelToSwitch.MonoBehaviorDataComponent));
                        }
                    }
                }

                panelToSwitch.HasLoaded = true;
                CurrentNode = panelToSwitch.StartingNode;
                _panel = panelToSwitch;
            }
        }

        [SerializeField, ReadOnlyInspector] private Node _currentNode;
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

        /// <summary>
        /// Switches from one panel to another.
        /// WARNING: Only do this if only one BehaviorTree uses the panels.
        /// </summary>
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


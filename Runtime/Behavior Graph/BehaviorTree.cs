using UnityEngine;

namespace BehaviorGraph
{
    public class BehaviorTree : MonoBehaviour
    {
        [SerializeField] private BehaviorPanel _panel;
        public BehaviorPanel Panel { get => _panel; private set
            {
                if (value.MonoBehaviorDataComponent != null)
                {
                    foreach (Node n in value.PanelNodes)
                    {
                        n.SetUpFields(gameObject, Panel.MonoBehaviorDataComponent);
                        n.Transitions.ForEach((t) => t.TransitionCondition.SetUpFields(gameObject, Panel.MonoBehaviorDataComponent));
                    }
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
            }
        }

        private void Awake()
        {
            Panel = _panel;
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
            CurrentNode.Transitions.ForEach((t) =>
            {
                if (t.TransitionCondition.Condition())
                    SwitchCurrentNode(t.NextInLine);
            });
        }
    }
}


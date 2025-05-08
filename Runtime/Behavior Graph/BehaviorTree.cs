using System.Collections.Generic;
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
                    if (value.HasLoaded) // Set up with deep clone.
                        panelToSwitch = ScriptableObjectDeepClone.DeepClone(value);

                    SetUpPanel(panelToSwitch);
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

        private void SetUpPanel(BehaviorPanel panel)
        {
            foreach (Node n in panel.PanelNodes)
            {
                // Set up fields for all transitions.
                n.SetUpFields(gameObject, panel.MonoBehaviorDataComponent);

                for (int i = 0; i < n.Transitions.Count; i++)
                {
                    n.Transitions[i].TransitionCondition.SetUpFields(gameObject, panel.MonoBehaviorDataComponent);
                }

                // Sort transitions.
                for (int i = 0; i < n.Transitions.Count; i++)
                {
                    if (n.Transitions[i].TransitionCondition is TriggerTransition)
                    {
                        if (n.Triggers == null)
                            n.Triggers = new List<TriggerTransition>();

                        var trigger = n.Transitions[i].TransitionCondition as TriggerTransition;

                        trigger.thisTree = this;
                        trigger.nextNode = n.Transitions[i].NextInLine;
                        n.Triggers.Add(trigger);
                    }
                    else
                    {
                        if (n.UpdateTransitions == null)
                            n.UpdateTransitions = new List<NodeTransition>();

                        n.UpdateTransitions.Add(n.Transitions[i]);
                    }
                }

                if (n.Triggers.Count == 0)
                    n.Triggers = null;

                if (n.UpdateTransitions.Count == 0)
                    n.UpdateTransitions = null;
            }
        }

        /// <summary>
        /// Switches from one panel to another.
        /// This may be costly for performance if several BehaviorTrees switch at once for complex behaviors.
        /// </summary>
        public void SwitchBehaviorPanel(BehaviorPanel panel)
        {
            Panel = panel;
        }

        internal void SwitchCurrentNode(Node newNode)
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

            if (CurrentNode.UpdateTransitions == null)
                return;

            NodeTransition t = null;

            for (int i = 0; i < CurrentNode.UpdateTransitions.Count; i++)
            {
                t = CurrentNode.UpdateTransitions[i];

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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorGraph
{
    public static class ScriptableObjectDeepClone
    {
        public static T DeepClone<T>(T original) where T : BehaviorPanel
        {
            if (original == null)
                return null;

            // Clone the main ScriptableObject.
            T clone = ScriptableObject.CreateInstance<T>();
            clone.DataComponentName = original.DataComponentName;

            // Clone all initial Nodes.
            for (int i = 0; i < original.PanelNodes.Count; i++)
            {
                clone.PanelNodes.Add(ScriptableObject.Instantiate(original.PanelNodes[i]));
                clone.PanelNodes[i].Transitions = new List<NodeTransition>();
            }

            for (int i = 0; i < original.PanelNodes.Count; i++)
            {
                for (int j = 0; j < original.PanelNodes[i].Transitions.Count; j++)
                {
                    // The object containing the transition logic for this transition.
                    var transitionObject = original.PanelNodes[i].Transitions[j].TransitionCondition;
                    // Copy transition logic to new instance.
                    var newTransition = ScriptableObject.Instantiate(transitionObject);
                    newTransition.NodeCallingTransition = clone.PanelNodes[i];

                    for (int k = 0; k < clone.PanelNodes.Count; k++)
                    {
                        // Set up starting node if the current copied Node's ID matches the original starting ID.
                        if (clone.PanelNodes[k].UniqueID == original.StartingNode.UniqueID)
                            clone.StartingNode = clone.PanelNodes[k];

                        // Iterate through copied Nodes until ID matches with the next Node for original transitions.
                        if (clone.PanelNodes[k].UniqueID == original.PanelNodes[i].Transitions[j].NextInLine.UniqueID)
                        {
                            // Clone Transitions with new Nodes.
                            clone.PanelNodes[i].Transitions.Add(new NodeTransition(newTransition, clone.PanelNodes[k]));

                            break;
                        }
                    }
                }
            }

            return clone;
        }
    }
}

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.GraphEditor
{
    public class BehaviorGraphField : BlackboardField
    {
        public BehaviorGraphField() : base()
        {
            base.capabilities = Capabilities.Selectable;
        }

        //protected override void BuildFieldContextualMenu(ContextualMenuPopulateEvent evt)
        //{
        //    evt.menu.AppendAction("Copy", (menuAction) => GUIUtility.systemCopyBuffer = text);

        //    evt.menu.AppendSeparator();
        //}
    }
}

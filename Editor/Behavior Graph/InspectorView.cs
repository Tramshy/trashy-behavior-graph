using UnityEditor;
using UnityEngine.UIElements;

namespace BehaviorGraph.GraphEditor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        public BehaviorGraphInspectSO CurrentData { get; private set; }

        public void UpdateInspectorView(BehaviorGraphInspectSO viewUpdate)
        {
            Clear();

            if (viewUpdate == null)
                return;

            CurrentData = viewUpdate;

            Editor editor = Editor.CreateEditor(viewUpdate);
            IMGUIContainer container = new IMGUIContainer(() => editor.OnInspectorGUI());
            Add(container);
        }
    }
}

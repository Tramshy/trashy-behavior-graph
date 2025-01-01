using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace BehaviorGraph.GraphEditor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private BehaviorGraphEditor _editor;
        private BehaviorGraphView _view;

        private Texture2D _hiddenIcon;

        public void Initialize()
        {
            _editor = BehaviorGraphEditor.WND as BehaviorGraphEditor;
            _view = BehaviorGraphEditor.ThisGraphView;

            _hiddenIcon = new Texture2D(1, 1);
            _hiddenIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            _hiddenIcon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Nodes")),
                new SearchTreeGroupEntry(new GUIContent("Base"), 1)
            };

            var nodes = TypeCache.GetTypesDerivedFrom<Node>();
            var customNodes = nodes.ToList();

            foreach (Type node in nodes)
            {
                if (typeof(IBaseBehaviorElement).IsAssignableFrom(node))
                {
                    tree.Add(new SearchTreeEntry(new GUIContent(node.Name, _hiddenIcon))
                    {
                        userData = node,
                        level = 2
                    });

                    customNodes.Remove(node);
                }
            }

            tree.Add(new SearchTreeGroupEntry(new GUIContent("Custom"), 1));

            foreach (Type customNode in customNodes)
            {
                tree.Add(new SearchTreeEntry(new GUIContent(customNode.Name, _hiddenIcon))
                {
                    userData = customNode,
                    level = 2
                });
            }

            return tree;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            // This is kind of broken but whatever.
            Vector2 windowMousePosition = _editor.rootVisualElement.ChangeCoordinatesTo(_editor.rootVisualElement.parent, context.screenMousePosition - _editor.position.position);
            Vector2 localMousePosition = _view.contentViewContainer.WorldToLocal(windowMousePosition);

            _view.AddNewNode(SearchTreeEntry.userData as Type, new Rect(localMousePosition, Vector2.one));

            return true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    [CustomEditor(typeof(BehaviorPanel))]
    public class BehaviorPanelEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BehaviorPanel panel = (BehaviorPanel)target;

            GUILayout.Label("After setting Behavior Data Component,\nyou must link it by pressing the button below before runtime.");
            if (GUILayout.Button("Link Behavior Data"))
                panel.SetBehaviorData();
        }
    }
}

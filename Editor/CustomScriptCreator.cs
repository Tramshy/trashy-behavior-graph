using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class CustomScriptCreator
{
    private static readonly string _nodeScriptName = "CustomNodeTemplate.txt", _transitionScriptName = "CustomTransitionTemplate.txt";
    private static readonly string _path = "Assets/trashy-behavior-graph/Editor/Templates/";
    //Packages/com.tramshy.trashy-behavior-graph/Editor
    [MenuItem("Assets/Create/Behavior Graph/Custom Node Script", false, 80)]
    public static void CreateNewNode()
    {
        string activeFolderPath = GetSelectedPathOrFallback();

        string path = EditorUtility.SaveFilePanelInProject("Create Custom Node Script", "NewCustomNode.cs", "cs", "Enter a file name for the new node", activeFolderPath);

        if (string.IsNullOrEmpty(path))
            return;

        string template = File.ReadAllText(_path + _nodeScriptName);
        string scriptName = Path.GetFileNameWithoutExtension(path);
        string content = template.Replace("#SCRIPTNAME#", scriptName);

        File.WriteAllText(path, content);
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Create/Behavior Graph/Custom Transition Script", false, 80)]
    public static void CreateNewTransition()
    {
        string activeFolderPath = GetSelectedPathOrFallback();

        string path = EditorUtility.SaveFilePanelInProject("Create Custom Transition Script", "NewCustomTransition.cs", "cs", "Enter a file name for the new transition", activeFolderPath);

        if (string.IsNullOrEmpty(path))
            return;

        string template = File.ReadAllText(_path + _transitionScriptName);
        string scriptName = Path.GetFileNameWithoutExtension(path);
        string content = template.Replace("#SCRIPTNAME#", scriptName);

        File.WriteAllText(path, content);
        AssetDatabase.Refresh();
    }

    private static string GetSelectedPathOrFallback()
    {
        string path = "Assets";

        foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        {
            path = AssetDatabase.GetAssetPath(obj);
            if (File.Exists(path))
            {
                path = Path.GetDirectoryName(path);
            }
            break;
        }
        return path;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class CustomScriptCreator
{
    private static readonly string _nodeScriptName = "CustomNodeTemplate.txt", _transitionScriptName = "CustomTransitionTemplate.txt";
    private static readonly string _path = "Packages/com.tramshy.trashy-behavior-graph/Editor/Templates/";

    [MenuItem("Assets/Create/Behavior Graph/Custom Node Script", false, 80)]
    public static void CreateNewNode()
    {
        string selectedPath = GetSelectedPathOrFallback();
        string scriptPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectedPath, "NewCustomNode.cs"));

        string templateContent = File.ReadAllText(_path + _nodeScriptName);

        string scriptContent = templateContent.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(scriptPath));

        File.WriteAllText(scriptPath, scriptContent);

        AssetDatabase.Refresh();
        Object newScript = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
        Selection.activeObject = newScript;
    }

    [MenuItem("Assets/Create/Behavior Graph/Custom Transition Script", false, 80)]
    public static void CreateNewTransition()
    {
        string selectedPath = GetSelectedPathOrFallback();
        string scriptPath = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(selectedPath, "NewCustomTransition.cs"));

        string templateContent = File.ReadAllText(_path + _transitionScriptName);

        string scriptContent = templateContent.Replace("#SCRIPTNAME#", Path.GetFileNameWithoutExtension(scriptPath));

        File.WriteAllText(scriptPath, scriptContent);

        AssetDatabase.Refresh();
        Object newScript = AssetDatabase.LoadAssetAtPath<Object>(scriptPath);
        Selection.activeObject = newScript;
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

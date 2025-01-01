using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    [InitializeOnLoad]
    public static class SaveFolderSetup
    {
        static SaveFolderSetup()
        {
            string saveFolderPath = "Assets/Behavior Graph Saves/Editor";

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
                AssetDatabase.Refresh();

                Debug.Log("Created 'SavedData' folder in the Assets directory for Behavior Graph save files.");
            }
        }
    }
}

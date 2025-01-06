using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    [InitializeOnLoad]
    public static class SerializeDataInitialization
    {
        static SerializeDataInitialization()
        {
            string saveFolderPath = "Assets/Behavior Graph Saves/Editor";
            string serializerPath = saveFolderPath + "/Resources";

            if (!Directory.Exists(saveFolderPath))
            {
                Directory.CreateDirectory(saveFolderPath);
                AssetDatabase.Refresh();

                Debug.Log("Created 'SavedData' folder in the Assets directory for Behavior Graph save files.");
            }


            if (!Directory.Exists(serializerPath))
            {
                Directory.CreateDirectory(serializerPath);
                AssetDatabase.Refresh();

                Debug.Log("Created 'Resources' folder in the Assets directory for Node Instance Serialization.");
            }

            serializerPath += "/Instance Serializer.asset";

            if (!File.Exists(serializerPath))
            {
                NodeInstancesSerializer serializer = ScriptableObject.CreateInstance(typeof(NodeInstancesSerializer)) as NodeInstancesSerializer;

                serializer.SetUpNodeInstances();

                AssetDatabase.CreateAsset(serializer, serializerPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Created new Serializer");
            }
        }
    }
}

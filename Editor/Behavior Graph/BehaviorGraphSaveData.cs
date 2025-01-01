using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    // Next time use Scriptable Objects for saving in editor.
    [Serializable]
    public class BehaviorGraphSaveData
    {
        public BehaviorGraphSaveData(BehaviorGraphNode[] nodes)
        {
            Nodes = nodes.ToList();

            Identifier = Guid.NewGuid().ToString();
        }

        public BehaviorGraphSaveData(List<BehaviorGraphNode> nodes)
        {
            Nodes = nodes;

            Identifier = Guid.NewGuid().ToString();
        }

        public List<BehaviorGraphNode> Nodes;

        // Reminder to self: properties cannot be serialized.
        public string Identifier; // { get; private set; }
    }

    public static class GraphSave
    {
        public static void SaveDataAsJSON(BehaviorGraphSaveData data, string saveName, string path = "Packages/trashy-behavior-graph/Editor/Behavior Graph/Graph Saves/")
        {
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Directory does not exist: {path}");

                return;
            }

            path += saveName + ".json";

            if (File.Exists(path))
                File.Delete(path);

            string json = JsonUtility.ToJson(data);

            File.WriteAllText(path, json);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static BehaviorGraphSaveData LoadDataFromGUID(string guid, string path = "Packages/trashy-behavior-graph/Editor/Behavior Graph/Graph Saves/")
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory does not exist: {path}");

            try
            {
                // Can't load from GUID because it uses custom GUID not the kind that interacts with meta data.
                string json = File.ReadAllText(path + guid + ".json");

                BehaviorGraphSaveData data = JsonUtility.FromJson<BehaviorGraphSaveData>(json);

                // Will always be at least one element due to Start Node.
                // This is only needed for when Unity is restarted.
                // Upon restart Unity will lose its in-memory reference to the scriptable object and manual reassignment is necessary.
                if (data.Nodes[0].ThisNode == null)
                {
                    foreach (BehaviorGraphNode n in data.Nodes)
                    {
                        n.ThisNode = AssetDatabase.LoadAssetAtPath<Node>(AssetDatabase.GUIDToAssetPath(n.GUID));

                        n.Ports.ForEach((p) => p.Transition = AssetDatabase.LoadAssetAtPath<NodeTransitionObject>(AssetDatabase.GUIDToAssetPath(p.GUID)));
                    }
                }

                return data;
            }
            catch { }

            string[] jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

            foreach (string file in jsonFiles)
            {
                try
                {
                    string content = File.ReadAllText(file);

                    if (content.Contains(guid))
                    {
                        BehaviorGraphSaveData data = JsonUtility.FromJson<BehaviorGraphSaveData>(file);

                        if (data.Nodes[0].ThisNode == null)
                        {
                            foreach (BehaviorGraphNode n in data.Nodes)
                            {
                                n.ThisNode = AssetDatabase.LoadAssetAtPath<Node>(AssetDatabase.GUIDToAssetPath(n.GUID));

                                n.Ports.ForEach((p) => p.Transition = AssetDatabase.LoadAssetAtPath<NodeTransitionObject>(AssetDatabase.GUIDToAssetPath(p.GUID)));
                            }
                        }

                        return data;
                    }
                }
                catch
                {
                    throw new Exception($"Failed to read file {file}");
                }
            }

            throw new Exception($"No file found at {path}");
        }

        [Obsolete]
        public static BehaviorGraphSaveData LoadDataFromPath(string path)
        {
            if (!File.Exists(path))
                throw new Exception($"No file found at {path}");

            string json = File.ReadAllText(path);
            BehaviorGraphSaveData loadedData = JsonUtility.FromJson<BehaviorGraphSaveData>(json);

            return loadedData;
        }
        
        public static bool IsFileAtGUIDValid(string guid, string path = "Packages/trashy-behavior-graph/Editor/Behavior Graph/Graph Saves")
        {
            return File.Exists($"{path}/{guid}");
        }
    }
}

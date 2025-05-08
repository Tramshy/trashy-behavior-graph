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

        public string Identifier; // { get; private set; }
    }

    public static class GraphSave
    {
        public static void SaveDataAsJSON(BehaviorGraphSaveData data, string saveName, string path = "Assets/Behavior Graph Saves/Editor/")
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

        public static BehaviorGraphSaveData LoadDataFromGUID(string guid, string path = "Assets/Behavior Graph Saves/Editor/")
        {
            if (!Directory.Exists(path))
                throw new Exception($"Directory does not exist: {path}");

            string json = File.ReadAllText(path + guid + ".json");

            BehaviorGraphSaveData data = JsonUtility.FromJson<BehaviorGraphSaveData>(json);

            bool needsManuelSet = false;

            foreach (var n in data.Nodes)
            {
                if (n.ThisNode == null)
                {
                    needsManuelSet = true;

                    break;
                }

                foreach (var t in n.ThisNode.Transitions)
                {
                    if (t.TransitionCondition || t.NextInLine == null)
                    {
                        needsManuelSet = true;

                        break;
                    }
                }

                foreach (var p in n.Ports)
                {
                    if (p.Transition == null)
                    {
                        needsManuelSet = true;

                        break;
                    }
                }
            }

            // Will always be at least one element due to Start Node.
            // This is only needed for when Unity is restarted.
            // Upon restart Unity will lose its in-memory reference to the scriptable object and manual reassignment is necessary.
            if (needsManuelSet)
            {
                for (int i = 0; i < data.Nodes.Count; i++)
                {
                    NodeInstancesSerializer ser = Resources.Load<NodeInstancesSerializer>("Instance Serializer");

                    var n = data.Nodes[i];

                    n.ThisNode = ser.NodeInstances.GetNodeInstance(n.GUID) as Node;

                    for (int j = 0; j < n.ThisNode.Transitions.Count; j++)
                    {
                        var t = n.ThisNode.Transitions[j];

                        t.TransitionCondition = ser.NodeInstances.GetNodeInstance(t.TGUID) as NodeTransitionObject;
                        t.NextInLine = ser.NodeInstances.GetNodeInstance(t.NGUID) as Node;
                    }

                    for (int j = 0; j < n.Ports.Count; j++)
                    {
                        n.Ports[j].Transition = ser.NodeInstances.GetNodeInstance(n.Ports[j].GUID) as NodeTransitionObject;
                    }
                }
            }

            return data;
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
        
        public static bool IsFileAtGUIDValid(string guid, string path = "Assets/Behavior Graph Saves/Editor")
        {
            return File.Exists($"{path}/{guid}");
        }
    }
}

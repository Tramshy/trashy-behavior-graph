using System.IO;
using UnityEditor;
using UnityEngine;

namespace BehaviorGraph.GraphEditor
{
    public class PanelJSONSync : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (Path.GetExtension(assetPath) != ".asset")
                return AssetDeleteResult.DidNotDelete;

            BehaviorPanel toDeletePanel = AssetDatabase.LoadAssetAtPath<BehaviorPanel>(assetPath);

            if (toDeletePanel == null)
            {
                NodeInstancesSerializer toDeleteSerializer = AssetDatabase.LoadAssetAtPath<NodeInstancesSerializer>(assetPath);

                if (toDeleteSerializer != null)
                {
                    Debug.LogWarning("Do not attempt to delete or remove Instance Serializer, this will lead to a severe loss of data.");

                    return AssetDeleteResult.FailedDelete;
                }

                return AssetDeleteResult.DidNotDelete;
            }

            string guid = toDeletePanel.Identifier;
            string jsonDataPath = $"Assets/Behavior Graph Saves/Editor/{guid}.json";

            if (File.Exists(jsonDataPath))
            {
                File.Delete(jsonDataPath);
                File.Delete(jsonDataPath + ".meta");
            }

            return AssetDeleteResult.DidNotDelete;
        }
    }

    public class CleanUpInstances : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (deletedAssets.Length > 0)
            {
                try
                {
                    Resources.Load<NodeInstancesSerializer>("Instance Serializer").ClearEmptyInstances();
                }
                catch
                {
                    return;
                }
            }
        }
    }
}

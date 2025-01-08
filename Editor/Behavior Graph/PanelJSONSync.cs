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
            //else
            //{
            //    Debug.LogWarning($"No associated serialized data was found for deleted object at path: {jsonDataPath}.\n" +
            //                    $"This should only happen if the associated JSON file was renamed.\n" +
            //                    $"Associated serialized data has not been deleted with Panel, this could cause build up.");
            //}

            return AssetDeleteResult.DidNotDelete;
        }
    }

    public class CleanUpInstances : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (deletedAssets.Length > 0)
                Resources.Load<NodeInstancesSerializer>("Instance Serializer").ClearEmptyInstances();
        }
    }
}

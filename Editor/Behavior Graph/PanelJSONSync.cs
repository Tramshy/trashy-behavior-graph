using System.IO;
using UnityEditor;

namespace BehaviorGraph.GraphEditor
{
    public class PanelJSONSync : AssetModificationProcessor
    {
        public static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            if (Path.GetExtension(assetPath) != ".asset")
                return AssetDeleteResult.DidNotDelete;

            BehaviorPanel toDelete = AssetDatabase.LoadAssetAtPath<BehaviorPanel>(assetPath);

            if (toDelete == null)
                return AssetDeleteResult.DidNotDelete;

            string guid = toDelete.Identifier;
            string jsonDataPath = $"Assets/Editor/Behavior Graph/Graph Saves/{guid}.json";

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
}

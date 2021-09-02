using UnityEditor;
using System.IO;

public class CreateAssetBundles 
{
    [MenuItem("Assets/Build AssetsBundles")]
    static void BuildAssetBundles()
    {
        string assetBundleDirectory = "Assets/StreamingAssets/AssetBundles";

        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }

        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, 
            BuildTarget.StandaloneWindows);
    }
}
//The class in this script manages the assetBundles loading process for assetBundles with shared dependencies

using UnityEngine;
using System.IO;
using System.Linq;


public class LoadAssetBundles : MonoBehaviour
{   

    void Start() {

        //The name of the assetBundles we want to load
        string aBundle1 = "atype";
        string aBundle2 = "btype";

        //Load manifest to find all dependencies of the assetBundles that will be loaded
        AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + "/AssetBundles", "AssetBundles"));

        if (assetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

        string[] dependencies1 = manifest.GetAllDependencies(aBundle1);
        string[] dependencies2 = manifest.GetAllDependencies(aBundle2);

        //Filter duplicate dependencies before loading it
        var allDependencies = dependencies1.Union(dependencies2).ToArray();

        foreach (string dependency in allDependencies)
        {
            AssetBundle.LoadFromFile(Path.Combine(Path.Combine(Application.streamingAssetsPath + "/AssetBundles"), dependency));
        }    

        //With all dependencies loaded we can load the assetBundles that will be used
        var asset1 
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + "/AssetBundles", aBundle1));

        if (asset1 == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        var asset2
            = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath + "/AssetBundles", aBundle2));

        if (asset2 == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        //Get specific assets from the loaded assetBundles. A couple of cubes in our case
        var prefab1 = asset1.LoadAsset<GameObject>("CubeA");
        var prefab2 = asset2.LoadAsset<GameObject>("CubeB");

        Instantiate(prefab1);
        Instantiate(prefab2, new Vector3(1.1f, 0f, 0f), Quaternion.identity);

    }
}

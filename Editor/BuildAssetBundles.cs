using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundles
{
    [MenuItem("AssetBundle/Build AssetBundles")]
    private static void Build()
    {
        string dir = Application.persistentDataPath + "/AssetBundles";
        Debug.Log(dir);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
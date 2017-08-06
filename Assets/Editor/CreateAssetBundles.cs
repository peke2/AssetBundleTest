using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles()
	{
		BuildPipeline.BuildAssetBundles("Assets/AssetBundles/AssetBundleWindows", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
	}

    [MenuItem("Assets/Build Test")]
    static void BuildTest()
    {
        AssetBundleBuild[] buildMap = new AssetBundleBuild[1];

        //string[] testNames = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName("dailog1", "dialog");
        //Debug.Log("確認["+testNames[0]+"]");
        /*
        string[] assetNames = new string[3];
        buildMap[0].assetBundleName = "dialog_bundle";
        assetNames[0] = "Assets/Resources/Dialog1.prefab";
        assetNames[1] = "Assets/Resources/Dialog2.prefab";
        assetNames[2] = "Assets/Resources/Dialog3.prefab";
        */
        string[] assetNames = new string[1];
        buildMap[0].assetBundleName = "dialog_bundle";
        assetNames[0] = "Assets/Resources/Dialog3.prefab";

        buildMap[0].assetNames = assetNames;

        //  確認用なので強制的にビルドするオプションを指定
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", buildMap, BuildAssetBundleOptions.ForceRebuildAssetBundle, BuildTarget.StandaloneWindows);
    }

    [MenuItem("Assets/Clear Cache")]
    static void ClearCache()
    {
        Caching.CleanCache();
    }
}

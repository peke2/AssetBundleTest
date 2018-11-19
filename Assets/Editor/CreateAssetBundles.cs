﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


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
        
        /*string[] assetNames = new string[3];
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
        ClearCache();
    }

	[MenuItem("Assets/Build Text Asset")]
	static void BuidTextAsset()
	{
		string assetBundleDirectory = "Assets/AssetBundles";
		//	出力先が無ければ作成(gitで空のディレクトリが保持されないので最初に作る必要がある)
		if (!Directory.Exists(assetBundleDirectory) )
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}

		//BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

		var taDoc = Resources.Load<TextAsset>("Text/document");
		var taInfo = Resources.Load<TextAsset>("Text/info");
		TextAsset[] objs = new TextAsset[]{ taDoc, taInfo};
		bool isSucceeded;
		//isSucceeded = BuildPipeline.BuildAssetBundle(null, objs, "Assets/AssetBundles/text", BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
		isSucceeded = BuildPipeline.BuildAssetBundle(null, objs, "Assets/AssetBundles/text", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		Debug.Log("["+((isSucceeded==true)?"成功":"失敗")+ "]<-テキストアセットバンドル作成結果");

		//	Json形式のテキストからアセットバンドルを作成
		var jsonText = Resources.Load<TextAsset>("Json/info");
		//var infoBase = JsonUtility.FromJson<InfoBase>(jsonText.text);
		isSucceeded = BuildPipeline.BuildAssetBundle(null, new TextAsset[] { jsonText }, "Assets/AssetBundles/info", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		Debug.Log("[" + ((isSucceeded == true) ? "成功" : "失敗") + "]<-情報アセットバンドル作成結果");
	}

}

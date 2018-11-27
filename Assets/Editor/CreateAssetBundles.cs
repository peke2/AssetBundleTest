using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class CreateAssetBundles
{
	static void prepareOutputDirectory(string assetBundleDirectory = "Assets/AssetBundles")
	{
		//	出力先が無ければ作成(gitで空のディレクトリが保持されないので最初に作る必要がある)
		if (!Directory.Exists(assetBundleDirectory))
		{
			Directory.CreateDirectory(assetBundleDirectory);
		}
	}

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
		prepareOutputDirectory();

		var taDoc = Resources.Load<TextAsset>("Text/document");
		var taInfo = Resources.Load<TextAsset>("Text/info");
		TextAsset[] objs = new TextAsset[]{ taDoc, taInfo};
		bool isSucceeded;
		//isSucceeded = BuildPipeline.BuildAssetBundle(null, objs, "Assets/AssetBundles/text", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		isSucceeded = BuildPipeline.BuildAssetBundle(null, objs, "Assets/AssetBundles/text", BuildAssetBundleOptions.None, BuildTarget.Android);
		Debug.Log("["+((isSucceeded==true)?"成功":"失敗")+ "]<-テキストアセットバンドル作成結果");

		//	Json形式のテキストからアセットバンドルを作成
		var jsonText = Resources.Load<TextAsset>("Json/info");
		//var infoBase = JsonUtility.FromJson<InfoBase>(jsonText.text);
		isSucceeded = BuildPipeline.BuildAssetBundle(null, new TextAsset[] { jsonText }, "Assets/AssetBundles/info", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
		Debug.Log("[" + ((isSucceeded == true) ? "成功" : "失敗") + "]<-情報アセットバンドル作成結果");
	}

	[MenuItem("Assets/Build Version Test Asset")]
	static void BuidVersionTestAsset()
	{
		string[] names = new string[] {
			"res0",
			"res1",
			"res2",
			"res3",
			"res4",
			"res5",
		};

		string inputPath = "Assets/Resources/Json";
		string outputPath = "Assets/AssetBundles/json";

		string[] paths = new string[]{
			"local",
			"remote",
		};

		foreach (var path in paths)
		{
			var buildMap = new List<AssetBundleBuild>();

			foreach (var name in names)
			{
				var fileName = Path.Combine(Path.Combine(inputPath, path), name) + ".json";
				if ( !File.Exists(fileName) )
				{
					Debug.Log("スキップ[" + fileName + "]");
					continue;
				}

				var build = new AssetBundleBuild();
				build.assetNames = new string[] { fileName };
				build.assetBundleName = name;
				buildMap.Add(build);
			}

			if (buildMap.Count == 0) continue;

			string op = Path.Combine(outputPath, path);
			prepareOutputDirectory(op);

			BuildPipeline.BuildAssetBundles(op, buildMap.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
		}
	}

	[MenuItem("Assets/Build Texture Asset")]
	static void BuidTextureAsset()
	{
		string[] names = new string[] {
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			"bg_a_wall.tga",
			//"bg_a_floor.tga",
			//"bg_a_floor.png",
		};

		string inputPath = "Assets/Resources/Textures";
		string outputPath = "Assets/AssetBundles";

		foreach (var name in names)
		{
			var fileName = Path.Combine(inputPath, name);
			if (!File.Exists(fileName))
			{
				Debug.Log("入力ファイル無し[" + fileName + "]");
				continue;
			}

			var build = new AssetBundleBuild();
			//var noExtName = Path.GetFileNameWithoutExtension(name);
			string outputName = name + ".ab";

			build.assetNames = new string[] { fileName };
			build.assetBundleName = outputName;

			prepareOutputDirectory(outputPath);
			AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outputPath, new AssetBundleBuild[] { build }, BuildAssetBundleOptions.None, BuildTarget.Android);
			Debug.Log("アセットバンドル作成完了[" + fileName + "]");
		}

	}
}

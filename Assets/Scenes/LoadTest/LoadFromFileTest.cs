using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadFromFileTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/bg_a_floor.tga.unity3d");
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/bg_a_floor.tga.unity3d.ios");
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/cage_b_cactus1.tga.unity3d");
		var path = Path.Combine(Application.streamingAssetsPath, "temp/cage_b_frame.tga.unity3d");
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/text.windows");
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/text.android");
		//var ab = AssetBundle.LoadFromFile("Assets/StreamingAssets/temp/bg_a_wall.tga.unity3d");
		//var ab = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "temp/sandpit_a-blue.tga.unity3d"));
		//var path = Path.Combine(Application.streamingAssetsPath, "temp/test");
		//var path = Application.streamingAssetsPath + "/" + "temp/test.json";

		var bytes = File.ReadAllBytes(path);

		var ab = AssetBundle.LoadFromFile(path);
		if (ab)
		{
			var names = ab.GetAllAssetNames();
			foreach (var name in names)
			{
				Debug.Log(name);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

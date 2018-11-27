using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextureLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//string fname = Path.Combine(Application.dataPath, "AssetBundles/bg_a_wall.tga.ab");
		string fname = Path.Combine(Application.dataPath, "AssetBundles/bg_a_floor.tga.ab");
		//string fname = Path.Combine(Application.dataPath, "AssetBundles/bg_a_floor.png.ab");
		var ab = AssetBundle.LoadFromFile(fname);
		string[] names = ab.GetAllAssetNames();
		Texture tex = ab.LoadAsset<Texture>(names[0]);

		var renderer = GetComponent<Renderer>();
		renderer.material.SetTexture("_MainTex", tex);

	}

	// Update is called once per frame
	void Update () {
		
	}
}

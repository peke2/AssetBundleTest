using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFromWeb : MonoBehaviour {

	// Use this for initialization
	void Start () {
		load();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator load()
	{
		string url = "http://172.16.10.208:50080/AssetBundleWindows";
		WWW www = WWW.LoadFromCacheOrDownload(url, 1);

		yield return www;
		AssetBundle bundle = www.assetBundle;

		AssetBundleRequest request = bundle.LoadAssetAsync("block/crate", typeof(Texture));
		yield return request;

		Texture tex = request.asset as Texture;
		bundle.Unload(false);
		www.Dispose();
	}

}

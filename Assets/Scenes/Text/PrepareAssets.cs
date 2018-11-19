using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PrepareAssets : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//StartCoroutine(load());
		StartCoroutine(loadFromWeb());
	}

	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator load()
	{
		var path = Application.streamingAssetsPath;
		var request = AssetBundle.LoadFromFileAsync(Path.Combine(path, "text"));
		yield return request;

		var assetBundle = request.assetBundle;
		if(assetBundle != null)
		{
			Debug.Log("アセットバンドル読み込み完了");

			var ta1 = assetBundle.LoadAsset<TextAsset>("info");
			Debug.Log("内容[" + ta1.text + "]");

			var ta2 = assetBundle.LoadAsset<TextAsset>("document");
			Debug.Log("内容[" + ta2.text + "]");
		}
		else
		{
			Debug.Log("アセットバンドル読み込み失敗");
		}
	}


	IEnumerator loadFromWeb()
	{
		while(!Caching.ready)
		{
			yield return null;
		}


		int version = 8;    //	このバージョンでキャッシュに残る？ → キャッシュに残るバージョンと同じ場合、キャッシュから読む

		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle("http://127.0.0.1:24080/info", 0);
		yield return request.Send();
		var assetBundle = DownloadHandlerAssetBundle.GetContent(request);
		var textAsset = assetBundle.LoadAsset<TextAsset>("info");
		var info = JsonUtility.FromJson<InfoBase>(textAsset.text);
		Debug.Log("バージョン[" + info.version.ToString() + "]");
		Debug.Log("取得済み情報数=" + info.elements.Length.ToString());
		foreach (var elem in info.elements)
		{
			Debug.Log("name=[" + elem.name + "]");
		}
	}
}

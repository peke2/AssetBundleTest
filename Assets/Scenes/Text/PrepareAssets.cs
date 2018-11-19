using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class PrepareAssets : MonoBehaviour {

	bool isLoading = false;

	Dictionary<string, AssetBundle> loadedAssetBundles;

	/**
	 *	アセットバンドルがあるか？
	 */
	bool existsAssetBundle(string name)
	{
		return loadedAssetBundles.ContainsKey(name);
	}

	/**
	 *	読み込み済みのアセットバンドルを取得する 
	 */
	AssetBundle getLoadedAssetBundle(string name)
	{
		if (!existsAssetBundle(name)) return null;
		return loadedAssetBundles[name];
	}

	/**
	 *	指定されたアセットバンドルを解放
	 */
	void releaseAssetBundle(string name)
	{
		if (!existsAssetBundle(name)) return;

		loadedAssetBundles[name].Unload(true);	//	バンドル内の全てのアセットを解放
		loadedAssetBundles.Remove(name);
	}


	private void Awake()
	{
		loadedAssetBundles = new Dictionary<string, AssetBundle>();
	}

	// Use this for initialization
	void Start () {
		//StartCoroutine(load());
		//StartCoroutine(loadFromWeb());
		StartCoroutine(loadMulti());	//	同じアセットバンドルを読み込んだ時のエラー挙動確認用
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

	/**
	 *	同じアセットを読み込んだ時に「既にアセットがある」エラーが出るので、同じアセットバンドルを複数回読み込んでみる 
	 */
	IEnumerator loadMulti()
	{
		int loadCount = 0;
		do
		{
			yield return StartCoroutine(loadFromWeb());
			if (isLoading) yield return null;
			loadCount++;
		} while (loadCount < 2);
	}


	IEnumerator loadFromWeb()
	{
		string path = "http://127.0.0.1:24080/info";

		//	二重に読み込まないための処理
		if( existsAssetBundle(path) )
		{
			yield break;
		}

		isLoading = true;

		//	WWW.LoadFromCacheOrDownload() は obsolete なので置き換える
		UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(path, 0);
		yield return request.SendWebRequest();

		if (request.isNetworkError || request.isHttpError)
		{
			Debug.Log(request.error);
		}
		else
		{
			//複数回読み込んだ場合の「既にアセットがあるエラー」は、アセットバンドルの問題ではなく、
			//同じアセットを複数保持することが出来ないだけだと思われる…
			var assetBundle = DownloadHandlerAssetBundle.GetContent(request);
			var textAsset = assetBundle.LoadAsset<TextAsset>("info");
			var info = JsonUtility.FromJson<InfoBase>(textAsset.text);
			Debug.Log("バージョン[" + info.version.ToString() + "]");
			Debug.Log("取得済み情報数=" + info.elements.Length.ToString());
			foreach (var elem in info.elements)
			{
				Debug.Log("name=[" + elem.name + "]");
			}

			//	二重で同じアセットバンドルを読み込まないようにするため保持しておく
			loadedAssetBundles[path] = assetBundle;
		}
		isLoading = false;
	}
}

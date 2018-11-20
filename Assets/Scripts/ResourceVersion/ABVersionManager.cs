using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class ABVersionManager : MonoBehaviour {

	public class MergedInfo{
		public enum ResourceType{
			LOCAL,
			REMOTE,
		}

		public ABVersionInfo.Info info;
		public ResourceType type;

		public MergedInfo(ABVersionInfo.Info info, ResourceType type)
		{
			this.info = info;
			this.type = type;
		}
	}


	Dictionary<string, MergedInfo> versionInfos = new Dictionary<string, MergedInfo>();



	void Awake()
	{
		//リモートのバージョンリストを取得
			//ここで取得できなければローカルのアセットバンドルのみ使う？
				//既にキャッシュ上にアセットバンドルがある場合は？
					//可能ならそれを使いたい
						//どうやって判断？
							//一度読み込み終わった後、マージ済みのバージョンリストをローカルストレージに保持する必要あり
		//ローカルのバージョンリストを取得
		//マージ
			//リモートから取得できていなければローカルストレージ上のバージョンリストを取得
				//リソースとバージョンのリストを作成
					//更新されない場合の読み込みに使用するためローカルストレージに保持
		//リモートのアセットバンドルを列挙
			//キャッシュ上にある？
				//無ければリストアップ
					//リモートから取得するアセットバンドルのリストを作成
						//ここからサイズも取得可能
	}


	void Start()
	{
		//StartCoroutine(loadVersionInfosFromRemote("http://127.0.0.1:24080/info.json"));
		StartCoroutine(loadVersionInfos());
	}


	/// <summary>
	/// ファイルバージョン情報を追加
	/// </summary>
	/// <param name="infos">ファイル情報の配列</param>		
	public void addInfos(ABVersionInfo.Info[] infos, MergedInfo.ResourceType type)
	{
		if(infos == null)
		{
			return;
		}

		foreach (ABVersionInfo.Info info in infos)
		{
			string key = info.name;
			//	バージョンが上の情報を残す
			bool contains = versionInfos.ContainsKey(key);
			if (!contains || (info.version > versionInfos[key].info.version))
			{
				versionInfos[key] = new MergedInfo(info, type);
			}
		}
	}

	public Dictionary<string, MergedInfo> getFileVersionIfnos()
	{
		return versionInfos;
	}


	//[マージ確認の項目]
	//ローカルファイルが無い
	//リモートファイルが無い
	//ローカルのバージョンが高い
	//リモートのバージョンが高い
	//どちらも同じバージョン
	//ローカルにのみ項目がある
	//リモートにのみ項目がある


	ABVersionInfo remoteVersionInfos;
	ABVersionInfo localVersionInfos;
	IEnumerator loadVersionInfos()
	{
		remoteVersionInfos = null;
		localVersionInfos = null;

		yield return loadVersionInfosFromRemote("http://127.0.0.1:24080/info.json");
		loadVersionInfosFromLocal(Path.Combine(Application.streamingAssetsPath, "info.json"));

		addInfos(remoteVersionInfos.infos, MergedInfo.ResourceType.REMOTE);
		addInfos(localVersionInfos.infos, MergedInfo.ResourceType.LOCAL);
	}

	IEnumerator loadVersionInfosFromRemote(string url)
	{
		UnityWebRequest request = UnityWebRequest.Get(url);
		yield return request.SendWebRequest();

		if( request.isNetworkError || request.isHttpError )
		{
			Debug.Log(request.error);
		}
		else
		{
			var versionInfos = JsonUtility.FromJson<ABVersionInfo>(request.downloadHandler.text);
			Debug.Log("リモートバージョン情報取得成功");
			Debug.Log("ファイルバージョン["+versionInfos.version+"]");
			foreach (var info in versionInfos.infos)
			{
				Debug.Log(info.name+"["+info.version.ToString()+"]");
			}
			remoteVersionInfos = versionInfos;
		}
	}

	void loadVersionInfosFromLocal(string path)
	{
		if( !File.Exists(path) )
		{
			Debug.Log("ファイル["+path+"]が開けません");
			return;
		}
		string text = File.ReadAllText(path);

		var versionInfos = JsonUtility.FromJson<ABVersionInfo>(text);
		Debug.Log("ローカルバージョン情報取得成功");
		Debug.Log("ファイルバージョン[" + versionInfos.version + "]");
		foreach (var info in versionInfos.infos)
		{
			Debug.Log(info.name + "[" + info.version.ToString() + "]");
		}
		localVersionInfos = versionInfos;
	}


}

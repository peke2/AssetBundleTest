using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class ABVersionManager : MonoBehaviour {

	[Serializable]
	public class Info{
		public enum ResourceType{
			LOCAL,
			REMOTE,
		}

		public ABVersionInfo.Element element;
		public ResourceType type;

		public Info(ABVersionInfo.Element element, ResourceType type)
		{
			this.element = element;
			this.type = type;
		}
	}

	//	マージ済みのバージョン情報
	Dictionary<string, Info> mergedVersionInfos = new Dictionary<string, Info>();
	bool isReady = false;

	/// <summary>
	/// バージョン管理情報が準備済みか？
	/// </summary>
	/// <returns>true 準備済み</returns>
	public bool isVersionInfosReady()
	{
		return isReady;
	}

	/// <summary>
	/// バージョン情報を取得
	/// </summary>
	/// <returns></returns>
	public Dictionary<string, Info> getVersionInfos()
	{
		return new Dictionary<string, Info>(mergedVersionInfos);
	}

#if false
	[Serializable]
	public class CurrentVersionInfosContainer
	{
		public Info[] infos;

		public CurrentVersionInfosContainer(Dictionary<string, Info> mergedInfos)
		{
			infos = new Info[mergedInfos.Count];

			int count = 0;
			foreach (var key in mergedInfos.Keys)
			{
				var info = mergedInfos[key];
				infos[count++] = info;
			}
		}
	}
#endif

	const string VERSION_INFO_FILE_NAME = "version_info.json";
	//const string REMOTE_RESOURCE_URL = "http://127.0.0.1:24080/remote/";
	//public string RemoteRsourceUrl
	//{
	//	get; set;
	//}


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
	}

	//void initVersionInfos(string remoteUrl=null)
	//{
	//	StartCoroutine(readVersionInfos());
	//}



	/// <summary>
	/// ファイルバージョン情報を追加
	/// </summary>
	/// <param name="elements">ファイル情報の配列</param>		
	public void addInfos(ABVersionInfo info, Info.ResourceType type)
	{
		if( info == null )
		{
			return;
		}

		ABVersionInfo.Element[] elements = info.elements;
		if ( elements == null )
		{
			return;
		}

		foreach (var element in elements)
		{
			string key = element.name;
			//	バージョンが上の情報を残す
			//	バージョンが同じ場合ローカルを優先させて残す(余計な取得を無くす)
			bool contains = mergedVersionInfos.ContainsKey(key);
			if (!contains || (element.version > mergedVersionInfos[key].element.version) || (type==Info.ResourceType.LOCAL && element.version==mergedVersionInfos[key].element.version))
			{
				mergedVersionInfos[key] = new Info(element, type);
			}
		}
	}

	//public Dictionary<string, Info> getFileVersionIfnos()
	//{
	//	return mergedVersionInfos;
	//}


	//[マージ確認の項目]
	//ローカルファイルが無い
	//リモートファイルが無い
	//ローカルのバージョンが高い
	//リモートのバージョンが高い
	//どちらも同じバージョン
	//ローカルにのみ項目がある
	//リモートにのみ項目がある


	string combineUrl(string baseStr, string relativeStr)
	{
		Uri baseUri = new Uri(baseStr);
		Uri uri = new Uri(baseUri, relativeStr);
		return uri.AbsoluteUri;
	}

	ABVersionInfo remoteVersionInfos;
	ABVersionInfo localVersionInfos;
	public IEnumerator readVersionInfos(string remoteUrl=null)
	{
		isReady = false;

		remoteVersionInfos = null;
		localVersionInfos = null;

		//	リモートとローカルからバージョン情報ファイルを取得
		if (remoteUrl != null)
		{
			yield return readVersionInfosFromRemote(combineUrl(remoteUrl, VERSION_INFO_FILE_NAME));
		}
		readVersionInfosFromLocal(Path.Combine(Application.streamingAssetsPath, Path.Combine("local", VERSION_INFO_FILE_NAME)) );

		addInfos(remoteVersionInfos, Info.ResourceType.REMOTE);
		addInfos(localVersionInfos, Info.ResourceType.LOCAL);

		//確認用でここに入れてみる
		//yield return updateAssetBundles(REMOTE_RESOURCE_URL);
		isReady = true;
	}


	IEnumerator readVersionInfosFromRemote(string url)
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
			foreach (var info in versionInfos.elements)
			{
				Debug.Log(info.name+"["+info.version.ToString()+"]");
			}
			remoteVersionInfos = versionInfos;
		}
	}

	void readVersionInfosFromLocal(string path)
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
		foreach (var info in versionInfos.elements)
		{
			Debug.Log(info.name + "[" + info.version.ToString() + "]");
		}
		localVersionInfos = versionInfos;
	}

	//以下、対応は見送り
#if false
	IEnumerator updateAssetBundles(string remoteUrl)
	{
		string path = Path.Combine(Application.persistentDataPath, "current_version_infos.json");

		Dictionary<string, Info> currentMergedInfo = readCurrentVersionInfos(path);

		Dictionary<string, Info> updateTargets = retrieveUpdateTarget(mergedVersionInfos, currentMergedInfo);

		//	更新が無ければ何もしない
		if( updateTargets.Count == 0 )
		{
			Debug.Log("アセットバンドルの更新無し");
			yield break;
		}

		//	更新前に一度削除
		deleteCurrentVersionInfos(path);

		//アセットバンドルの更新
		foreach(var key in updateTargets.Keys)
		{
			if(updateTargets[key].type == Info.ResourceType.LOCAL )
			{
				continue;
			}

			//ストレージのキャッシュに乗せる
			ABVersionInfo.Element info = updateTargets[key].element;
			string uri = combineUrl(remoteUrl, info.name);
			using (var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET))
			{
				request.downloadHandler = new DownloadHandlerAssetBundle(path, info.version, 0);
				yield return request.SendWebRequest();

				Debug.Log("["+path+"] is downloaded");
				//AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
			}
		}

		Debug.Log("アセットバンドルの更新完了");

		//	マージ済みの情報を書き込む
		writeCurrentVersionInfos(path, mergedVersionInfos);
	}


	Dictionary<string, Info> retrieveUpdateTarget(Dictionary<string, Info> newInfos, Dictionary<string, Info> currentInfos)
	{
		//	現在の情報が無ければ新規をそのまま使う
		if( currentInfos == null )
		{
			return newInfos;
		}

		//差分の抽出
		//新規(new側にのみある)
		//削除(current側にのみある) → Unity管理のキャッシュから削除する方法はある？
		//新規(new)側のバージョンが上

		var updatedInfos = new Dictionary<string, Info>();

		//削除に関しては保留
		foreach (var key in newInfos.Keys)
		{
			var newi = newInfos[key];

			if ( newi.type == Info.ResourceType.LOCAL )
			{
				//	アップデート対象はリモートのみ
				//	ローカルのバージョンが上の場合は無視
				continue;
			}

			if ( !currentInfos.ContainsKey(key) )
			{
				//	新規
				updatedInfos[key] = newi;
				continue;
			}

			var curi = currentInfos[key];
			if( newi.element.version > curi.element.version )
			{
				//	更新対象
				updatedInfos[key] = newi;
			}
		}

		return updatedInfos;
	}



	/// <summary>
	/// 現在(読み込み済み)のバージョン情報を削除
	/// </summary>
	void deleteCurrentVersionInfos(string path)
	{
		if (!File.Exists(path)) return;
		File.Delete(path);
	}

	/// <summary>
	/// 現在(読み込み済み)のバージョン情報を書き込み
	/// </summary>
	Dictionary<string, Info> readCurrentVersionInfos(string path)
	{
		if( !File.Exists(path))
		{
			Debug.Log("ファイル["+path+"]が無いので読み込みをスキップ");
			return null;
		}

		var jsonText = File.ReadAllText(path);
		var container = JsonUtility.FromJson<CurrentVersionInfosContainer>(jsonText);

		var mergedInfos = new Dictionary<string, Info>();
		foreach(var info in container.infos)
		{
			mergedInfos[info.element.name] = info;
		}
		return mergedInfos;
	}

	/// <summary>
	/// 現在(読み込み済み)のバージョン情報を書き込み
	/// </summary>
	void writeCurrentVersionInfos(string path, Dictionary<string, Info> mergedInfos)
	{
		var container = new CurrentVersionInfosContainer(mergedInfos);
		var jsonText = JsonUtility.ToJson(container);

		//string path = Path.Combine(Application.persistentDataPath, "");

		File.WriteAllText(path, jsonText);
	}
#endif
}

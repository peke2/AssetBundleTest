using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.IO;

public class ABVersionManager : MonoBehaviour {

	[Serializable]
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


	Dictionary<string, MergedInfo> mergedVersionInfos = new Dictionary<string, MergedInfo>();

	[Serializable]
	public class CurrentVersionInfosContainer
	{
		public MergedInfo[] infos;

		public CurrentVersionInfosContainer(Dictionary<string, MergedInfo> mergedInfos)
		{
			infos = new MergedInfo[mergedInfos.Count];

			int count = 0;
			foreach (var key in mergedInfos.Keys)
			{
				var info = mergedInfos[key];
				infos[count++] = info;
			}
		}
	}

	const string VERSION_INFO_FILE_NAME = "version_info.json";
	const string REMOTE_RESOURCE_URL = "http://127.0.0.1:24080/remote/";


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
		StartCoroutine(readVersionInfos());
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
			//	バージョンが同じ場合ローカルを優先させて残す(余計な取得を無くす)
			bool contains = mergedVersionInfos.ContainsKey(key);
			if (!contains || (info.version > mergedVersionInfos[key].info.version) || (type==MergedInfo.ResourceType.LOCAL && info.version==mergedVersionInfos[key].info.version))
			{
				mergedVersionInfos[key] = new MergedInfo(info, type);
			}
		}
	}

	public Dictionary<string, MergedInfo> getFileVersionIfnos()
	{
		return mergedVersionInfos;
	}


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
	IEnumerator readVersionInfos()
	{
		remoteVersionInfos = null;
		localVersionInfos = null;

		yield return readVersionInfosFromRemote( combineUrl(REMOTE_RESOURCE_URL, VERSION_INFO_FILE_NAME));
		readVersionInfosFromLocal(Path.Combine(Application.streamingAssetsPath, Path.Combine("local", VERSION_INFO_FILE_NAME)) );

		addInfos(remoteVersionInfos.infos, MergedInfo.ResourceType.REMOTE);
		addInfos(localVersionInfos.infos, MergedInfo.ResourceType.LOCAL);

		yield return updateAssetBundles(REMOTE_RESOURCE_URL);
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
			foreach (var info in versionInfos.infos)
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
		foreach (var info in versionInfos.infos)
		{
			Debug.Log(info.name + "[" + info.version.ToString() + "]");
		}
		localVersionInfos = versionInfos;
	}


	IEnumerator updateAssetBundles(string remoteUrl)
	{
		string path = Path.Combine(Application.persistentDataPath, "current_version_infos.json");

		Dictionary<string, MergedInfo> currentMergedInfo = readCurrentVersionInfos(path);

		Dictionary<string, MergedInfo> updateTargets = retrieveUpdateTarget(mergedVersionInfos, currentMergedInfo);

		//	更新前に一度削除
		deleteCurrentVersionInfos(path);

		//アセットバンドルの更新
		foreach(var key in updateTargets.Keys)
		{
			if(updateTargets[key].type == MergedInfo.ResourceType.LOCAL )
			{
				continue;
			}

			ABVersionInfo.Info info = updateTargets[key].info;
			string uri = combineUrl(remoteUrl, info.name);
			using (var request = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbGET))
			{
				request.downloadHandler = new DownloadHandlerAssetBundle(path, info.version, 0);
				yield return request.SendWebRequest();

				Debug.Log("["+path+"] is downloaded");
				//これでキャッシュには乗る？
				//AssetBundle assetBundle = DownloadHandlerAssetBundle.GetContent(request);
			}
		}

		//	更新後の情報で書き込み
		writeCurrentVersionInfos(path, updateTargets);
	}


	Dictionary<string, MergedInfo> retrieveUpdateTarget(Dictionary<string, MergedInfo> newInfos, Dictionary<string, MergedInfo> currentInfos)
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

		var updatedInfos = new Dictionary<string, MergedInfo>();

		//削除に関しては保留
		foreach (var key in newInfos.Keys)
		{
			var newi = newInfos[key];

			if ( newi.type == MergedInfo.ResourceType.LOCAL )
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
			if( newi.info.version > curi.info.version )
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
	Dictionary<string, MergedInfo> readCurrentVersionInfos(string path)
	{
		if( !File.Exists(path))
		{
			Debug.Log("ファイル["+path+"]が無いので読み込みをスキップ");
			return null;
		}

		var jsonText = File.ReadAllText(path);
		var container = JsonUtility.FromJson<CurrentVersionInfosContainer>(jsonText);

		var mergedInfos = new Dictionary<string, MergedInfo>();
		foreach(var info in container.infos)
		{
			mergedInfos[info.info.name] = info;
		}
		return mergedInfos;
	}

	/// <summary>
	/// 現在(読み込み済み)のバージョン情報を書き込み
	/// </summary>
	void writeCurrentVersionInfos(string path, Dictionary<string, MergedInfo> mergedInfos)
	{
		var container = new CurrentVersionInfosContainer(mergedInfos);
		var jsonText = JsonUtility.ToJson(container);

		//string path = Path.Combine(Application.persistentDataPath, "");

		File.WriteAllText(path, jsonText);
	}

}

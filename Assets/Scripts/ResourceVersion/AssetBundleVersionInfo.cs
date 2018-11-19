using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AssetBundleVersionInfo {

	public enum ResourceType{
		Local,
		Remote,
	}

	[Serializable]
	public class FileVersionInfo
	{
		public string name;
		public int fileSize;
		public string md5;
		public int version;

		public ResourceType resourceType = ResourceType.Local;
	}

	Dictionary<string, FileVersionInfo> versionInfoDict;


	public AssetBundleVersionInfo()
	{
		versionInfoDict = new Dictionary<string, FileVersionInfo>();
	}


	/// <summary>
	/// ファイルバージョン情報を追加
	/// </summary>
	/// <param name="infos">ファイル情報の配列</param>		
	public void addInfos(FileVersionInfo[] infos)
	{
		foreach (FileVersionInfo info in infos)
		{
			string key = info.name;
			//	バージョンが上の情報を残す
			bool contains = versionInfoDict.ContainsKey(key);
			if (!contains || (info.version > versionInfoDict[key].version))
			{
				versionInfoDict[key] = info;
			}
		}
	}

	public Dictionary<string, FileVersionInfo> getFileVersionIfnos()
	{
		return versionInfoDict;
	}


	//ローカルとリモートのファイルバージョン情報をマージ
	//対象がリモートの情報か？
		//現在キャッシュされているアセットバンドルのバージョンを取得
			//リモートが新しいか？
				//リモートからアセットバンドルを読み込み

	//どのアセットバンドルがどのバージョンか？ とうやって管理？


}

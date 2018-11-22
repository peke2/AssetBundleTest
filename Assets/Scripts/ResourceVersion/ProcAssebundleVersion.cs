using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//	アセットバンドルバージョン管理の動作を確認するための呼び出しコンポーネント

public class ProcAssebundleVersion : MonoBehaviour {

	ABVersionManager manager;
	bool isReady = false;

	// Use this for initialization
	void Start () {
		manager = GetComponent<ABVersionManager>();

		StartCoroutine( manager.readVersionInfos("http://127.0.0.1:24080/remote/") );
	}
	
	// Update is called once per frame
	void Update ()
	{
		if( !isReady && manager.isVersionInfosReady() )
		{
			Debug.Log("バージョン情報読み込み完了");
			Dictionary<string, ABVersionManager.Info> infos = manager.getVersionInfos();
			foreach(var key in infos.Keys)
			{
				var info = infos[key];
				Debug.Log("key=("+key+") ファイル名["+info.element.name+"] Ver."+info.element.version+"("+info.type.ToString()+")");
			}
			isReady = true;
		}
	}
}

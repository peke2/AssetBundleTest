using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CacheUtil{

	[MenuItem("Tools/キャッシュクリア")]
	static void clear()
	{
		bool isSucceeded = Caching.ClearCache();
		if( isSucceeded == true )
		{
			Debug.Log("キャッシュのクリアに成功");
		}
		else
		{
			Debug.Log("キャッシュのクリアに失敗");
		}

	}
}

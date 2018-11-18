using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(Load());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator Load()
    {
        //WWW www = WWW.LoadFromCacheOrDownload("http://127.0.0.1:20080/share/resources/dialog_bundle", 1);
        //WWW www = new WWW("http://127.0.0.1:20080/dialog_bundle");
        WWW www = new WWW("http://127.0.0.1:20080/share/dialog_bundle");
        yield return www;

        Object[] assets = www.assetBundle.LoadAllAssets();

        GameObject obj = Instantiate(assets[0]) as GameObject;

        GameObject parentObj = GameObject.Find("Canvas");
        obj.transform.SetParent(parentObj.transform);
    }

}

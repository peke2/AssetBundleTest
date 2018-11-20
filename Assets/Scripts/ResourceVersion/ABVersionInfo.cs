using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ABVersionInfo {

	public enum ResourceType{
		Local,
		Remote,
	}

	[Serializable]
	public class Info
	{
		public string name;
		public uint fileSize;
		public string md5;
		public uint version;

		//public ResourceType resourceType = ResourceType.Local;
	}

	public uint version;
	public Info[] infos;

	public ABVersionInfo()
	{
	}
}

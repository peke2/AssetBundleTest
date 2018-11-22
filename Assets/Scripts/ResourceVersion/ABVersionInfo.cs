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
	public class Element
	{
		public string name;
		public uint size;
		public string md5;
		public uint version;

		//public ResourceType resourceType = ResourceType.Local;
	}

	public uint version;
	public Element[] elements;

	public ABVersionInfo()
	{
	}
}

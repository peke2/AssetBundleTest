using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class InfoBase
{
	[Serializable]
	public class Element{
		public string name;
		public int value;
		public string comment;
	}

	public string version;
	public Element[] elements;
}

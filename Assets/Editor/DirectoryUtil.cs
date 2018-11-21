using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DirectoryUtil
{
	[MenuItem("Tools/PersistentDataPathを開く")]
	static void openPersistentDirectory()
	{
		System.Diagnostics.Process.Start(Application.persistentDataPath);
	}
}

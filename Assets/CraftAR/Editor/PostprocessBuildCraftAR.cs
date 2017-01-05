using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEditor.Callbacks;
using System.Diagnostics;
using System.IO;

public class PostprocessBuildCraftAR : MonoBehaviour {
	
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject) {

		#if UNITY_IOS
		Process proc = new Process();
		proc.EnableRaisingEvents=false;
		proc.StartInfo.FileName = Application.dataPath + "/Plugins/CraftAR-iOS/PostBuildIOSScript";
		CDebug.Log("process filename: "+proc.StartInfo.FileName);
		proc.StartInfo.Arguments = "'" + pathToBuiltProject + "'";

		// Add the Unity version as an argument to the postbuild script, use 'Unity3' for all 3.x versions and for
		// 4 and up use the API to get it
		string unityVersion;
		#if UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2 || UNITY_3_1 || UNITY_3_0_0 || UNITY_3_0
		unityVersion = "Unity3";
		#else
		unityVersion = Application.unityVersion;
		#endif
		proc.StartInfo.Arguments += " '" + unityVersion + "'";
		proc.Start();
		proc.WaitForExit();
		#endif
	}

}
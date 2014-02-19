using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneListSaver : EditorWindow
{
	struct SceneInfo
	{
		public string Name;
		public bool Enabled;
	}
	
	static string[] ActiveSceneList = { };
	static string[] InactiveSceneList = { };
	
	static bool SaveList = true;
	
	static int FirstLevel = 0;
	static int LastLevel = -1;
	
	static Dictionary<string, int> ScenesDict = new Dictionary<string, int>();
	
	static int UpdateCounter = 0;
	
	static bool FoldoutActive = false;
	static bool FoldoutInactive = false;
	
	SceneListSaver()
	{
		SceneListReader.Initialise();
		
		FirstLevel = SceneListReader.FirstLevel;
		LastLevel = SceneListReader.LastLevel;
		
		UpdateSceneList();
		
		Repaint();
	}
	
	[MenuItem ("Window/Scene List")]
	public static void ShowWindow()
	{
		EditorWindow.GetWindow<SceneListSaver>("Scene List").Show();
	}

	void OnGUI()
	{
		SaveList = EditorGUILayout.Toggle("Save List", SaveList);
		
		FirstLevel = EditorGUILayout.IntField("First Level", FirstLevel);
		LastLevel = EditorGUILayout.IntField("Last Level", LastLevel);
		
		CheckLevelValues();
		
		GUILayout.BeginVertical();

		GUILayout.Space(10);
		
		GUILayout.EndVertical();
		
		FoldoutActive = EditorGUILayout.Foldout(FoldoutActive, "Scenes in Build");
		
		if (FoldoutActive)
		{
			GUILayout.BeginHorizontal();
	
			GUILayout.Space(20);
			
			GUILayout.BeginVertical();
			
			GUILayout.Space(5);
			
			EditorBuildSettingsScene[] Scenes = EditorBuildSettings.scenes;
			
			for (int S = 0; S < Scenes.Length && S < ActiveSceneList.Length; S++)
			{
				if (Scenes[S].enabled)
					GUILayout.Label(ActiveSceneList[S] + ": " + S);
				else
					GUILayout.Label(ActiveSceneList[S] + ": " + S + " - disabled");
			}
			
			GUILayout.Space(10);
	
			GUILayout.EndVertical();
			
			GUILayout.EndHorizontal();
		}
		
		FoldoutInactive = EditorGUILayout.Foldout(FoldoutInactive, "Scenes not in Build");
		
		if (FoldoutInactive)
		{
			GUILayout.BeginHorizontal();
	
			GUILayout.Space(20);
	
			GUILayout.BeginVertical();
	
			GUILayout.Space(5);
			
			for (int Scene = 0; Scene < InactiveSceneList.Length; Scene++)
				GUILayout.Label(InactiveSceneList[Scene]);
	
			GUILayout.EndVertical();
	
			GUILayout.EndHorizontal();
		}
	}

	void OnInspectorUpdate()
	{
		if (++UpdateCounter >= 10)
			UpdateCounter = 0;
		else
			return;
		
		UpdateSceneList();
		
		Repaint();
	}
	
	static void UpdateSceneList()
	{
		ScenesDict.Clear();
		
		string[] SceneFiles = System.IO.Directory.GetFiles(Application.dataPath, "*.unity", System.IO.SearchOption.AllDirectories);
		
		foreach (string AssetName in SceneFiles)
			ScenesDict.Add(System.IO.Path.GetFileNameWithoutExtension(AssetName), 0);
		
		EditorBuildSettingsScene[] Scenes = EditorBuildSettings.scenes;
		
		if (ActiveSceneList == null || ActiveSceneList.Length != Scenes.Length)
			ActiveSceneList = new string[Scenes.Length];
		
		for (int S = 0; S < Scenes.Length; S++)
		{
			ActiveSceneList[S] = System.IO.Path.GetFileNameWithoutExtension(Scenes[S].path);
			
			ScenesDict[ActiveSceneList[S]] = Scenes[S].enabled ? 2 : 1;
		}
		
		if (InactiveSceneList == null || InactiveSceneList.Length != ScenesDict.Count - Scenes.Length)
			InactiveSceneList = new string[ScenesDict.Count - Scenes.Length];
		
		int Inactive = 0;
		
		foreach (KeyValuePair<string, int> KVP in ScenesDict)
			if (KVP.Value == 0)
				InactiveSceneList[Inactive++] = KVP.Key;
	}

	static void CheckLevelValues()
	{
		if (FirstLevel < 0)
			FirstLevel = 0;

		if (FirstLevel > ActiveSceneList.Length - 1)
			FirstLevel = ActiveSceneList.Length - 1;

		if (LastLevel < 0 || LastLevel > ActiveSceneList.Length - 1)
			LastLevel = ActiveSceneList.Length - 1;

		if (LastLevel < FirstLevel)
			LastLevel = FirstLevel;
	}

	public class LevelAMP : UnityEditor.AssetModificationProcessor
	{
		public static string[] OnWillSaveAssets(string[] AssetFiles)
		{
			SceneListReader.Initialise();
			
			CheckLevelValues();

			if (!SaveList || ActiveSceneList.Length == 0)
				return AssetFiles;
			
			string ResourcesFolder = Application.dataPath + "/Resources/";

			if (!System.IO.Directory.Exists(ResourcesFolder))
				System.IO.Directory.CreateDirectory(ResourcesFolder);

			System.IO.File.WriteAllText(ResourcesFolder + "Level List.bytes", ActiveSceneList.Length + "\n" + FirstLevel + "\n" + LastLevel + "\n" + string.Join("\n", ActiveSceneList));

			return AssetFiles;
		}
	}
}
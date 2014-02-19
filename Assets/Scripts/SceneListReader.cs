using System.IO;
using UnityEngine;

public static class SceneListReader
{
	public static string[] SceneList = { "" };
	public static string[] LevelList = { "" };
	
	public static int FirstLevel { get; private set; }
	public static int LastLevel { get; private set; }
	
	public static void Initialise()
	{
#if UNITY_EDITOR
		if (!File.Exists("Assets/Resources/Level List.bytes"))
		{
			FirstLevel = 0;
			LastLevel = UnityEditor.EditorBuildSettings.scenes.Length - 1;
			return;
		}
		
		StreamReader SR = new StreamReader(new FileStream("Assets/Resources/Level List.bytes", FileMode.Open));
#else
		TextAsset TA = (TextAsset)Resources.Load("Level List");
		
		if (TA == null)
		{
			FirstLevel = 0;
			LastLevel = Application.levelCount - 1;
			return;
		}
		
		StreamReader SR = new StreamReader(new MemoryStream(TA.bytes));
#endif

		int SceneCount = int.Parse(SR.ReadLine());
		FirstLevel = int.Parse(SR.ReadLine());
		LastLevel = int.Parse(SR.ReadLine());
		
		SceneList = new string[SceneCount];
		LevelList = new string[LastLevel - FirstLevel + 1];

		int Scene = 0;
		int Level = 0;

		while (!SR.EndOfStream)
		{
			SceneList[Scene] = SR.ReadLine();

			if (Scene >= FirstLevel && Scene <= LastLevel)
				LevelList[Level++] = SceneList[Scene];

			Scene++;
		}
		
		SR.Close();
	}
}
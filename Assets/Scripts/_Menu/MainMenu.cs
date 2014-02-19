using UnityEngine;

public class MainMenu : MonoBehaviour
{
	int LastLevel = -1;
	
	bool LevelSelect = false;
	
	bool Options = false;
	
	void Awake()
	{
		LastLevel = PlayerPrefs.GetInt("Last Played Level", -1);
		
		SceneListReader.Initialise();
		
		if (Debug.isDebugBuild)
			Level.HighestLevel = SceneListReader.LastLevel;
	}
	
	void OnGUI()
	{
		if (LevelSelect)
		{
			LevelSelectGUI();
			return;
		}
		
		if (Options)
		{
			OptionsGUI();
			return;
		}
		
		float Y = Screen.height * 0.3f;
		float Inc = Screen.height * 0.1f;
		
		if (LastLevel >= 0 && GUI.Button(new Rect(Screen.width * 0.3f, Y, Screen.width * 0.4f, Screen.height * 0.1f), "Continue"))
			Level.LoadLevel(LastLevel);
		
		Y += Inc;
		
		if (GUI.Button(new Rect(Screen.width * 0.3f, Y, Screen.width * 0.4f, Screen.height * 0.1f), "Start"))
			Level.LoadLevel(SceneListReader.FirstLevel);
		
		Y += Inc;
		
		if (GUI.Button(new Rect(Screen.width * 0.3f, Y, Screen.width * 0.4f, Screen.height * 0.1f), "Level Select"))
			LevelSelect = true;
		
		Y += Inc;
		
		if (GUI.Button(new Rect(Screen.width * 0.3f, Y, Screen.width * 0.4f, Screen.height * 0.1f), "Options"))
			Options = true;
		
		Y += Inc;
		
		if (GUI.Button(new Rect(Screen.width * 0.3f, Y, Screen.width * 0.4f, Screen.height * 0.1f), "Quit"))
#if UNITY_EDITOR
			UnityEditor.EditorApplication.ExecuteMenuItem("Edit/Play");
#else
			Application.Quit();
#endif
	}
	
	void LevelSelectGUI()
	{
		for (int Lvl = 0; Lvl < SceneListReader.LevelList.Length; Lvl++)
		{
			string LevelName = SceneListReader.LevelList[Lvl];
			
			if (Lvl > Level.HighestLevel)
			{
				GUI.color = Color.grey;
			
				GUI.Button(new Rect(Screen.width * 0.1f * (Lvl % 8 + 1), Screen.height * 0.1f * (Lvl / 8 + 2), Screen.width * 0.1f, Screen.height * 0.1f), LevelName);
			}
			else if (GUI.Button(new Rect(Screen.width * 0.1f * (Lvl % 8 + 1), Screen.height * 0.1f * (Lvl / 8 + 2), Screen.width * 0.1f, Screen.height * 0.1f), LevelName))
			{
				Level.LoadLevel(LevelName);
				return;
			}
		}
		
		if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Main Menu"))
			LevelSelect = false;
	}
	
	void OptionsGUI()
	{
		GUI.Box(new Rect(Screen.width * 0.4f, Screen.height * 0.45f, Screen.width * 0.2f, Screen.height * 0.1f), "No options yet :D");
		
		if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Main Menu"))
			Options = false;
	}
}
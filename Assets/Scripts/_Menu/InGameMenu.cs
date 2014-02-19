using UnityEngine;

public class InGameMenu : MonoBehaviour
{
	public bool Paused = false;
	
	void Awake()
	{
		Level.HighestLevel = PlayerPrefs.GetInt("Highest Level", SceneListReader.FirstLevel);
	}
	
	void OnGUI()
	{
		GUI.skin.label.alignment = TextAnchor.MiddleCenter;
		
		GUI.skin.box.wordWrap = true;
		
		GUI.Label(new Rect(Screen.width * 0.4f, 0.0f, Screen.width * 0.2f, Screen.height * 0.05f), Application.loadedLevelName);
		
		GUI.Box(new Rect(Screen.width * 0.3f, Screen.height * 0.05f, Screen.width * 0.4f, Screen.height * 0.1f), Level.CurrentLevel.LevelInfo);
		
		if (Level.CurrentLevel.Win)
			GUI.Label(new Rect(Screen.width * 0.4f, Screen.height * 0.15f, Screen.width * 0.2f, Screen.height * 0.1f), "Victory");
	
		if (Button(new Rect(Screen.width * 0.025f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.1f), "Main Menu"))
			Level.LoadLevel("Main Menu");
		
		if (Button(new Rect(Screen.width * 0.775f, Screen.height * 0.05f, Screen.width * 0.2f, Screen.height * 0.1f), "Reset"))
			Level.CurrentLevel.ResetLevel();
		
		if (Button(new Rect(Screen.width * 0.025f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Level Select"))
		{
			Time.timeScale = 0.0f;
			Paused = true;
		}
		
		if (Application.loadedLevel + 1 >= Application.levelCount)
		{
			if (Button(new Rect(Screen.width * 0.775f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Restart Game"))
				Level.LoadLevel(SceneListReader.FirstLevel);
		}
		else if (Application.loadedLevel >= Level.HighestLevel)
		{
			GUI.color = Color.grey;
		
			Button(new Rect(Screen.width * 0.775f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Next Level");
		}
		else if (Button(new Rect(Screen.width * 0.775f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Next Level"))
			Level.LoadLevel(Application.loadedLevel + 1);
		
		GUI.color = Color.white;
		
		if (Paused)
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
					Time.timeScale = 1.0f;
					Paused = false;
					Level.LoadLevel(LevelName);
					return;
				}
			}
			
			GUI.color = Color.white;
			
			if (GUI.Button(new Rect(Screen.width * 0.4f, Screen.height * 0.85f, Screen.width * 0.2f, Screen.height * 0.1f), "Resume"))
			{
				Time.timeScale = 1.0f;
				Paused = false;
			}
		}
	}
	
	bool Button(Rect ScreenRect, string Text)
	{
		Color LastColour = GUI.color;
		
		if (Paused)
			GUI.color = Color.grey;
		
		bool Val = GUI.Button(ScreenRect, Text) && !Paused;
		
		GUI.color = LastColour;
		
		return Val;
	}
}
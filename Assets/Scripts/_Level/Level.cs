using UnityEngine;

public class Level : MonoBehaviour
{
	#region Variables
	
	public static Level CurrentLevel = null;
	
	Transform[] Trs = null;
	Vector3[] StartPositions = null;
	Quaternion[] StartRotations = null;
	Rigidbody[] Rigids = null;
	Target[] Targets = null;
	MovingPlatform[] Platforms = null;
	Windmill[] Windmills = null;
	Flipper[] Flippers = null;
	Barrel[] Barrels = null;
	
	public static int HighestLevel = 0;
	
	public bool Win = false;
	
	public bool SleepOnStart = true;
	
	public string LevelInfo = null;
	
	// analytics stuff
	int ResetCount = 0;
	float LevelTime = 0.0f;
	
	#endregion
	
	Level()
	{
		CurrentLevel = this;
		
#if UNITY_EDITOR
		SceneListReader.Initialise();
#endif
	}
	
	void Awake()
	{
		Trs = GetComponentsInChildren<Transform>();
		
		StartPositions = new Vector3[Trs.Length];
		StartRotations = new Quaternion[Trs.Length];
		Rigids = new Rigidbody[Trs.Length];
		Targets = GetComponentsInChildren<Target>();
		Platforms = GetComponentsInChildren<MovingPlatform>();
		Windmills = GetComponentsInChildren<Windmill>();
		Flippers = GetComponentsInChildren<Flipper>();
		Barrels = GetComponentsInChildren<Barrel>();
		
		for (int Tr = 0; Tr < Trs.Length; Tr++)
		{
			StartPositions[Tr] = Trs[Tr].localPosition;
			StartRotations[Tr] = Trs[Tr].localRotation;
			Rigids[Tr] = Trs[Tr].rigidbody;
			
			if (Rigids[Tr] && SleepOnStart)
				Rigids[Tr].Sleep();
		}
	}
	
	void Update()
	{
		LevelTime += Time.deltaTime;
		
		
#if UNITY_ANDROID || UNITY_IPHONE
		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			Vector3 MousePos = Input.touches[0].position;
			
#else
		if (Input.GetMouseButtonDown(0))
		{
			Vector3 MousePos = Input.mousePosition;
#endif
			
			MousePos.z = 20.0f;
				
			RaycastHit Hit;
			
			if (Physics.Raycast(Camera.main.ScreenPointToRay(MousePos), out Hit))
			{
				Barrel Brl = Hit.collider.GetComponent<Barrel>();
					
				if (Brl != null)
					Brl.Activate();
			}
		}
	}
	
	void FixedUpdate()
	{
		foreach (Target T in Targets)
			if (!T.AtDestination)
				return;
		
		// level complete
		if (!Win)
		{
			Win = true;
			
			if (Application.loadedLevel + 1 > HighestLevel)
			{
				HighestLevel = Application.loadedLevel + 1;
				PlayerPrefs.SetInt("Highest Level", HighestLevel);
			}
		}
	}
	
	public void ResetLevel()
	{
		for (int Tr = 0; Tr < Trs.Length; Tr++)
		{
			Trs[Tr].localPosition = StartPositions[Tr];
			Trs[Tr].localRotation = StartRotations[Tr];
			
			if (Rigids[Tr] && !Rigids[Tr].isKinematic)
			{
				Rigids[Tr].velocity = Vector3.zero;
				Rigids[Tr].angularVelocity = Vector3.zero;
				
				if (SleepOnStart)
					Rigids[Tr].Sleep();
			}
		}
		
		foreach (Target T in Targets)
			T.Reset();
		
		foreach (MovingPlatform MP in Platforms)
			MP.Reset();
		
		foreach (Windmill W in Windmills)
			W.Reset();
		
		foreach (Flipper F in Flippers)
			F.Reset();
		
		foreach (Barrel B in Barrels)
			B.Reset();
		
		Win = false;
		
		ResetCount++;
	}
	
	public static void LoadLevel(string LevelName)
	{
		int LevelNo = -1;
		
		for (int Lvl = 0; Lvl < SceneListReader.LevelList.Length; Lvl++)
		{
			if (SceneListReader.LevelList[Lvl] == LevelName)
			{
				LevelNo = Lvl;
				break;
			}
		}
		
		if (SceneToLevelNumber(LevelNo) >= 0)
			CurrentLevel.StartCoroutine("Upload", new string[] { Application.loadedLevelName, CurrentLevel.LevelTime.ToString(), CurrentLevel.ResetCount.ToString(), CurrentLevel.Win.ToString()});
		
		Application.LoadLevel(LevelName);
		
		PlayerPrefs.SetInt("Last Played Level", Application.loadedLevel);
	}
	
	public static void LoadLevel(int LevelNo)
	{
		if (CurrentLevel != null && SceneToLevelNumber(LevelNo) >= 0)
			CurrentLevel.StartCoroutine("Upload", new string[] { Application.loadedLevelName, CurrentLevel.LevelTime.ToString(), CurrentLevel.ResetCount.ToString(), CurrentLevel.Win.ToString()});
		
		Application.LoadLevel(LevelNo);
		
		PlayerPrefs.SetInt("Last Played Level", LevelNo);
	}
	
	public static int LevelToSceneNumber(int LevelNo)
	{
		if (LevelNo < SceneListReader.FirstLevel || LevelNo > SceneListReader.LastLevel)
			return -1;
		
		return LevelNo + SceneListReader.FirstLevel;
	}
	
	public static int SceneToLevelNumber(int SceneNo)
	{
		if (SceneNo < SceneListReader.FirstLevel || SceneNo > SceneListReader.LastLevel)
			return -1;
		
		return SceneNo - SceneListReader.FirstLevel;
	}
	
	System.Collections.IEnumerator Upload(string[] Params)
	{
		WWWForm Form = new WWWForm();
		Form.AddField("name", Params[0]);
		Form.AddField("time", Params[1]);
		Form.AddField("resets", Params[2]);
		Form.AddField("completes", Params[3]);
		Form.AddField("hash", Md5Sum(Params[0] + Params[1] + Params[2] + Params[3] + "12345").ToLower());
		
		WWW DatabaseEntry = new WWW("http://www.playgroundheroesstudio.com/php/updateFailSuccess.php?", Form);
			
		while (DatabaseEntry.progress != 1)
			yield return new WaitForFixedUpdate();
		
		yield return DatabaseEntry;
			
		Debug.Log(DatabaseEntry.text);
	}
		
	static string Md5Sum(string input)
	{
    	System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
    	byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
    	byte[] hash = md5.ComputeHash(inputBytes);
 
    	System.Text.StringBuilder sb = new System.Text.StringBuilder();
		
    	for (int i = 0; i < hash.Length; i++)
    	    sb.Append(hash[i].ToString("X2"));
		
    	return sb.ToString();
	}
}
using UnityEngine;

public class TrampolineManager : MonoBehaviour
{
	Transform Tr = null;
	
	public float Strength = 5.0f;
	
	public int Width = 4;
	
	public Trampoline TrampolinePrefab = null;
	
	public Transform HardBlockPrefab = null;
	
	void Awake()
	{
		if (!Application.isPlaying)
			return;
		
		Tr = transform;
		
		Trampoline LocalTrampoline = (Trampoline)Instantiate(TrampolinePrefab);
		
		LocalTrampoline.Init(Tr, Width - 2);
		
		LocalTrampoline.Strength = Strength;
		
		Vector3 CubePos = new Vector3(-Width * 0.5f + 0.5f, 1.0f, 0.0f);
		
		InitHardBlock(CubePos);
		
		CubePos.y--;
		
		for (int X = 0; X < Width; X++)
		{
			InitHardBlock(CubePos);
			CubePos.x++;
		}
		
		CubePos.x--;
		
		CubePos.y++;
		
		InitHardBlock(CubePos);
	}
	
	void Update()
	{
		if (Width < 2)
			Width = 2;
	}
	
	void OnDrawGizmos()
	{
		if (!Tr)
			Tr = transform;
		
		Gizmos.matrix = Tr.localToWorldMatrix;
		
		Gizmos.color = Color.yellow;
		
		Gizmos.DrawWireCube(Vector3.up, new Vector3(Width - 2, 1.0f, 1.0f));
		
		Gizmos.color = Color.grey;
		
		Vector3 CubePos = new Vector3(-Width * 0.5f + 0.5f, 1.0f, 0.0f);
		
		Gizmos.DrawWireCube(CubePos, Vector3.one);
		
		CubePos.y--;
		
		for (int X = 0; X < Width; X++)
		{
			Gizmos.DrawWireCube(CubePos, Vector3.one);
			CubePos.x++;
		}
		
		CubePos.x--;
		
		CubePos.y++;
		
		Gizmos.DrawWireCube(CubePos, Vector3.one);
		
		Gizmos.color = Color.cyan;
		
		Gizmos.DrawRay(Vector3.zero, Vector3.up * 3);
	}
	
	void InitHardBlock(Vector3 LocalPos)
	{
		Transform LocalBlock = (Transform)Instantiate(HardBlockPrefab);
		
		LocalBlock.parent = Tr;
		
		LocalBlock.localPosition = LocalPos;
		
		LocalBlock.localRotation = Quaternion.identity;
	}
}
using UnityEngine;

[ExecuteInEditMode]
public class FanManager : MonoBehaviour
{
	Transform Tr = null;
	
	public int Width = 3;
	
	public float Height = 4.0f;
	
	public float Strength = 20.0f;
	
	public Fan FanPrefab = null;
	
	public Transform HardBlockPrefab = null;
	
	Vector3 FanPos { get { return new Vector3(0.0f, Height * 0.5f, 0.0f); } }
	
	void Awake()
	{
		if (!Application.isPlaying)
			return;
		
		Tr = transform;
		
		Fan LocalFan = (Fan)Instantiate(FanPrefab);
		
		LocalFan.Init(Tr, FanPos, Width, Height - 1.0f);
		
		LocalFan.Strength = Strength;
		
		for (int B = 0; B < Width; B++)
		{
			Transform LocalBlock = (Transform)Instantiate(HardBlockPrefab);
			
			LocalBlock.parent = Tr;
			
			LocalBlock.localPosition = new Vector3(B - Width * 0.5f + 0.5f, 0.0f, 0.0f);
			
			LocalBlock.localRotation = Quaternion.identity;
		}
	}
	
	void OnDrawGizmos()
	{
		if (!Tr)
			Tr = transform;
		
		Gizmos.matrix = Tr.localToWorldMatrix;
		
		for (int B = 0; B < Width; B++)
			Gizmos.DrawWireCube(new Vector3(B - Width * 0.5f + 0.5f, 0.0f, 0.0f), Vector3.one);
		
		Gizmos.color = Color.green;
		
		Gizmos.DrawWireCube(FanPos, new Vector3(Width, Height - 1.0f, 1.0f));
	}
}
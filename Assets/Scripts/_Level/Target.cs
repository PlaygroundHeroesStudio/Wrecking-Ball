using UnityEngine;

[RequireComponent(typeof(Light))]
public class Target : MonoBehaviour
{
	enum Region
	{
		TopLeft,
		Top,
		TopRight,
		Left,
		Centre,
		Right,
		BottomLeft,
		Bottom,
		BottomRight
	}
	
	Rigidbody Rigid = null;
	
	bool _InDebrisZone = false;
	
	public Texture OffScreenTex = null;
	
	Region OffScreenRegion = Region.Centre;
	
	Light ZoneLight = null;
	
	public bool InDebrisZone { get { return _InDebrisZone; } set { _InDebrisZone = value; ZoneLight.enabled = value; } }
	
	public bool AtDestination { get { return InDebrisZone && Rigid.IsSleeping(); } }
	
	void Awake()
	{
		Rigid = rigidbody;
		ZoneLight = GetComponent<Light>();
		ZoneLight.enabled = false;
	}
	
	void OnTriggerEnter(Collider C)
	{
		if (C.GetComponent<DebrisZone>())
			InDebrisZone = true;
	}
	
	void OnTriggerExit(Collider C)
	{
		if (C.GetComponent<DebrisZone>())
			InDebrisZone = false;
	}
	
	void OnGUI()
	{
		Vector3 ScreenPos = Camera.main.WorldToScreenPoint(Rigid.position);
		
		if (ScreenPos.x < 0.0f)
			OffScreenRegion = Region.Left;
		else if (ScreenPos.x > Screen.width)
			OffScreenRegion = Region.Right;
		else
			OffScreenRegion = Region.Centre;
		
		if (ScreenPos.y < 0.0f)
		{
			if (OffScreenRegion == Region.Left)
				OffScreenRegion = Region.BottomLeft;
			else if (OffScreenRegion == Region.Right)
				OffScreenRegion = Region.BottomRight;
			else
				OffScreenRegion = Region.Bottom;
		}
		else if (ScreenPos.y > Screen.height)
		{
			if (OffScreenRegion == Region.Left)
				OffScreenRegion = Region.TopLeft;
			else if (OffScreenRegion == Region.Right)
				OffScreenRegion = Region.TopRight;
			else
				OffScreenRegion = Region.Top;
		}
		
		if (OffScreenRegion != Region.Centre)
		{
			ScreenPos.x = Mathf.Clamp(ScreenPos.x, 0.0f, Screen.width - 128.0f);
			ScreenPos.y = Mathf.Clamp(ScreenPos.y, 128.0f, Screen.height);
			
			GUI.DrawTextureWithTexCoords(new Rect(ScreenPos.x, Screen.height - ScreenPos.y, 128, 128), OffScreenTex, GetUV());
		}
	}
	
	public Rigidbody GetRigid()
	{
		return Rigid;
	}
	
	public void Reset()
	{
		InDebrisZone = false;
	}
	
	Rect GetUV()
	{
		int Pos = (int)OffScreenRegion;
		
		return new Rect((Pos % 3) / 3.0f, 1.0f - (Pos / 3 + 1) / 3.0f, 1.0f / 3, 1.0f / 3);
	}
}
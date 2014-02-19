#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingPlatform : MonoBehaviour
{
	enum MoveDir
	{
		Forward,
		Reverse
	}
	
	[System.Serializable]
	public class Destination
	{
		public Vector3 Position = Vector3.zero;
		public Vector3 Rotation = Vector3.zero;
		public Vector3 Bend = Vector3.zero;
		public float TravelTime = 2.0f;
		public bool BendMovement = false;
	}
	
	Transform Tr = null;
	
	Rigidbody Rigid = null;
	
	public bool Loop = false;
	
	public Destination[] Dests = null;
	
	public Destination StartDest = null;
	
	int CurPos = -1;
	
	float Timer = 0.0f;
	
	MoveDir Dir = MoveDir.Forward;
	
	void Awake()
	{
		Tr = transform;
		Rigid = rigidbody;
		StartDest.Position = Tr.position;
		
		foreach (Destination D in Dests)
			if (D.TravelTime < Time.fixedDeltaTime)
				D.TravelTime = Time.fixedDeltaTime;
	}
				
	void FixedUpdate()
	{
		if (Dests == null || Dests.Length == 0)
			return;
		
		if (Dir == MoveDir.Forward)
		{
			if (CurPos < 0)
				Timer += Time.fixedDeltaTime / Dests[0].TravelTime;
			else if (CurPos >= Dests.Length - 1 && Loop)
				Timer += Time.fixedDeltaTime / StartDest.TravelTime;
			else
				Timer += Time.fixedDeltaTime / Dests[CurPos + 1].TravelTime;
		}
		else
			Timer += Time.fixedDeltaTime / Dests[CurPos].TravelTime;
		
		while (Timer >= 1.0f)
		{
			Timer -= 1.0f;
			
			CurPos += Dir == MoveDir.Forward ? 1 : -1;
			
			if (CurPos < 0 && Dir == MoveDir.Reverse)
			{
				CurPos = -1;
				Dir = MoveDir.Forward;
			}
			else if (CurPos >= Dests.Length - 1 && !Loop)
			{
				CurPos = Dests.Length - 1;
				Dir = MoveDir.Reverse;
			}
			else if (CurPos >= Dests.Length)
				CurPos = -1;
		}
		
		if (Dir == MoveDir.Forward)
		{
			if (CurPos < 0)
				Lerp(StartDest, Dests[0]);
			else if (CurPos >= Dests.Length - 1 && Loop)
				Lerp(Dests[CurPos], StartDest);
			else
				Lerp(Dests[CurPos], Dests[CurPos + 1]);
		}
		else if (CurPos == 0)
			Lerp(Dests[CurPos], StartDest);
		else
			Lerp(Dests[CurPos], Dests[CurPos - 1]);
	}
	
	void Lerp(Destination SD, Destination ED)
	{
		Destination Dest = Dir == MoveDir.Forward ? SD : ED;
		
		if (Dest.BendMovement)
			Rigid.MovePosition(Vector3.Lerp(SD.Position, ED.Position, Timer) + Dest.Bend * Mathf.Sin(Timer * Mathf.PI));
		else
			Rigid.MovePosition(Vector3.Lerp(SD.Position, ED.Position, Timer));
		
		Rigid.MoveRotation(Quaternion.Lerp(Quaternion.Euler(SD.Rotation), Quaternion.Euler(ED.Rotation), Timer));
	}
	
	public void Reset()
	{
		Dir = MoveDir.Forward;
		CurPos = -1;
		Timer = 0.0f;
	}
	
	void OnDrawGizmos()
	{
#if UNITY_EDITOR
		if (Tr == null)
			Awake();
		
		Gizmos.color = Color.magenta;
		
		Gizmos.DrawLine(Tr.position, Tr.position + Tr.rotation * Vector3.left);
		
		Gizmos.color = Color.white;
		
		if (Dests == null || Dests.Length == 0)
			return;
		
		Vector3 Mid = Vector3.zero;
		
		GameObject[] Selected = Selection.gameObjects;
		
		foreach (GameObject GO in Selected)
		{
			if (GO == gameObject)
			{
				Gizmos.DrawLine(Tr.position, Dests[0].Position);
				
				if (StartDest.BendMovement)
				{
					Gizmos.color = Color.green;
					
					Mid = (Tr.position + Dests[0].Position) * 0.5f;
					
					Gizmos.DrawLine(Mid, Mid + StartDest.Bend);
					
					Gizmos.color = Color.white;
				}
				
				Gizmos.DrawWireSphere(Tr.position, 0.5f);
				
				for (int Pos = 0; Pos < Dests.Length; Pos++)
				{
					Gizmos.color = Color.magenta;
					
					Gizmos.DrawLine(Dests[Pos].Position, Dests[Pos].Position + Quaternion.Euler(Dests[Pos].Rotation) * Vector3.left);
					
					Gizmos.color = Color.white;
					
					if (Pos + 1 < Dests.Length)
					{
						Gizmos.DrawLine(Dests[Pos].Position, Dests[Pos + 1].Position);
						
						if (Dests[Pos].BendMovement)
						{
							Gizmos.color = Color.green;
							
							Mid = (Dests[Pos].Position + Dests[Pos + 1].Position) * 0.5f;
							
							Gizmos.DrawLine(Mid, Mid + Dests[Pos].Bend);
							
							Gizmos.color = Color.white;
						}
					}
					else if (Loop)
					{
						Gizmos.DrawLine(Dests[Pos].Position, Tr.position);
						
						if (Dests[Pos].BendMovement)
						{
							Gizmos.color = Color.green;
							
							Mid = (Dests[Pos].Position + Tr.position) * 0.5f;
							
							Gizmos.DrawLine(Mid, Mid + Dests[Pos].Bend);
							
							Gizmos.color = Color.white;
						}
					}
					
					Gizmos.DrawWireSphere(Dests[Pos].Position, 0.5f);
				}
				
				break;
			}
		}
#endif
	}
}
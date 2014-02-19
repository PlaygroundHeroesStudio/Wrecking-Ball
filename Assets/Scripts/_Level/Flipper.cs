using UnityEngine;

public class Flipper : MonoBehaviour
{
	Rigidbody Rigid = null;
	
	Quaternion StartRotation = Quaternion.identity;
	
	public Vector3[] MidRotations = null;
	
	public Vector3 EndRotation = Vector3.zero;
	
	public float RotationSpeed = 360.0f;
	
	bool Flipping = false;
	
	float RotLength = 0.0f;
	
	float Timer = 0.0f;
	
	void Awake()
	{
		Rigid = rigidbody;
		
		StartRotation = Rigid.rotation;
		
		UpdateRotLength();
	}
	
	void FixedUpdate()
	{
		if (Timer <= 0.0f && !Flipping)
			return;
		
		RotateToNext();
	}
	
	void RotateToNext()
	{
		float PreTimer = Timer;
		
		Timer += RotationSpeed / RotLength * Time.fixedDeltaTime * (Flipping ? 1 : -1);
		
		if ((int)Timer != (int)PreTimer)
			UpdateRotLength();
		
		if (Timer <= 0.0f)
			Timer = 0.0f;
		else if (Timer >= MidRotations.Length + 1)
		{
			Timer = MidRotations.Length + 1;
			Flipping = false;
		}
		
		int CurRot = Mathf.Min((int)Timer, MidRotations.Length);
		
		Quaternion Q1 = CurRot == 0 ? StartRotation : Quaternion.Euler(MidRotations[CurRot - 1]);
		Quaternion Q2 = CurRot >= MidRotations.Length ? Quaternion.Euler(EndRotation) : Quaternion.Euler(MidRotations[CurRot]);
		
		Rigid.MoveRotation(Quaternion.Lerp(Q1, Q2, Timer - (int)Mathf.Min(Timer, MidRotations.Length)));
	}
	
	void UpdateRotLength()
	{
		int CurRot = Mathf.Min((int)Timer, MidRotations.Length);
		
		Quaternion Q1 = CurRot == 0 ? StartRotation : Quaternion.Euler(MidRotations[CurRot - 1]);
		Quaternion Q2 = CurRot >= MidRotations.Length ? Quaternion.Euler(EndRotation) : Quaternion.Euler(MidRotations[CurRot]);
		
		RotLength = QuatAngleMag(Q1, Q2) * Mathf.Rad2Deg;
	}
	
	Quaternion QuatDiff(Quaternion Q1, Quaternion Q2)
	{
		return Quaternion.Inverse(Q1) * Q2;
	}
	
	float QuatAngleMag(Quaternion Q1, Quaternion Q2)
	{
		return 2 * Mathf.Acos(Quaternion.Dot(Q1, Q2));
	}
	
	void OnCollisionEnter()
	{
		Flipping = true;
	}
	
#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			if (Rigid == null)
				Rigid = rigidbody;
		
			StartRotation = Rigid.rotation;
		}
		
		Vector3 StartPoint = Rigid.position + StartRotation * Vector3.left * 5;
		
		Gizmos.DrawWireSphere(StartPoint, 0.25f);
		
		for (int Rot = 0; Rot <= MidRotations.Length; Rot++)
			DrawPoints(ref StartPoint, Rot);
	}
	
	void DrawPoints(ref Vector3 StartPoint, int MidRot)
	{
		Vector3 MidPoint = StartPoint;
		
		Quaternion Q1 = MidRot == 0 ? StartRotation : Quaternion.Euler(MidRotations[MidRot - 1]);
		
		for (int I = 0; I <= 12; I++)
		{
			float Lerp = (float)I / 12;
			
			Gizmos.color = Color.Lerp(Color.red, Color.green, Lerp);
			
			Quaternion Q2 = Quaternion.Euler(MidRot >= MidRotations.Length ? EndRotation : MidRotations[MidRot]);
			
			Vector3 EndPoint = Rigid.position + Quaternion.Lerp(Q1, Q2, Lerp) * Vector3.left * 5;
			
			Gizmos.DrawLine(MidPoint, EndPoint);
			
			MidPoint = EndPoint;
		}
		
		Gizmos.color = Color.white;
		
		Gizmos.DrawWireSphere(MidPoint, 0.25f);
		
		StartPoint = MidPoint;
	}
#endif
	
	public void Reset()
	{
		Flipping = false;
		
		Rigid.rotation = StartRotation;
	}
}
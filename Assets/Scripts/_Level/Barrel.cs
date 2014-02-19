using UnityEngine;

public class Barrel : MonoBehaviour
{
	Transform Tr = null;
	
	public Rigidbody Captured = null;
	
	Vector3 StartPos = Vector3.zero;
	
	public Vector3 EndPos = Vector3.zero;
	
	public float Force = 5.0f;
	
	public float Duration = 3.0f;
	
	float Timer = 0.0f;
	
	int Dir = 1;
	
	void Awake()
	{
		Tr = transform;
		
		StartPos = Tr.position;
	}
	
	void Update()
	{
		Timer += Time.deltaTime / Duration * Dir;
		
		if (Timer <= 0.0f)
		{
			Timer = -Timer;
			Dir = 1;
		}
		else if (Timer >= 1.0f)
		{
			Timer = 2.0f - Timer;
			Dir = -1;
		}
		
		Tr.position = Vector3.Lerp(StartPos, EndPos, Timer);
	}
	
	void OnTriggerEnter(Collider C)
	{
		if (Captured != null || !C.GetComponent<Shootable>())
			return;
		
		Rigidbody Rigid = C.rigidbody;
		
		if (Rigid != null)
			Capture(Rigid);
	}
	
	void OnDrawGizmos()
	{
		if (!Application.isPlaying)
		{
			if (Tr == null)
				Tr = transform;
		
			StartPos = Tr.position;
		}
		
		Gizmos.DrawWireSphere(EndPos, 0.25f);
		
		Gizmos.color = Color.yellow;
		
		Gizmos.DrawLine(StartPos, EndPos);
		
		Gizmos.color = Color.green;
		
		Gizmos.DrawRay(StartPos, Tr.up * 3);
	}
	
	public void Reset()
	{
		Timer = 0.0f;
		Tr.position = StartPos;
		Dir = 1;
		
		if (Captured == null)
			return;
		
		Captured.isKinematic = false;
		Captured.transform.parent = Level.CurrentLevel.transform;
		Captured = null;
	}
	
	public void Activate()
	{
		if (Captured == null)
			return;
		
		Captured.isKinematic = false;
		Captured.velocity = Tr.up * Force;
		Captured.transform.parent = Level.CurrentLevel.transform;
		Captured = null;
	}
	
	public void Capture(Rigidbody Rigid)
	{
		Captured = Rigid;
		Captured.velocity = Vector3.zero;
		Captured.isKinematic = true;
		
		Transform CTr = Captured.transform;
		
		CTr.parent = Tr;
		CTr.localPosition = Vector3.zero;
		CTr.localRotation = Quaternion.identity;
	}
}
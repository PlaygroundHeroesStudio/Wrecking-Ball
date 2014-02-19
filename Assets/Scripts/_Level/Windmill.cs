using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Windmill : MonoBehaviour
{
	Transform Tr = null;
	
	Rigidbody Rigid = null;
	
	public float RotateSpeed = 5.0f;
	
	Vector3 StartRot = Vector3.zero;
	
	void Awake()
	{
		Tr = transform;
		
		Rigid = rigidbody;
		
		StartRot = Tr.eulerAngles;
	}
	
	void Update()
	{
		Rigid.angularVelocity = Vector3.back * RotateSpeed;
	}
	
	public void Reset()
	{
		Tr.eulerAngles = StartRot;
	}
}
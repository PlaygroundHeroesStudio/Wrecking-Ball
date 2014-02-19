using UnityEngine;

public class Trampoline : MonoBehaviour
{
	Transform Tr = null;
	
	public float Strength = 5.0f;
	
	void Awake()
	{
		Tr = transform;
	}
	
	void OnTriggerEnter(Collider C)
	{
		Rigidbody Rigid = C.rigidbody;
		
		if (Rigid)
		{
			Vector3 Vel = Tr.InverseTransformDirection(Rigid.velocity);
			
			Vel.y = Strength;
			
			Rigid.velocity = Tr.TransformDirection(Vel);
		}
	}
				
	public void Init(Transform Parent, int Width)
	{
		Tr.parent = Parent;
		
		Tr.localPosition = Vector3.up;
		
		Tr.localScale = new Vector3(Width, 1.0f, 1.0f);
		
		Tr.localRotation = Quaternion.identity;
	}
}
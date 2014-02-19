using UnityEngine;

public class Repel : MonoBehaviour
{
	Transform Tr = null;
	
	public float Strength = 3.0f;
	
	void Awake()
	{
		Tr = transform;
	}
	
	void OnTriggerStay(Collider C)
	{
		Rigidbody Rigid = C.rigidbody;
		
		if (Rigid)
			Rigid.velocity -= (Tr.position - Rigid.position).normalized * Strength * Time.fixedDeltaTime;
	}
}
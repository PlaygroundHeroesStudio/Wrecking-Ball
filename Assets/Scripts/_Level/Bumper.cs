using UnityEngine;

public class Bumper : MonoBehaviour
{
	public float Force = 10.0f;
	
	void OnCollisionEnter(Collision C)
	{
		Rigidbody OtherRigid = C.rigidbody;
		
		if (OtherRigid == null)
			return;
		
		OtherRigid.velocity = Vector3.Reflect(C.relativeVelocity.normalized, C.contacts[0].normal) * Force;
	}
}
using UnityEngine;

public class Fan : MonoBehaviour
{
	Transform Tr = null;
	
	public float Strength = 20.0f;
	
	public ParticleSystem Particles = null;
	
	void Awake()
	{
		Tr = transform;
	}
	
	void OnTriggerStay(Collider C)
	{
		Rigidbody Rigid = C.rigidbody;
		
		if (Rigid)
			Rigid.velocity += Tr.up * Strength * Time.fixedDeltaTime;
	}
				
	public void Init(Transform Parent, Vector3 LocalPosition, int Width, float Height)
	{
		Tr.parent = Parent;
		
		Tr.localPosition = LocalPosition;
		
		Tr.localScale = new Vector3(Width, Height, 1.0f);
		
		Tr.localRotation = Quaternion.identity;
		
		Particles.startSpeed = Height * 0.5f + 1;
		
		((ParticleSystemRenderer)Particles.renderer).lengthScale = Height * 0.5f + 1;
	}
}
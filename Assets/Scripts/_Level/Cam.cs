using UnityEngine;

public class Cam : MonoBehaviour
{
	protected Transform Tr = null;
	
	protected virtual void Awake()
	{
		Tr = transform;
	}
	
	protected virtual void Update()
	{
	
	}
}
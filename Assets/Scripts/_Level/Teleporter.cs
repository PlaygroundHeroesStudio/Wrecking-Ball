using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
	public enum ExitStyle
	{
		Relative,	// will transform velocity to be relative to the exit transform
		Fixed,		// will exit in the direction of the exit transform's up
		Unchanged	// velocity will be unchanged
	}
	
	Transform Tr = null;
	
	public Transform Connection = null;
	
	public ExitStyle Style = ExitStyle.Fixed;
	
	public Transform LinkPrefab = null;
	
	Teleporter ConTel = null;
	
	Transform Link = null;
	
	List<Collider> Teleporting = new List<Collider>();
	
	void Awake()
	{
		Tr = transform;
		
		if (Connection == null)
			return;
		
		ConTel = Connection.GetComponent<Teleporter>();
		
		if (ConTel.Connection == Tr && Link != null)
			return;
		
		Transform LTr = (Transform)Instantiate(LinkPrefab, (Tr.position + Connection.position) * 0.5f + Vector3.forward,
											   Quaternion.LookRotation(Connection.position - Tr.position, Vector3.up));
		
		ConTel.Link = LTr;
		
		Vector3 Scale = LTr.localScale;
		
		Scale.z = (Connection.position - Tr.position).magnitude;
		
		LTr.localScale = Scale;
		
		LTr.parent = Level.CurrentLevel.transform;
	}
	
	void OnTriggerEnter(Collider C)
	{
		if (Connection == null || !C.GetComponent<Teleportable>())
			return;
		
		if (Teleporting.Contains(C))
		{
			Teleporting.Remove(C);
			return;
		}
		
		if (ConTel != null)
			ConTel.Teleporting.Add(C);
		
		Transform CTr = C.transform;
		
		Transform Parent = CTr.parent;
		
		CTr.position = Connection.position;
		
		Rigidbody Rigid = C.rigidbody;
		
		if (Rigid != null)
		{
			switch (Style)
			{
				case ExitStyle.Relative:
					Rigid.velocity = Connection.TransformDirection(Rigid.velocity);
					
					Rigid.angularVelocity = Connection.TransformDirection(Rigid.angularVelocity);
					break;
				case ExitStyle.Fixed:
					Rigid.velocity = Connection.up * Rigid.velocity.magnitude;
					break;
				default:
					break;
			}
		}
		
		Vector3 Rot = CTr.localEulerAngles;
		
		CTr.parent = Connection;
		
		CTr.localEulerAngles = Rot;
		
		CTr.parent = Parent;
	}
	
	void OnDrawGizmos()
	{
		if (Tr == null)
			Tr = transform;
		
		Gizmos.DrawRay(Tr.position, Tr.up * 3);
	}
}
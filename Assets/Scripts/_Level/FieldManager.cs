using UnityEngine;

public class FieldManager : MonoBehaviour
{
	Transform Tr = null;
	
	public Transform FociTop = null;
	public Transform FociBottom = null;
	
	void Awake()
	{
		Tr = transform;
		
		Quaternion Rot = Tr.rotation;
		
		Tr.rotation = Quaternion.identity;
		
		Transform FociInst = (Transform)Instantiate(FociTop);
		
		FociInst.parent = Tr;
		
		FociInst.localPosition = new Vector3(0.0f, 0.5f, 0.0f);
		
		FociInst = (Transform)Instantiate(FociBottom);
		
		FociInst.parent = Tr;
		
		FociInst.localPosition = new Vector3(0.0f, -0.5f, 0.0f);
		
		Tr.rotation = Rot;
	}
}
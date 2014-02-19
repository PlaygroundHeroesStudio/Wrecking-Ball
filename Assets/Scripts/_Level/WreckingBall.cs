using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WreckingBall : MonoBehaviour
{
	Rigidbody Rigid = null;
	
	public float Gravity = 9.8f;
	
	void Awake()
	{
		Rigid = rigidbody;
	}
	
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			Level.CurrentLevel.ResetLevel();
	}
#endif
	
	void FixedUpdate()
	{
#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		if (Input.touchCount > 0)
		{
			Touch T = Input.touches[0];
			
			Vector3 Grav = new Vector3(T.position.x - Screen.width * 0.5f, T.position.y - Screen.height * 0.5f, 0.0f).normalized;
			
			Rigid.velocity += Grav * Gravity * Time.fixedDeltaTime;
		}
		else
			Rigid.velocity += Vector3.down * Gravity * Time.fixedDeltaTime;
#else
		Vector3 Grav = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);
		
		if (Grav.sqrMagnitude > 0.0f)
			Grav.Normalize();
		else
			Grav.y = -1.0f;
		
		Rigid.velocity += Grav * Gravity * Time.fixedDeltaTime;
#endif
	}
}
using UnityEngine;
using System.Collections;

public class FloatingTextTeleportScript : MonoBehaviour {

	float m_fFloatSpeed = 0.5f;
	Vector3 m_up = Vector2.up;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 pos = transform.position;
		//pos.y += m_fFloatSpeed * Time.deltaTime;
		pos += m_up * m_fFloatSpeed * Time.deltaTime;


		if(pos.y >= 20)
			pos.y = -5;
		transform.position = pos;


	}
}

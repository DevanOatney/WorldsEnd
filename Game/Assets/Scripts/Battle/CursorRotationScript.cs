using UnityEngine;
using System.Collections;

public class CursorRotationScript : MonoBehaviour {

	float m_fRotationSpeed = 60.0f;
	float m_fRotationAmount = 5.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(new Vector2(m_fRotationAmount * m_fRotationSpeed * Time.deltaTime, 0.0f));
	}
}

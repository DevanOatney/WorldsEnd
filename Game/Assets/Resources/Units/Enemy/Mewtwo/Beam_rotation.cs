using UnityEngine;
using System.Collections;

public class Beam_rotation : MonoBehaviour {
	public float rotation_speed = 60.0f;
	public float rotation_amount = 5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (rotation_amount * rotation_speed * Time.deltaTime, 0.0f, 0.0f));
	
	}
}

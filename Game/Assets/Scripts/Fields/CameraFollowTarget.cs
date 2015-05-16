using UnityEngine;
using System.Collections;

public class CameraFollowTarget : MonoBehaviour {

	public GameObject m_goTarget;
	float dampTime = 0.25f;
	private Vector3 velocity = Vector3.zero;
	// Use this for initialization
	void Start () 
	{
		GetComponent<AudioSource>().volume = GameObject.Find("PersistantData").GetComponent<DCScript>().m_fMusicVolume;
		Vector3 pos = m_goTarget.transform.position;
		pos.z = transform.position.z;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_goTarget != null)
		{

			//Vector3 pos = new Vector3(m_goTarget.transform.position.x, m_goTarget.transform.position.y, transform.position.z);
			//pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
			//pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
			//transform.position = pos;




			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(m_goTarget.transform.position);
			Vector3 delta = m_goTarget.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}
	}
}
using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

public class CameraFollowTarget : MonoBehaviour {

	public GameObject m_goTarget;
	public GameObject m_goNextTarget = null;
	float dampTime = 0.25f;
	private Vector3 velocity = Vector3.zero;

	public bool m_bShouldSwirl = false;
	// Use this for initialization
	void Start () 
	{
		GetComponent<AudioSource>().volume = GameObject.Find("PersistantData").GetComponent<DCScript>().m_fMusicVolume;
		if(m_goTarget == null)
			m_goTarget = GameObject.Find("Player");
		Vector3 pos = m_goTarget.transform.position;
		pos.z = transform.position.z;
		transform.position = pos;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bShouldSwirl == true)
		{
			Vector2 rad = GetComponent<Vortex>().radius;
			GetComponent<Vortex>().radius = new Vector2(rad.x + 1*Time.deltaTime, rad.y + 1 * Time.deltaTime);
		}
		if(m_goTarget != null)
		{

			//Vector3 pos = new Vector3(m_goTarget.transform.position.x, m_goTarget.transform.position.y, transform.position.z);
			//pos.x = Mathf.Clamp(pos.x, leftBound, rightBound);
			//pos.y = Mathf.Clamp(pos.y, bottomBound, topBound);
			//transform.position = pos;
			Vector3 point = GetComponent<Camera>().WorldToViewportPoint(m_goTarget.transform.position);

			Vector3 delta = m_goTarget.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
			if(delta.magnitude > 2.0f)
			{
				Debug.Log("hit");
				transform.position = transform.position + delta;
			}
			else
			{
				if(delta.magnitude < 0.05f)
				{
					if(m_goNextTarget != null)
					{
						m_goTarget = m_goNextTarget;
						m_goNextTarget = null;
						point = GetComponent<Camera>().WorldToViewportPoint(m_goTarget.transform.position);
						delta = m_goTarget.transform.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); 
					}
				}
				else
				{
					Vector3 destination = transform.position + delta;
					transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
				}
			}
		}
	}
}
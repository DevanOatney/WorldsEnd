using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLocationScript : MonoBehaviour 
{
	public List<GameObject> m_lWaypoints;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnDrawGizmos()
	{
		foreach (GameObject _wypnt in m_lWaypoints)
		{
			if (_wypnt.tag == "Harvest Point")
				Gizmos.color = Color.red;
			Gizmos.DrawSphere (_wypnt.transform.position, 0.2f);	

		}
	}
}

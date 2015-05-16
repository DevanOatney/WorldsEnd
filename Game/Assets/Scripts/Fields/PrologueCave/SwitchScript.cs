using UnityEngine;
using System.Collections;

public class SwitchScript : MonoBehaviour 
{
	public bool m_bHasBeenInterractedWith = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c)
	{
		if(c.name == "Action Box(Clone)")
		{
			if(m_bHasBeenInterractedWith == false)
			{
				m_bHasBeenInterractedWith = true;
				GameObject.Find("Event System").GetComponent<PrologueCaveEventScript>().ReduceCounter();
			}
		}
	}
}

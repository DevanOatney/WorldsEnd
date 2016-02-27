using UnityEngine;
using System.Collections;

public class TopTabMouseScript : MonoBehaviour 
{
	public int m_nIndex;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseEnter()
	{
		Debug.Log("hit");
		transform.parent.parent.GetComponent<CBattleActionsScript>().ChangeIndex(m_nIndex);
	}
}

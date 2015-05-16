using UnityEngine;
using System.Collections;

public class BackgroundScript : MonoBehaviour 
{
	public Sprite[] m_sBackgrounds;
	void Awake()
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = m_sBackgrounds[0];
	}
	// Use this for initialization
	void Start () 
	{
		SetBackground(GameObject.Find("PersistantData").GetComponent<DCScript>().GetBattleFieldBackgroundIter());
	}
	
	// Update is called once per frame
	void Update () 
	{
	}

	public void SetBackground(int i)
	{
		gameObject.GetComponent<SpriteRenderer>().sprite = m_sBackgrounds[i];
	}
}

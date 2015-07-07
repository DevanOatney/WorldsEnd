using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OILScript : MonoBehaviour 
{
	float m_fBufferedTimer = 0.0f;
	float m_fBucket = 0.5f;
	string[] m_szTagsToOrganize = new string[] {"Player", "Treasure", "Merchant", "Ally", "Enemy", "Interractable", "InnKeeper"};

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		m_fBufferedTimer += Time.deltaTime;
		if(m_fBufferedTimer >= m_fBucket)
		{
			List<GameObject> lMovingObjs = new List<GameObject>();
			GameObject[] goMO;
			foreach(string s in m_szTagsToOrganize)
			{
				goMO = GameObject.FindGameObjectsWithTag(s);
				if(goMO.Length > 0)
					lMovingObjs.AddRange(goMO);
			}
			lMovingObjs.Sort(delegate(GameObject x, GameObject y) {
				return x.transform.position.y.CompareTo(y.transform.position.y);});

			int c = 49;
			foreach(GameObject g in lMovingObjs)
			{
				if(g.GetComponent<SpriteRenderer>() != null)
				{
					g.GetComponent<SpriteRenderer>().sortingOrder = c;
					c--;
				}
			}
			m_fBufferedTimer = 0.0f;
		}
	}
}

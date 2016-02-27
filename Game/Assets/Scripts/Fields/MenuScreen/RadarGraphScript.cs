using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI.Extensions;


public class RadarGraphScript : MonoBehaviour 
{
	public GameObject m_goOuterOutline;
	public GameObject m_goInnerOutline;
	public GameObject m_goInnerFill;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_goInnerFill.GetComponent<UIPolygon>().color == Color.yellow)
			m_goInnerFill.GetComponent<UIPolygon>().color = Color.red;
	}

	public void AdjustFill(List<float> lPoints)
	{
		float[] distances = m_goInnerFill.GetComponent<UIPolygon>().VerticesDistances;
		for(int i = 0; i < distances.Length-1;++i)
		{
			distances[i] = lPoints[i];
		}
		m_goInnerFill.GetComponent<UIPolygon>().VerticesDistances = distances;
		m_goInnerFill.GetComponent<UIPolygon>().color = Color.yellow;
	}

}

using UnityEngine;
using System.Collections;

public class SlowTextScript : MonoBehaviour 
{
	Vector3 pos;
	public GameObject textSpeed;
	public GUIStyle textStyle;
	private int m_nTextInteger = 1;
	// Use this for initialization
	void Start () 
	{ 
		pos = transform.position;
		pos = Camera.main.WorldToScreenPoint(pos);
		pos.y = Screen.height - pos.y - 15;
		textStyle.fontSize = 18;
		textStyle.normal.textColor = Color.white;
	}
	
	// Update is called once per frame
	void Update () 
	{
	}
	void OnGUI()
	{
		if(textSpeed.GetComponent<TextSpeedScript>().m_nSelectedIndex == m_nTextInteger)
			textStyle.normal.textColor = Color.black;
		else
			textStyle.normal.textColor = Color.white;

			//slow
		if(GUI.Button(new Rect(pos.x, pos.y, 70, 20), "Slow", textStyle))
			textSpeed.GetComponent<TextSpeedScript>().ChangeIndex(m_nTextInteger);

        
	}
	void OnMouseDown()
	{
		textSpeed.GetComponent<TextSpeedScript>().ChangeIndex(m_nTextInteger);
	}
}

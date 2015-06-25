using UnityEngine;
using System.Collections;

public class MenuSelectScript : MonoBehaviour 
{
	public int m_nIndex = 3;

	// Use this for initialization
	void Start () 
	{
		//Apparently textmeshes are blurrier now.. using this hack to make my text less blurry in ratio to an orthographic camera
		float pixelRatio = (Camera.main.orthographicSize * 2.0f) / Camera.main.pixelHeight;
		gameObject.GetComponent<TextMesh>().transform.localScale = new Vector3(pixelRatio * 10.0f, pixelRatio * 10.0f, pixelRatio * 0.1f);
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnMouseDown()
	{
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().MouseSelection(m_nIndex);
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().SetHighlightedIndex(m_nIndex);
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().ChangeHighlightedPosition();
	}

	void Enter()
	{
		switch(m_nIndex)
		{
		case 0:
		{
			//New Game
			GameObject.Find("PersistantData").GetComponent<DCScript>().GetParty().Clear();
			GameObject Callan = Resources.Load<GameObject>("Units/Ally/Callan/Callan");
			Callan.GetComponent<PlayerBattleScript>().SetUnitStats();


			GameObject Devan = Resources.Load<GameObject>("Units/Ally/Devan/Devan");
			Devan.GetComponent<PlayerBattleScript>().SetUnitStats();

			Application.LoadLevel("Inon_Scene");
		}
			break;
		case 1:
		{
			//Continue
			Application.LoadLevel("IntroContinue_Scene");
		}
			break;
		case 2:
		{
			//Settings
			Application.LoadLevel("IntroSettings_Scene");
		}
			break;
		case 3:
		{
			//Exit
			Application.Quit();
		}
			break;
		}
	}
}

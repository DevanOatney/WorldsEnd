using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIAlertWindowScript : MonoBehaviour 
{
	public enum MESSAGEID {eITEMREWARD, eJOINPARTY, eRECRUITED}
	[SerializeField]
	GameObject m_goItemRewardWindow = null;
	[SerializeField]
	GameObject m_goRecruitmentWindow = null;

	private GameObject m_goListeningObject = null;
	private bool m_bWindowActivated = false;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bWindowActivated == true)
		{
			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				m_bWindowActivated = false;
				GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
				m_goItemRewardWindow.SetActive(false);
				m_goRecruitmentWindow.SetActive(false);
				if(m_goListeningObject != null)
					m_goListeningObject.SendMessage("MessageWindowDeactivated");
			}
		}
	}

	//the listener can be null, in which case all that happens is that the message pops up, the player can't move until they press enter, and when they do the message dissappears.
	public void ActivateWindow(MESSAGEID _nMessageID, string _szNameOfThing, GameObject _goListener)
	{
		GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().BindInput();
		if(_nMessageID == MESSAGEID.eJOINPARTY)
		{
			m_goRecruitmentWindow.SetActive(true);
			m_goRecruitmentWindow.GetComponentInChildren<Text>().text = _szNameOfThing + " has joined the party!";
			m_bWindowActivated = true;
		}
		else if(_nMessageID == MESSAGEID.eRECRUITED)
		{
			m_goRecruitmentWindow.SetActive(true);
			m_goRecruitmentWindow.GetComponentInChildren<Text>().text = _szNameOfThing + " has been recruited!";
			m_bWindowActivated = true;
		}
		else if(_nMessageID == MESSAGEID.eITEMREWARD)
		{
			m_goItemRewardWindow.SetActive(true);
			m_goItemRewardWindow.GetComponentInChildren<Text>().text = "Obtained " + _szNameOfThing + ".";
			m_bWindowActivated = true;
		}
		m_goListeningObject = _goListener;
	}

}

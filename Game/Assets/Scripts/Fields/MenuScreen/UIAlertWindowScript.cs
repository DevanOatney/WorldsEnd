using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
	public class cKVP
	{
		public cKVP(MESSAGEID _ID, string _msg) {_eID = _ID; _szMessage = _msg;}
		public MESSAGEID _eID;
		public string _szMessage;
	}
	Queue<cKVP> m_qMessages = new Queue<cKVP>();
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
				if(m_qMessages.Count <= 0)
				{
					m_bWindowActivated = false;
					GameObject.Find("Player").GetComponent<FieldPlayerMovementScript>().ReleaseBind();
					m_goItemRewardWindow.SetActive(false);
					m_goRecruitmentWindow.SetActive(false);
					if(m_goListeningObject != null)
						m_goListeningObject.SendMessage("MessageWindowDeactivated", SendMessageOptions.RequireReceiver);
					Input.ResetInputAxes();
				}
				else
				{
					SetUpMessageWindow(m_qMessages.Dequeue());
				}
			}
		}
	}

	//the listener can be null, in which case all that happens is that the message pops up, the player can't move until they press enter, and when they do the message dissappears.
	public void ActivateWindow(MESSAGEID _nMessageID, string _szNameOfThing, GameObject _goListener)
	{
		if(m_qMessages.Count > 0)
		{
			m_qMessages.Enqueue(new cKVP(_nMessageID, _szNameOfThing));
		}
		else
		{
			GameObject _player = GameObject.Find("Player");
			if(_player.GetComponent<FieldPlayerMovementScript>().GetAllowInput() == false)
				_player.GetComponent<FieldPlayerMovementScript>().BindInput();
			SetUpMessageWindow(_nMessageID, _szNameOfThing);
		}


		m_goListeningObject = _goListener;
	}

	void SetUpMessageWindow(cKVP _msg)
	{
		SetUpMessageWindow(_msg._eID, _msg._szMessage);
	}
	
	void SetUpMessageWindow(MESSAGEID _eID, string _szMessage)
	{
		if(_eID == MESSAGEID.eJOINPARTY)
		{
			m_goRecruitmentWindow.SetActive(true);
			m_goRecruitmentWindow.GetComponentInChildren<Text>().text = _szMessage + " has joined the party!";
			m_bWindowActivated = true;
		}
		else if(_eID == MESSAGEID.eRECRUITED)
		{
			m_goRecruitmentWindow.SetActive(true);
			m_goRecruitmentWindow.GetComponentInChildren<Text>().text = _szMessage + " has been recruited!";
			m_bWindowActivated = true;
		}
		else if(_eID == MESSAGEID.eITEMREWARD)
		{
			m_goItemRewardWindow.SetActive(true);
			m_goItemRewardWindow.GetComponentInChildren<Text>().text = "Obtained " + _szMessage + ".";
			m_bWindowActivated = true;
		}
	}
}

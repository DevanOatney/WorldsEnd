using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueuedWarBattleMessagesScript : MonoBehaviour 
{
	GameObject m_goLeftBox;
	GameObject m_goRightBox;
	FightSceneControllerScript m_fscScript;
	bool m_bDisplayDialogue;
	Queue<sMessage> m_qDialogue = new Queue<sMessage>();
	sMessage m_mCurrentMessage = null;
	bool m_bThisIsAMeleeFight = false;
	class sMessage
	{
		public string s_szMessage;
		public bool s_bIsRightSide;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (m_bDisplayDialogue == true)
		{
			if (m_mCurrentMessage == null)
			{
				DisplayMessage ();
			}
			if (Input.GetKeyUp (KeyCode.Return))
			{
				m_goLeftBox.SetActive (false);
				m_goRightBox.SetActive (false);
				if (m_qDialogue.Count <= 0)
				{
					m_bDisplayDialogue = false;
					m_mCurrentMessage = null;
					//re-activate the fight scene.
					m_fscScript.m_bPauseFightScene = false;
					if (m_bThisIsAMeleeFight == true)
					{
						m_bThisIsAMeleeFight = false;
						m_fscScript.MeleeUpdate ();
					}
					return;
				}
				DisplayMessage ();

			}
		}
	}

	void DisplayMessage()
	{
		m_mCurrentMessage = m_qDialogue.Dequeue ();
		if (m_mCurrentMessage.s_bIsRightSide == true)
		{

			m_goRightBox.SetActive (true);
			m_goRightBox.GetComponentInChildren<Text> ().text = m_mCurrentMessage.s_szMessage;
		}
		else
		{
			m_goLeftBox.SetActive (true);
			m_goLeftBox.GetComponentInChildren<Text> ().text = m_mCurrentMessage.s_szMessage;
		}
		if (m_qDialogue.Count > 0)
		{
			if (m_qDialogue.Peek ().s_bIsRightSide != m_mCurrentMessage.s_bIsRightSide)
			{
				//This message is for the other side, feel free to display it too!
				m_mCurrentMessage = m_qDialogue.Dequeue ();
				if (m_mCurrentMessage.s_bIsRightSide == true)
				{

					m_goRightBox.SetActive (true);
					m_goRightBox.GetComponentInChildren<Text> ().text = m_mCurrentMessage.s_szMessage;
				}
				else
				{
					m_goLeftBox.SetActive (true);
					m_goLeftBox.GetComponentInChildren<Text> ().text = m_mCurrentMessage.s_szMessage;
				}
			}
		}
	}

	public void Initialize(GameObject _leftBox, GameObject _rightBox, FightSceneControllerScript _script)
	{
		m_goLeftBox = _leftBox;
		m_goRightBox = _rightBox;
		m_fscScript = _script;

	}

	public void AddToQueue(bool _isRightSide, string _szMessage, bool _meleeFight = false)
	{
		m_bThisIsAMeleeFight = _meleeFight;
		m_fscScript.m_bPauseFightScene = true;
		sMessage _newMessage = new sMessage ();
		_newMessage.s_bIsRightSide = _isRightSide;
		_newMessage.s_szMessage = _szMessage;
		m_qDialogue.Enqueue (_newMessage);
		m_bDisplayDialogue = true;
	}
}

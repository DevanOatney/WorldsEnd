using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageWindowScript : MonoBehaviour 
{
	GameObject m_goTextWindow;
	string m_szCompleteMessage = "";
	int m_nLetterIter = 0;
	bool m_bReadMessage = false;
	float m_fTextTimer = 0.0f;
	float m_fTextBucket = 0.05f;
	bool m_bTimeToClose = false;
	// Use this for initialization
	void Start () 
	{
		m_goTextWindow = GameObject.Find("TextOnWindow");
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(m_bReadMessage == true)
		{
			if(m_bTimeToClose == true)
			{
				if(Input.anyKeyDown)
				{
					GameObject.Find("TurnWatcher").SendMessage("DoneReadingMessage");
					m_bReadMessage = false;
					Input.ResetInputAxes();
				}

			}
			else if(m_fTextTimer >= m_fTextBucket)
			{
				m_nLetterIter++;
				if(m_nLetterIter > m_szCompleteMessage.Length)
				{

					m_bTimeToClose = true;
				}
				else
				{
					string newLine = "";
					for(int i = 0; i < m_nLetterIter; ++i)
						newLine += m_szCompleteMessage[i];
					m_goTextWindow.GetComponent<Text>().text = newLine;
					m_fTextTimer = 0.0f;
				}
			}
			else
				m_fTextTimer += Time.deltaTime;

			if(Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
			{
				m_nLetterIter = m_szCompleteMessage.Length;
				m_goTextWindow.GetComponent<Text>().text = m_szCompleteMessage;
			}
		}
	}



	public void BeginMessage(string szMessage)
	{
		m_szCompleteMessage = szMessage;
		m_nLetterIter = 0;
		m_bReadMessage = true;

	}
}

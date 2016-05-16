using UnityEngine;
using System.Collections;

public class SystemWindowScript : MonoBehaviour
{
    public GameObject[] m_goChoices;
    public GameObject m_goHighlighter;
    public GameObject m_goWatcher;
    int m_nChoiceIter = 0;
    bool m_bIsActive = false;
	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (m_bIsActive == true)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_nChoiceIter += 1;
                if (m_nChoiceIter >= m_goChoices.Length)
                    m_nChoiceIter = 0;
                m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_nChoiceIter -= 1;
                if (m_nChoiceIter < 0)
                    m_nChoiceIter = m_goChoices.Length - 1;
                m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                switch (m_nChoiceIter)
                {
                    case 0:
                        {
                            //End Turn
                            m_bIsActive = false;
                            gameObject.SetActive(false);
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().EndFactionTurn();
                        }
                        break;
                    case 1:
                        {
                            //Back
                            m_bIsActive = false;
                            gameObject.SetActive(false);
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().ActionCancelled();
                        }
                        break;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_bIsActive = false;
                gameObject.SetActive(false);
                m_goWatcher.GetComponent<WarBattleWatcherScript>().ActionCancelled();
            }
           
        }
	}

    public void ActivateWindow()
    {
        m_bIsActive = true;
        gameObject.SetActive(true);
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
    }
}

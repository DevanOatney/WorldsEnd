using UnityEngine;
using System.Collections;

public class ActionWindowScript : MonoBehaviour
{
    [HideInInspector]
    public int m_nChoiceIter = 0;
    public GameObject[] m_goChoices;
    public GameObject m_goHighlighter;
    public GameObject m_goWatcher;
    bool m_bIsActive = false;
    GameObject m_goUnitData;

    // Use this for initialization
    void Start ()
    {
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
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
                            //Attack
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().ShowHighlightedSquares(m_goUnitData, m_goUnitData.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nAttackRange, Color.red);
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().AttackChoiceSelected();
                            gameObject.SetActive(false);
                        }
                        break;
                    case 1:
                        {
                            //Magic
                        }
                        break;
                    case 2:
                        {
                            //Wait
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().EndMyTurn(m_goUnitData);
                            gameObject.SetActive(false);
                        }
                        break;
                }

            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_goWatcher.GetComponent<WarBattleWatcherScript>().ActionCancelled();
            }
        }
	}

    public void ActivateWindow(GameObject _cWarUnit)
    {
        m_goUnitData = _cWarUnit;
        m_bIsActive = true;
        gameObject.SetActive(true);
    }
}

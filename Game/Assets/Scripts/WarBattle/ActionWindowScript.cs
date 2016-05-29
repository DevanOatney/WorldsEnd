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
    //This is only set to false when something other than the player is using this window.
    bool m_bAllowInput = true;
    GameObject m_goUnitData;

    // Use this for initialization
    void Start ()
    {
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
	}


    public void MoveDown()
    {
        m_nChoiceIter += 1;
        if (m_nChoiceIter >= m_goChoices.Length)
            m_nChoiceIter = 0;
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
    }
    public void MoveUp()
    {
        m_nChoiceIter -= 1;
        if (m_nChoiceIter < 0)
            m_nChoiceIter = m_goChoices.Length - 1;
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
    }
    public void Confirm()
    {
        switch (m_nChoiceIter)
        {
            case 0:
                {
                    //Attack
                    m_goWatcher.GetComponent<WarBattleWatcherScript>().ShowHighlightedSquares(m_goUnitData, m_goUnitData.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nAttackRange, Color.red, true);
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
                    m_goWatcher.GetComponent<WarBattleWatcherScript>().EndUnitTurn(m_goUnitData);
                    gameObject.SetActive(false);
                }
                break;
        }
        m_nChoiceIter = 0;
    }

	// Update is called once per frame
	void Update ()
    {
        if (m_bIsActive == true && m_bAllowInput == true)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveDown();
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveUp();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                Confirm();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                m_goWatcher.GetComponent<WarBattleWatcherScript>().ActionCancelled();
            }
        }
	}

    public void ActivateWindow(GameObject _cWarUnit)
    {
        if (m_goWatcher.GetComponent<WarBattleWatcherScript>().m_bIsAllyTurn == false)
            m_bAllowInput = false;
        else
            m_bAllowInput = true;
        m_goUnitData = _cWarUnit;
        m_bIsActive = true;
        m_goHighlighter.GetComponent<RectTransform>().localPosition = m_goChoices[m_nChoiceIter].GetComponent<RectTransform>().localPosition;
        gameObject.SetActive(true);
    }
}

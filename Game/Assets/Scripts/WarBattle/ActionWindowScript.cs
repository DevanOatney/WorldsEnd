using UnityEngine;
using System.Collections;

public class ActionWindowScript : MonoBehaviour
{
    [HideInInspector]
    public int m_nChoiceIter = 0;
    public GameObject[] m_goChoices;
    public GameObject m_goHighlighter;
    bool m_bIsActive = false;
    FightSceneControllerScript.cWarUnit m_wuUnitData;

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
        }
	}

    public void ActivateWindow(FightSceneControllerScript.cWarUnit _cWarUnit)
    {
        m_bIsActive = true;
        gameObject.SetActive(true);
    }
}

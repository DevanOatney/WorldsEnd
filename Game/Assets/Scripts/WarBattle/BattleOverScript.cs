using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleOverScript : MonoBehaviour
{
    public GameObject m_goSliderBoundary;

    float m_fMovementSpeed = 60.0f;
    bool m_bStopSliding = false;
	// Use this for initialization
	void Start () {
	
	}

    public void ActivateEndWindow(string _szMessage)
    {
        GetComponentInChildren<Text>().text = _szMessage;
        gameObject.SetActive(true);
    }

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_bStopSliding == false)
            {
                m_bStopSliding = true;
                GetComponent<RectTransform>().localPosition = m_goSliderBoundary.GetComponent<RectTransform>().localPosition;
            }
            else
            {
                //TODO:End the scene.
            }
        }
        if (m_bStopSliding == false)
        {
            if (GetComponent<RectTransform>().localPosition.y < m_goSliderBoundary.GetComponent<RectTransform>().localPosition.y)
            {
                Vector3 _newPosition = GetComponent<RectTransform>().localPosition;
                _newPosition.y += m_fMovementSpeed * Time.deltaTime;
                GetComponent<RectTransform>().localPosition = _newPosition;
            }
            else
            {
                m_bStopSliding = true;
            }
        }
	}
}

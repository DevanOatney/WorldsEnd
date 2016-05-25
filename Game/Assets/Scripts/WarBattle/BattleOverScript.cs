using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleOverScript : MonoBehaviour
{
    public GameObject m_goSliderBoundary;

    float m_fMovementSpeed = 60.0f;
    bool m_bStopSliding = false;
    bool m_bBattleWon = false;
    DCScript ds;

    void Awake()
    {
        
    }

	// Use this for initialization
	void Start ()
    {
        GameObject pdata = GameObject.Find("PersistantData");
        if (pdata == null)
        {
            pdata = Instantiate(Resources.Load("Misc/PersistantData", typeof(GameObject))) as GameObject;
            pdata.name = pdata.name.Replace("(Clone)", "");
        }

        ds = pdata.GetComponent<DCScript>();
    }

    public void ActivateEndWindow(string _szMessage, bool _bBattleWon)
    {
        GetComponentInChildren<Text>().text = _szMessage;
        gameObject.SetActive(true);
        m_bBattleWon = _bBattleWon;
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
                if (m_bBattleWon == true)
                {
                    string previousField = ds.GetPreviousFieldName();
                    ds.SetPreviousFieldName(SceneManager.GetActiveScene().name);
                    SceneManager.LoadScene(previousField);
                }
                else
                {
                    SceneManager.LoadScene("GameOver_Scene");
                }
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

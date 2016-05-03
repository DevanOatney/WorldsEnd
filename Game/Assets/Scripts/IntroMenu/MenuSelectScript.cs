using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuSelectScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{

	public int m_nIndex;

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().SetHighlightedIndex(m_nIndex);
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().ChangeHighlightedPosition();
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().MouseSelection(m_nIndex);
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().SetHighlightedIndex(m_nIndex);
		GameObject.Find("Highlighter").GetComponent<IntroMenuHighlightInput>().ChangeHighlightedPosition();
	}

	#endregion


	void Enter()
	{
		switch(m_nIndex)
		{
		case 0:
		{
			//New Game
            SceneManager.LoadScene("Inon_Scene");
		}
			break;
		case 1:
		{
			//Continue
            SceneManager.LoadScene("IntroContinue_Scene");
		}
			break;
		case 2:
		{
			//Settings
            SceneManager.LoadScene("IntroSettings_Scene");
		}
			break;
		case 3:
		{
			//Exit
			Application.Quit();
		}
			break;
		}
	}
}

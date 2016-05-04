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
		GameObject _highlighter = GameObject.Find("Highlighter");
		if(_highlighter.GetComponent<IntroMenuHighlightInput>().m_bAllowInput == true)
		{
			_highlighter.GetComponent<IntroMenuHighlightInput>().SetHighlightedIndex(m_nIndex);
			_highlighter.GetComponent<IntroMenuHighlightInput>().ChangeHighlightedPosition();
		}
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		GameObject _highlighter = GameObject.Find("Highlighter");
		if(_highlighter.GetComponent<IntroMenuHighlightInput>().m_bAllowInput == true)
		{
			_highlighter.GetComponent<IntroMenuHighlightInput>().MouseSelection(m_nIndex);
			_highlighter.GetComponent<IntroMenuHighlightInput>().SetHighlightedIndex(m_nIndex);
			_highlighter.GetComponent<IntroMenuHighlightInput>().ChangeHighlightedPosition();
		}
	}

	#endregion


	public void Enter()
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

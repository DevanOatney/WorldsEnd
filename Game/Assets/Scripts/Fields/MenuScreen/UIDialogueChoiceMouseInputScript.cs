using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDialogueChoiceMouseInputScript : MonoBehaviour 
{
	public GameObject m_goListener;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DialogueChoiceHighlighted(int _index)
	{
		m_goListener.GetComponent<MessageHandler> ().UpdateDialogueChoiceIter (_index - 1);
	}
}

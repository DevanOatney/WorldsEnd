using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelectionScript : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
	public enum SIDE{ENEMY, ALLY};
	public SIDE m_esWhichSide;
	GameObject m_goTurnWatcher;
	// Use this for initialization
	void Start () 
	{
		m_goTurnWatcher = GameObject.Find ("TurnWatcher");
		if (transform.parent.GetComponent<CAllyBattleScript> () != null)
			m_esWhichSide = SIDE.ALLY;
		else
			m_esWhichSide = SIDE.ENEMY;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		if (m_esWhichSide == SIDE.ALLY)
			m_goTurnWatcher.GetComponent<TurnWatcherScript> ().Ally_TargetHighlighted (transform.parent.gameObject.GetComponent<CAllyBattleScript> ().FieldPosition);
		else
			m_goTurnWatcher.GetComponent<TurnWatcherScript> ().Enemy_TargetHighlighted (transform.parent.gameObject.GetComponent<StandardEnemyScript> ().FieldPosition);
	}

	#endregion

	#region IPointerClickHandler implementation

	public void OnPointerClick (PointerEventData eventData)
	{
		if (m_esWhichSide == SIDE.ALLY)
			m_goTurnWatcher.GetComponent<TurnWatcherScript> ().Ally_TargetSelected (transform.parent.gameObject.GetComponent<CAllyBattleScript> ().FieldPosition);
		else
			m_goTurnWatcher.GetComponent<TurnWatcherScript> ().Enemy_TargetSelected (transform.parent.gameObject.GetComponent<StandardEnemyScript> ().FieldPosition);
	}

	#endregion
}

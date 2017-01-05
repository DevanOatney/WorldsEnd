using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class SpellInSpellListScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	GameObject m_goSpellWindow;
	SpellLibrary.cSpellData m_sSpell;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(GameObject _spellWindow, SpellLibrary.cSpellData _theSpell)
	{
		m_goSpellWindow = _spellWindow;
		m_sSpell = _theSpell;
	}

	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		m_goSpellWindow.transform.FindChild ("SpellName").GetComponentInChildren<Text> ().text = m_sSpell.m_szSpellName;
		m_goSpellWindow.transform.FindChild ("SpellElement").GetComponentInChildren<Text> ().text = m_sSpell.m_nElementType.ToString();
		m_goSpellWindow.transform.FindChild ("SpellDescription").GetComponentInChildren<Text> ().text = m_sSpell.m_szDescription;
		Vector2 _ancPos = m_goSpellWindow.GetComponent<RectTransform> ().position;
		_ancPos.y = GetComponent<RectTransform> ().position.y;
		m_goSpellWindow.GetComponent<RectTransform> ().position = _ancPos;
		m_goSpellWindow.SetActive (true);

	}

	#endregion

	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		m_goSpellWindow.SetActive (false);
	}

	#endregion
}

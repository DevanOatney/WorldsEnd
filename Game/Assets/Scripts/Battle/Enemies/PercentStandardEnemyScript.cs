using UnityEngine;
using System.Collections;

public class PercentStandardEnemyScript : StandardEnemyScript 
{
	public float m_fPercentToEndFight = 0.25f;

	//Used for if this enemy can now actually be killed
	bool m_bShouldIgnorePercent = false;

	// Use this for initialization
	void Start () 
	{
		Initialize();
		CheckUnitChange();
	}

	void CheckUnitChange()
	{
		//used for if this unit can now actually be killed, unsure what this will become, currently just check if it's the
		//second time you fight the boar boss
		int result;
		GameObject dc = GameObject.Find("PersistantData");
		if(dc.GetComponent<DCScript>().m_dStoryFlagField.TryGetValue("ToAEvent", out result))
		{
			m_bShouldIgnorePercent = true;
		}
	}


	// Update is called once per frame
	void Update () 
	{
		HandleStates();
	}
	
	new public void Missed()
	{
		GameObject newText = Instantiate(m_goFadingText);
		newText.GetComponent<GUI_FadeText>().SetColor(true);
		newText.GetComponent<GUI_FadeText>().SetText("Miss");
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;
	}
	
	override public void AdjustHP(int dmg)
	{
		GameObject newText = Instantiate(m_goFadingText);
		if(dmg >= 0)
		{
			dmg = dmg - m_nDef;
			if(dmg < 1)
				dmg = 1;
		}
		if(dmg < 0)
			dmg = 0;
		m_nCurHP -= dmg;
		if(m_nCurHP / m_nMaxHP <= m_fPercentToEndFight && m_bShouldIgnorePercent == false)
		{
			m_nState = (int)ENEMY_STATES.eIDLE;
			GameObject tw = GameObject.Find("TurnWatcher");
			if(tw)
			{
				tw.GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, 0.0f);
			}
		}
		else if(m_nCurHP <= 0)
		{
			m_nState = (int)ENEMY_STATES.eDEAD;
			m_aAnim.SetBool("m_bIsDying", true);
		}
		else
		{
			m_aAnim.SetTrigger("m_bIsDamaged");
		}
		if(dmg >= 0)
			newText.GetComponent<GUI_FadeText>().SetColor(true);
		else
			newText.GetComponent<GUI_FadeText>().SetColor(false);

		newText.GetComponent<GUI_FadeText>().SetText((Mathf.Abs(dmg)).ToString());
		newText.GetComponent<GUI_FadeText>().SetShouldFloat(true);
		Vector3 textPos = transform.GetComponent<Collider>().transform.position;
		textPos.y += (gameObject.GetComponent<BoxCollider>().size.y * 0.75f);
		textPos = Camera.main.WorldToViewportPoint(textPos);
		newText.transform.position = textPos;

	}

	public void Dead()
	{
		IDied ();
	}
	override public void IDied()
	{
		GameObject.Find("TurnWatcher").GetComponent<TurnWatcherScript>().RemoveMeFromList(gameObject, 0.0f);
		Destroy(gameObject);
	}
}

using UnityEngine;
using System.Collections;

public class AnimatedAllyBattleScript : MonoBehaviour 
{

	void AttackAnimationEnd() 
	{
		transform.parent.gameObject.GetComponent<CAllyBattleScript>().AttackAnimationEnd();
	}

	void DamagedAnimationOver()
	{
		transform.parent.gameObject.GetComponent<CAllyBattleScript>().DamagedAnimationOver();
	}
}

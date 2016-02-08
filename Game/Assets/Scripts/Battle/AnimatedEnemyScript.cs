using UnityEngine;
using System.Collections;

public class AnimatedEnemyScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void EndParentAttack()
	{
		transform.parent.GetComponent<UnitScript>().AttackAnimationEnded();
	}
	void IDied()
	{
		transform.parent.GetComponent<UnitScript>().IDied();
	}
}

using UnityEngine;
using System.Collections;

public class WB_ArrowScript : MonoBehaviour
{
	public GameObject m_goWarBattleWatcher;
	public enum SIDE
	{
		eLEFT,
		eRIGHT}

	;
	public SIDE m_eSide;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter2D(Collider2D c)
	{
		if(c.gameObject.tag == "Fire")
		{
			int _roll = Random.Range (1, 3);
			if (_roll == 1)
			{
				if(m_eSide == SIDE.eLEFT)
					m_goWarBattleWatcher.GetComponent<FightSceneControllerScript> ().KillUnit (false);
				else
					m_goWarBattleWatcher.GetComponent<FightSceneControllerScript> ().KillUnit (true);
			}
		}
	}

    public void ArrowArrived()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}

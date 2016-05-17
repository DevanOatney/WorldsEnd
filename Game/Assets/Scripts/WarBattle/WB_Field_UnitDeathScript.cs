using UnityEngine;
using System.Collections;

public class WB_Field_UnitDeathScript : MonoBehaviour
{
    GameObject m_goOwner;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Init(GameObject _owner)
    {
        m_goOwner = _owner;
    }

    public void DeathAnimOver()
    {
        m_goOwner.GetComponent<TRPG_UnitScript>().DeathAnimationEnd();
        Destroy(gameObject.transform.parent.gameObject);
    }
}

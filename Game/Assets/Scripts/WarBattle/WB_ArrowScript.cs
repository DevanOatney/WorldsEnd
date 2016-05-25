using UnityEngine;
using System.Collections;

public class WB_ArrowScript : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ArrowArrived()
    {
        Destroy(gameObject.transform.parent.gameObject);
    }
}

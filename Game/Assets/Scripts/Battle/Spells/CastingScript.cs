using UnityEngine;
using System.Collections;

public class CastingScript : MonoBehaviour 
{
	public enum Elements{Fire, Water, Earth, Wind, Light, Dark};
	public Elements m_eElement;
	float m_fRotationSpeed = 50.0f;
	GameObject m_goBeamToSpawn;

	float m_fBeamSpawnTimer = 0.3f;
	float m_fBeamSpawnBucket = 0.2f;
	public GameObject m_goOwner;
	// Use this for initialization
	void Start () 
	{
		switch(m_eElement)
		{
		case Elements.Fire:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/RedBeam") as GameObject;
			}
			break;
		case Elements.Water:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/BlueBeam") as GameObject;
			}
			break;
		case Elements.Earth:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/YellowBeam") as GameObject;
			}
			break;
		case Elements.Wind:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/GreenBeam") as GameObject;
			}
			break;
		case Elements.Light:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/TealBeam") as GameObject;
			}
			break;
		case Elements.Dark:
			{
				m_goBeamToSpawn = Resources.Load("Animation Effects/Spell Effects/CastEffects/Beams/PinkBeam") as GameObject;
			}
			break;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(Vector3.back * m_fRotationSpeed * Time.deltaTime);

		m_fBeamSpawnTimer += Time.deltaTime;
		if(m_fBeamSpawnTimer >= m_fBeamSpawnBucket)
		{
			Vector3 newPos = transform.position;
			newPos.x += Random.Range(GetComponent<SpriteRenderer>().bounds.size.x * -0.5f, GetComponent<SpriteRenderer>().bounds.size.x * 0.5f);
			GameObject tempBeam = Instantiate(m_goBeamToSpawn, newPos , Quaternion.identity) as GameObject;
			Destroy(tempBeam, 1.2f);
			m_fBeamSpawnTimer = 0.0f;
		}
	}

	void EndAnimation()
	{
		m_goOwner.GetComponent<UnitScript>().ChargingOver();
		Destroy(gameObject);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TRPG_UnitScript : MonoBehaviour 
{
	//Each time an NPC comes up with a list of spots it wants to go, the request manager will spit back the optimal path to each (if there is one), add successful paths to this list
	List<Vector3[]> m_lvPathsFound = new List<Vector3[]>();
	//This represents all of the locations that the NPC could want to go to this turn and a numerical value for which it wants to go to the most (highest int value)
	List<cDesiredLocation> m_lDesiredLocationsThisTurn = new List<cDesiredLocation>();
	//Hook to the best path found to the location this unit wants to go to, once it's found.
	Vector3[] m_vDesiredPath;
	//Flag for if it's this units turn or not
	public bool m_bIsMyTurn = false;
	//Once the path requests are sent to be processed, the unit will hang until this flag is toggled to true
	bool m_bPathFound = false;
	//flag for when the unit arrives at their destination
	bool m_bArrivedAtDestination = false;
	//Integral count of how many paths have returned from the request manager, succesful or not, to know when each location has finished processing.
	int m_nCountOfPathsFound = 0;
	//iter for traversing the best path, once it's found.
	int m_nPathIter = 0;
	//The data needed for battles
	FightSceneControllerScript.cWarUnit m_wuUnitData;
	//Hook to the prefab for the base unit animation for this group of units
	public GameObject m_goBaseUnitPrefab;
	float m_fMovementTimer = 0.0f;
	float m_fMovementBucket = 1.0f;



	//TEMP TO TEST MOVEMENT
	public GameObject TEMP_MovementLocation;

	class cDesiredLocation
	{
		public cDesiredLocation(Vector3 _desLoc, int _priority) { m_vDesiredLoc = _desLoc; m_nPriority = _priority;}
		public Vector3 m_vDesiredLoc;
		public int m_nPriority;
	}
	// Use this for initialization
	void Start () 
	{
		m_wuUnitData = new FightSceneControllerScript.cWarUnit(null, m_goBaseUnitPrefab, 1.0f, 10, 4, 4, 4);
	}



	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.B))
		{
			m_bIsMyTurn = true;
			AddLocationToMoveTo(TEMP_MovementLocation.transform.localPosition, 1);
		}
		if(m_bIsMyTurn == true)
		{
			if(m_bPathFound == true)
			{
				m_fMovementTimer += Time.deltaTime;
				if(m_fMovementTimer >= m_fMovementBucket)
				{
					Vector3 _nextPos = m_vDesiredPath[m_nPathIter];
					float _nodeRadius = CPathRequestManager.m_Instance.m_psPathfinding.grid.nodeRadius;
					_nextPos.x = _nextPos.x + (_nodeRadius*0.5f) - (GetComponent<Collider2D>().bounds.size.x * 0.5f);
					_nextPos.y = _nextPos.y - (_nodeRadius*0.5f) + (GetComponent<Collider2D>().bounds.size.y * 0.5f);
					transform.position = _nextPos;

					m_nPathIter += 1;
					if(m_nPathIter >= m_vDesiredPath.Length)
					{
						//reached destination.
						m_bPathFound = false;
						m_bArrivedAtDestination = true;
						//TEMP TO TEST MOVEMENT
						EndTurn();
					}
					m_fMovementTimer = 0.0f;
				}
			}
		}
	}

	void AddLocationToMoveTo(Vector3 _location, int _priority)
	{
		m_lDesiredLocationsThisTurn.Add(new cDesiredLocation(_location, _priority));
		CPathRequestManager.RequestPath(transform.localPosition, _location, PathReturned);
	}

	void ProcessReturnedPaths()
	{
		if(m_lvPathsFound.Count <= 0)
		{
			//No valid paths were returned, can't move this turn.
			return;
		}
		int _nHighestPriority = -1;
		int _nIndexToBest = 0;
		int _nCounter = 0;
		foreach(Vector3[] _path in m_lvPathsFound)
		{
			Vector3 _endLoc = _path[_path.Length-1];
			foreach(cDesiredLocation _desiredLoc in m_lDesiredLocationsThisTurn)
			{
				if(_desiredLoc.m_vDesiredLoc == _endLoc)
				{
					//Found the path to this specific desired location.
					if(_desiredLoc.m_nPriority > _nHighestPriority)
					{
						//This NPC would rather go here than any previous location we've looked at.
						_nHighestPriority = _desiredLoc.m_nPriority;
						_nIndexToBest = _nCounter;
					}
				}
			}
			++_nCounter;
		}
		m_vDesiredPath = m_lvPathsFound[_nIndexToBest];
		m_bPathFound = true;
	}

	void PathReturned(Vector3[] _vPath, bool _bFoundPath)
	{
		m_nCountOfPathsFound += 1;
		if(_bFoundPath == true)
		{
			//was able to find a path to the desired destination.
			m_lvPathsFound.Add(_vPath);
		}
		else
		{
			//Was not able to find a path to desired destination.
		}
		if(m_nCountOfPathsFound >= m_lDesiredLocationsThisTurn.Count)
		{
			//All paths have been returned, time to figure out which to go to.
			ProcessReturnedPaths();
		}
	}

	public void DeathAnimationEnd()
	{
		EndTurn();
		Destroy(gameObject);
	}

	public void KillUnit()
	{
		EndTurn();
	}

	public void EndTurn()
	{
		m_nCountOfPathsFound = 0;
		m_nPathIter = 0;
		m_bIsMyTurn = false;
		m_bPathFound = false;
		m_bArrivedAtDestination = false;
		m_lvPathsFound.Clear();
		m_lDesiredLocationsThisTurn.Clear();
		m_vDesiredPath = null;
	}


	public void OnDrawGizmos() 
	{
		if (m_vDesiredPath != null) 
		{
			for (int i = m_nPathIter; i < m_vDesiredPath.Length; i ++) 
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(m_vDesiredPath[i], Vector3.one);

				if (i == m_nPathIter) {
					Gizmos.DrawLine(transform.position, m_vDesiredPath[i]);
				}
				else {
					Gizmos.DrawLine(m_vDesiredPath[i-1],m_vDesiredPath[i]);
				}
			}
		}
	}
}

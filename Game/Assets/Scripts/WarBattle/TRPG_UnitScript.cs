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
    //Flag for if this unit has acted this turn
    public bool m_bHasActedThisTurn = false;
	//Once the path requests are sent to be processed, the unit will hang until this flag is toggled to true
	bool m_bPathFound = false;
	//Integral count of how many paths have returned from the request manager, succesful or not, to know when each location has finished processing.
	int m_nCountOfPathsFound = 0;
	//iter for traversing the best path, once it's found.
	int m_nPathIter = 0;
	//The data needed for battles
    [HideInInspector]
	public FightSceneControllerScript.cWarUnit m_wuUnitData;
	//Hook to the prefab for the base unit animation for this group of units
	public GameObject m_goBaseUnitPrefab;
	float m_fMovementTimer = 0.0f;
	float m_fMovementBucket = 0.5f;
    public CNode m_cPositionOnGrid;


	class cDesiredLocation
	{
		public cDesiredLocation(Vector3 _desLoc, int _priority) { m_vDesiredLoc = _desLoc; m_nPriority = _priority;}
		public Vector3 m_vDesiredLoc;
		public int m_nPriority;
	}
    void Awake()
    {
        m_wuUnitData = new FightSceneControllerScript.cWarUnit(null, m_goBaseUnitPrefab, 1.0f, 10, 4, 4, 4, 1, 5);
        m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(transform.position);
    }
	// Use this for initialization
	void Start () 
	{
        
	}



	// Update is called once per frame
	void Update () 
	{
        float _fWidth = CPathRequestManager.m_Instance.m_psPathfinding.grid.m_fNodeWidth;
        float _fHeight = CPathRequestManager.m_Instance.m_psPathfinding.grid.m_fNodeHeight;
        if (_fWidth != transform.localScale.x || _fHeight != transform.localScale.y)
        {
            //the map has been resized since we last checked.
            Vector3 _newScale = new Vector3(_fWidth, _fHeight, 1.0f);
            transform.localScale = _newScale;
            transform.position = m_cPositionOnGrid.worldPosition;
        }


		if(m_bIsMyTurn == true)
		{
			if(m_bPathFound == true)
			{
				m_fMovementTimer += Time.deltaTime;
				if(m_fMovementTimer >= m_fMovementBucket)
				{
					Vector3 _nextPos = m_vDesiredPath[m_nPathIter];
                    m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_nextPos);
					transform.position = m_cPositionOnGrid.worldPosition;

					m_nPathIter += 1;
					if(m_nPathIter >= m_vDesiredPath.Length)
					{
						//reached destination.
                        MovementFinished();
                        GameObject.Find("WarBattleWatcher").GetComponent<WarBattleWatcherScript>().MovementFinished(gameObject);
					}
					m_fMovementTimer = 0.0f;
				}
			}
		}
	}

    public void MapResized()
    {
        float _fWidth = CPathRequestManager.m_Instance.m_psPathfinding.grid.m_fNodeWidth;
        float _fHeight = CPathRequestManager.m_Instance.m_psPathfinding.grid.m_fNodeHeight;
        if (_fWidth != transform.localScale.x || _fHeight != transform.localScale.y)
        {
            //the map has been resized since we last checked.
            Vector3 _newScale = new Vector3(_fWidth, _fHeight, 1.0f);
            transform.localScale = _newScale;
            transform.position = m_cPositionOnGrid.worldPosition;
        }
    }
    public void MoveToLocation(Vector3 _location)
    {
        AddLocationToMoveTo(_location, 2);
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
		
		if(_bFoundPath == true)
		{
			//was able to find a path to the desired destination.
			m_lvPathsFound.Add(_vPath);
            m_nCountOfPathsFound += 1;
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

    public CNode GetIndexForUnit()
    {
        return CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(transform.position);
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

    void MovementFinished()
    {
        m_nCountOfPathsFound = 0;
        m_nPathIter = 0;
        m_bIsMyTurn = false;
        m_bPathFound = false;
        m_lvPathsFound.Clear();
        m_lDesiredLocationsThisTurn.Clear();
        m_vDesiredPath = null;
    }

	public void EndTurn()
	{
        m_bIsMyTurn = false;
        m_bHasActedThisTurn = true;
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

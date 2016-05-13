using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarBattleWatcherScript : MonoBehaviour
{
    //hooks to gameobjects
    public GameObject m_goActionWindow;
    public GameObject m_goHighlighter;
    public GameObject m_goSelector;
    //game object for the map and starting positional data
    GameObject m_goMapData;
    DCScript dc;
    List<GameObject> m_lAllies = new List<GameObject>();
    List<GameObject> m_lEnemies = new List<GameObject>();
    Vector2 m_vSelectorGridPos = Vector2.zero;
    bool m_bIsAllyTurn = true;
    GameObject m_goSelectedUnit = null;

    void Awake()
    {
        m_lAllies.Clear();
        m_lEnemies.Clear();
        GameObject pdata = GameObject.Find("PersistantData");
        if (pdata == null)
        {

            //This is a debug play then.   Create a data canister, and put the main character in the party
            pdata = Instantiate(Resources.Load("Misc/PersistantData", typeof(GameObject))) as GameObject;
            pdata.name = pdata.name.Replace("(Clone)", "");
            dc = pdata.GetComponent<DCScript>();
            dc.m_szWarBattleDataPath = "Test_Map";
        }
        else
            dc = GameObject.Find("PersistantData").GetComponent<DCScript>();
    }
	// Use this for initialization
	void Start ()
    {
        LoadInMap();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_bIsAllyTurn == true)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                m_vSelectorGridPos.y -= 1;
                if (m_vSelectorGridPos.y >= CPathRequestManager.m_Instance.m_psPathfinding.grid.gridWorldSize.y)
                {
                    m_vSelectorGridPos.y -= 1;
                }
                CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_vSelectorGridPos);
                m_goSelector.transform.position = _base.worldPosition;

            }
            else if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                m_vSelectorGridPos.y += 1;
                if (m_vSelectorGridPos.y < CPathRequestManager.m_Instance.m_psPathfinding.grid.gridWorldSize.y * -1)
                {
                    m_vSelectorGridPos.y = 0;
                }
                CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_vSelectorGridPos);
                m_goSelector.transform.position = _base.worldPosition;
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                m_vSelectorGridPos.x += 1;
                if (m_vSelectorGridPos.x >= CPathRequestManager.m_Instance.m_psPathfinding.grid.gridWorldSize.x)
                {
                    m_vSelectorGridPos.x -= 1;
                }
                CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_vSelectorGridPos);
                m_goSelector.transform.position = _base.worldPosition;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                m_vSelectorGridPos.x -= 1;
                if (m_vSelectorGridPos.x < CPathRequestManager.m_Instance.m_psPathfinding.grid.gridWorldSize.x * -1)
                {
                    m_vSelectorGridPos.x = 0;
                }
                CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_vSelectorGridPos);
                m_goSelector.transform.position = _base.worldPosition;
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (m_goSelectedUnit == null)
                {
                    CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNodeFromIndex(m_vSelectorGridPos);
                    foreach (GameObject _go in m_lAllies)
                    {
                        if (_unitNode.worldPosition == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit().worldPosition)
                        {
                            m_goSelectedUnit = _go;
                            break;
                        }
                    }
                }
                else
                {

                    CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelectedUnit.transform.position);
                    CNode _destination = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNodeFromIndex(m_vSelectorGridPos);
                    Vector2 _strtDest = new Vector2(_unitNode.gridX, _unitNode.gridY);
                    Vector2 _endDest = new Vector2(_destination.gridX, _destination.gridY);
                    float _distance = Mathf.Sqrt(Mathf.Pow((_endDest.x - _strtDest.x), 2) + Mathf.Pow((_endDest.y - _strtDest.y), 2));
                    if (_distance < m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange)
                    {
                        m_goSelectedUnit.GetComponent<TRPG_UnitScript>().MoveToLocation(new Vector3(_endDest.x, _endDest.y, 0.0f));
                    }
                    m_goSelectedUnit = null;
                }
            }
        }
	}

    public void StartMyTurn(GameObject p_unit)
    {
        m_goSelector.transform.position = p_unit.transform.position;
        int rng = p_unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange;
        List<CNode> _lNeighbors = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighborNodes(p_unit.transform.position, p_unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange);
        foreach (CNode _neigh in _lNeighbors)
        {
            GameObject _movementHighlight = Instantiate(m_goHighlighter) as GameObject;
            Vector3 _pos = _movementHighlight.transform.position;
            _movementHighlight.GetComponent<SpriteRenderer>().enabled = true;
            _movementHighlight.transform.position = _neigh.worldPosition;
        }
    }

    public void EndMyTurn(GameObject p_unit)
    {
    }

    void LoadInMap()
    {
        m_goMapData = Instantiate(Resources.Load<GameObject>("WarBattleData/" + dc.m_szWarBattleDataPath));
        foreach (Transform child in m_goMapData.transform)
        {
            GameObject _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + child.name));
            _unit.transform.position = child.position;
            if (child.tag == "Enemy")
                m_lEnemies.Add(_unit);
            else if (child.tag == "Ally")
            {
                m_lAllies.Add(_unit);
                StartMyTurn(_unit);
            }
        }
    }
}

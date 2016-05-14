using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarBattleWatcherScript : MonoBehaviour
{
    enum War_States { eMovement, eWaitForMovement, eAttack, eMagic, eEndingAction}
    //hooks to gameobjects
    public GameObject m_goBattleScreen;
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
    bool m_bAllowInput = false;
    //Currently selected unit on the map
    GameObject m_goSelectedUnit = null;
    //hook to the highlighted squares showing where the unit can move.. attack.. whatever (keeping it so that we can make sure to destroy them)
    List<GameObject> m_lHighlightedSquares = new List<GameObject>();
    //The previous location that the unit that is currently was moving was at, so that if they hit cancel in the action window it will move them back to their previous position like nothing happened
    CNode m_cPreviousUnitPosition = null;
    int m_nState = 0;

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
        m_bAllowInput = true;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (m_bIsAllyTurn == true && m_bAllowInput == true)
        {
            HandleInput();
        }
	}

    void HandleInput()
    {
        switch (m_nState)
        {
            case (int)War_States.eMovement:
                {
                    BasicInputFunc();
                }
                break;
            case (int)War_States.eAttack:
                {
                    BasicInputFunc();
                }
                break;
            case (int)War_States.eMagic:
                {
                    BasicInputFunc();
                }
                break;
        }
    
    }

    void BasicInputFunc()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
            Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX, _base.gridY - 1));
            if (newPos.z != 500)
                m_goSelector.transform.position = newPos;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
            Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX, _base.gridY + 1));
            if (newPos.z != 500)
                m_goSelector.transform.position = newPos;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
            Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX + 1, _base.gridY));
            if (newPos.z != 500)
                m_goSelector.transform.position = newPos;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
            Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX - 1 + m_vSelectorGridPos.x, _base.gridY));
            if (newPos.z != 500)
                m_goSelector.transform.position = newPos;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            if (m_nState == (int)War_States.eMovement)
            {
                MovementConfirm();
            }
            else if (m_nState == (int)War_States.eAttack)
            {
                AttackConfirm();
            }
            else if (m_nState == (int)War_States.eMagic)
            {
                MagicConfirm();
            }

        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_nState == (int)War_States.eAttack || m_nState == (int)War_States.eMagic)
            {
                m_bAllowInput = false;
                ClearHighlightedSquares();
                m_goActionWindow.GetComponent<ActionWindowScript>().ActivateWindow(m_goSelectedUnit);
            }
            else if (m_nState != (int)War_States.eWaitForMovement && m_nState != (int)War_States.eEndingAction)
            {
                ActionCancelled();
            }
        }
    }

    public void ActionCancelled()
    {
        if (m_cPreviousUnitPosition != null)
            m_goSelectedUnit.transform.position = m_cPreviousUnitPosition.worldPosition;
        m_cPreviousUnitPosition = null;
        m_goSelectedUnit = null;
        ClearHighlightedSquares();
        m_nState = (int)War_States.eMovement;
        m_bAllowInput = true;
        m_goActionWindow.SetActive(false);
    }

    void MovementConfirm()
    {
        if (m_goSelectedUnit == null)
        {
            CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
            foreach (GameObject _go in m_lAllies)
            {
                //Is this object on the same grid position as the selector?
                if (_unitNode.worldPosition == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit().worldPosition)
                {
                    //Is this unit an ally?
                    if (_go.tag == "Ally")
                    {
                        ShowHighlightedSquares(_go, _go.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange, Color.yellow);
                        _go.GetComponent<TRPG_UnitScript>().m_bIsMyTurn = true;
                        m_goSelectedUnit = _go;
                    }
                    break;
                }
            }
        }
        else
        {
            //Check to make sure this unit hasn't already acted this turn
            if (m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn == false)
            {
                CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelectedUnit.transform.position);
                CNode _destination = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
                if (_unitNode == _destination)
                {
                    //early exit, the unit is trying to move to the same square it's already in, which is fine.
                    m_cPreviousUnitPosition = _unitNode;
                    ClearHighlightedSquares();
                    m_nState = (int)War_States.eWaitForMovement;
                    MovementFinished(m_goSelectedUnit);
                    return;
                }
                //check to make sure that no other unit is in the position you're trying to move toward
                foreach (GameObject _go in m_lAllies)
                {
                    CNode _node = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_go.transform.position);
                    if (_node == _destination)
                    {
                        //this spot has a different unit on it, you can't move here.
                        return;
                    }
                }
                foreach (GameObject _go in m_lEnemies)
                {
                    CNode _node = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_go.transform.position);
                    if (_node == _destination)
                    {
                        //this spot has a different unit on it, you can't move here.
                        return;
                    }
                }
                //check to make sure this distance is moveable by the unit
                Vector2 _strtDest = new Vector2(_unitNode.gridX, _unitNode.gridY);
                Vector2 _endDest = new Vector2(_destination.gridX, _destination.gridY);
                float _distance = Mathf.Sqrt(Mathf.Pow((_endDest.x - _strtDest.x), 2) + Mathf.Pow((_endDest.y - _strtDest.y), 2));
                if (_distance <= m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange)
                {
                    m_bAllowInput = false;
                    m_cPreviousUnitPosition = _unitNode;
                    ClearHighlightedSquares();
                    m_nState = (int)War_States.eWaitForMovement;
                    m_goSelectedUnit.GetComponent<TRPG_UnitScript>().MoveToLocation(_destination.worldPosition);
                }
            }

        }
    }
    public void AttackChoiceSelected()
    {
        m_nState = (int)War_States.eAttack;
        m_bAllowInput = true;
    }
    void AttackConfirm()
    {
        //Let's first find out if this is a valid selection of a unit to attack...
        CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
        GameObject _target = null;
        if (m_goSelectedUnit.tag == "Ally")
        {
            foreach (GameObject _go in m_lEnemies)
            {
                //Is this object on the same grid position as the selector?
                if (_unitNode.worldPosition == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit().worldPosition)
                {
                    _target = _go;
                }
            }
        }
        else
        {
            foreach (GameObject _go in m_lAllies)
            {
                //Is this object on the same grid position as the selector?
                if (_unitNode.worldPosition == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit().worldPosition)
                {
                    _target = _go;
                }
            }
        }
        if (_target != null)
        {
            ClearHighlightedSquares();
            m_bAllowInput = false;
            m_nState = (int)War_States.eEndingAction;
            m_goBattleScreen.SetActive(true);
            m_goBattleScreen.GetComponent<FightSceneControllerScript>().SetupBattleScene(_target.GetComponent<TRPG_UnitScript>().m_wuUnitData, m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData);
        }
        
        
    }
    public void MagicChoiceSelected()
    {
        m_nState = (int)War_States.eMagic;
    }
    public void MagicConfirm()
    {
    }

    public void MovementFinished(GameObject _unit)
    {
        m_goActionWindow.GetComponent<ActionWindowScript>().ActivateWindow(m_goSelectedUnit);
    }

    public void ClearHighlightedSquares()
    {
        foreach (GameObject _go in m_lHighlightedSquares)
        {
            Destroy(_go);
        }
        m_lHighlightedSquares.Clear();
    }

    public void ShowHighlightedSquares(GameObject p_unit, int _rng, Color _col)
    {
        _col.a = 0.2f;
        ClearHighlightedSquares();
        //m_goSelector.transform.position = p_unit.transform.position;
        List<CNode> _lNeighbors = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighborNodes(p_unit.transform.position, _rng);
        foreach (CNode _neigh in _lNeighbors)
        {
            GameObject _movementHighlight = Instantiate(m_goHighlighter) as GameObject;
            _movementHighlight.GetComponent<SpriteRenderer>().enabled = true;
            _movementHighlight.transform.position = _neigh.worldPosition;
            _movementHighlight.GetComponent<SpriteRenderer>().color = _col;
            m_lHighlightedSquares.Add(_movementHighlight);
        }
    }

    public void EndMyTurn(GameObject p_unit)
    {
        m_nState = (int)War_States.eMovement;
        m_goSelectedUnit = null;
        m_cPreviousUnitPosition = null;
        m_bAllowInput = true;
    }

    public void BattleSceneEnded(FightSceneControllerScript.cWarUnit _defender, FightSceneControllerScript.cWarUnit _attacker)
    {
        EndMyTurn(m_goSelectedUnit);
    }

    void LoadInMap()
    {
        m_goMapData = Instantiate(Resources.Load<GameObject>("WarBattleData/" + dc.m_szWarBattleDataPath));
        foreach (Transform child in m_goMapData.transform)
        {
            GameObject _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + child.name));
            _unit.transform.position = child.position;
            if (child.tag == "Enemy")
            {
                _unit.tag = "Enemy";
                m_lEnemies.Add(_unit);
            }
            else if (child.tag == "Ally")
            {
                _unit.tag = "Ally";
                m_lAllies.Add(_unit);
            }
        }
    }
}

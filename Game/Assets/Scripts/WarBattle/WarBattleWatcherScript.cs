using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WarBattleWatcherScript : MonoBehaviour
{
    enum War_States { eMovement, eWaitForMovement, eAttack, eMagic, eSystem, eEndingAction}
    //hooks to gameobjects
    public GameObject m_goBattleScreen;
    public GameObject m_goActionWindow;
    public GameObject m_goSystemWindow;
    public GameObject m_goBattleOverWindow;
    public GameObject m_goHighlighter;
    public GameObject m_goSelector;
    //game object for the map and starting positional data
    GameObject m_goMapData;
    DCScript dc;
    List<GameObject> m_lAllies = new List<GameObject>();
    List<GameObject> m_lEnemies = new List<GameObject>();
    List<GameObject> m_lGuests = new List<GameObject>();
    Vector2 m_vSelectorGridPos = Vector2.zero;
    public bool m_bIsAllyTurn = true;
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
        GetComponent<WarBattle_EnemyControllerScript>().Initialize(gameObject, m_goActionWindow);
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

    /// <summary>
    /// Moves the highlighting cursor in a direction.
    /// </summary>
    /// <param name="_direction">0 : Down, 1: Left, 2: Right, 3: Up.</param>
    public void MoveCursor(int _direction)
    {
        switch(_direction)
        {
            case 0:
                {
                    //Down
                    CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
                    Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX, _base.gridY - 1));
                    if (newPos.z != 500)
                        m_goSelector.transform.position = newPos;
                }
                break;
            case 1:
                {
                    //Left
                    CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
                    Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX - 1 + m_vSelectorGridPos.x, _base.gridY));
                    if (newPos.z != 500)
                        m_goSelector.transform.position = newPos;
                }
                break;
            case 2:
                {
                    //Right
                    CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
                    Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX + 1, _base.gridY));
                    if (newPos.z != 500)
                        m_goSelector.transform.position = newPos;
                }
                break;
            case 3:
                {
                    //Up
                    CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
                    Vector3 newPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.WorldPointFromIndex(new Vector2(_base.gridX, _base.gridY + 1));
                    if (newPos.z != 500)
                        m_goSelector.transform.position = newPos;
                }
                break;
        }
    }

    void BasicInputFunc()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveCursor(0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCursor(3);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveCursor(2);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveCursor(1);
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
                if (m_goSelectedUnit != null)
                    ActionCancelled();
                else
                {
                    //This means the player is pressing escape while not having even selected a unit yet, display the system window
                    m_nState = (int)War_States.eSystem;
                    m_goSystemWindow.GetComponent<SystemWindowScript>().ActivateWindow();
                    m_bAllowInput = false;
                }
            }
        }
    }

    public void MapResized()
    {
        //This means that they're messing around with the window size, iterate through all the units and adjust their positions/scales.
        foreach (GameObject _go in m_lAllies)
            _go.GetComponent<TRPG_UnitScript>().MapResized();
        foreach (GameObject _go in m_lEnemies)
            _go.GetComponent<TRPG_UnitScript>().MapResized();
        foreach (GameObject _go in m_lGuests)
            _go.GetComponent<TRPG_UnitScript>().MapResized();
    }

    public void ActionCancelled()
    {
        if (m_cPreviousUnitPosition != null)
        {
            m_goSelectedUnit.transform.position = m_cPreviousUnitPosition.worldPosition;
            m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_cPositionOnGrid = m_cPreviousUnitPosition;
        }
        m_cPreviousUnitPosition = null;
        m_goSelectedUnit = null;
        ClearHighlightedSquares();
        m_nState = (int)War_States.eMovement;
        m_bAllowInput = true;
        m_goActionWindow.SetActive(false);
        m_goSystemWindow.SetActive(false);
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
            if (m_goSelectedUnit == null)
            {
                //This means that the player is selecting a spot that has no unit in it, open up the system menu.
                m_nState = (int)War_States.eSystem;
                m_bAllowInput = false;
                m_goSystemWindow.GetComponent<SystemWindowScript>().ActivateWindow();
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
                Vector3[] _vaPath = CPathRequestManager.m_Instance.m_psPathfinding.FindPathImmediate(_unitNode.worldPosition, _destination.worldPosition);
                if (_vaPath.Length <= m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange)
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
        if (m_bIsAllyTurn == true)
        {
            m_nState = (int)War_States.eAttack;
            m_bAllowInput = true;
        }
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
            CNode _node = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelectedUnit.transform.position);
            CNode _targetNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_target.transform.position);
            Vector2 _strtDest = new Vector2(_node.gridX, _node.gridY);
            Vector2 _endDest = new Vector2(_targetNode.gridX, _targetNode.gridY);
            float _distance = Mathf.CeilToInt(Mathf.Sqrt(Mathf.Pow((_endDest.x - _strtDest.x), 2) + Mathf.Pow((_endDest.y - _strtDest.y), 2)));
            if (_distance <= m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nAttackRange)
            {
                ClearHighlightedSquares();
                m_bAllowInput = false;
                m_nState = (int)War_States.eEndingAction;
                m_goBattleScreen.SetActive(true);
                m_goBattleScreen.GetComponent<FightSceneControllerScript>().SetupBattleScene(_target.GetComponent<TRPG_UnitScript>().m_wuUnitData, m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData, (int)_distance);
            }
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
        if (m_bIsAllyTurn == true)
            m_goActionWindow.GetComponent<ActionWindowScript>().ActivateWindow(m_goSelectedUnit);
        else
        {
            GetComponent<WarBattle_EnemyControllerScript>().UnitMovementFinished();
            m_goActionWindow.GetComponent<ActionWindowScript>().ActivateWindow(GetComponent<WarBattle_EnemyControllerScript>().m_goCurrentUnitActing);
        }
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
        List<CNode> _lNeighbors = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(p_unit.transform.position, _rng);
        foreach (CNode _neigh in _lNeighbors)
        {
            GameObject _movementHighlight = Instantiate(m_goHighlighter) as GameObject;
            _movementHighlight.GetComponent<SpriteRenderer>().enabled = true;
            _movementHighlight.transform.position = _neigh.worldPosition;
            _movementHighlight.GetComponent<SpriteRenderer>().color = _col;
            m_lHighlightedSquares.Add(_movementHighlight);
        }
    }

    public void EndFactionTurn()
    {
        m_bIsAllyTurn = !m_bIsAllyTurn;
        if (m_bIsAllyTurn == true)
        {
            foreach (GameObject _go in m_lAllies)
            {
                _go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn = false;
                _go.GetComponentInChildren<Animator>().SetBool("m_bHasActed", false);
            }
            foreach (GameObject _go in m_lEnemies)
            {
                _go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn = true;
                _go.GetComponentInChildren<Animator>().SetBool("m_bHasActed", true);
            }
        }
        else
        {
            foreach (GameObject _go in m_lAllies)
            {
                _go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn = true;
                _go.GetComponentInChildren<Animator>().SetBool("m_bHasActed", true);
            }
            foreach (GameObject _go in m_lEnemies)
            {
                _go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn = false;
                _go.GetComponentInChildren<Animator>().SetBool("m_bHasActed", false);
            }
            GetComponent<WarBattle_EnemyControllerScript>().StartFactionTurn(m_lEnemies, null, m_lAllies);
        }
    }

    public void EndUnitTurn(GameObject p_unit)
    {
        p_unit.GetComponentInChildren<Animator>().SetBool("m_bHasActed", true);
        p_unit.GetComponent<TRPG_UnitScript>().EndTurn();
        // Let's check if anything died...
        for (int i = m_lAllies.Count - 1; i >= 0; i--)
        {
            if (m_lAllies[i].GetComponent<TRPG_UnitScript>().m_wuUnitData.m_fPercentRemaining < 0.05f)
            {
                GameObject _goCatchUnit = m_lAllies[i];
                GetComponent<WarBattle_EnemyControllerScript>().RemoveUnit(_goCatchUnit);
                _goCatchUnit.GetComponent<TRPG_UnitScript>().KillUnit();
            }
            else
                m_lAllies[i].GetComponent<TRPG_UnitScript>().CheckHP();
            if (m_lAllies.Count <= 0)
            {
                m_bAllowInput = false;
                Lose();
                return;
            }
        }
        for (int i = m_lEnemies.Count - 1; i >= 0; i--)
        {
            if (m_lEnemies[i].GetComponent<TRPG_UnitScript>().m_wuUnitData.m_fPercentRemaining < 0.05f)
            {
                GameObject _goCatchUnit = m_lEnemies[i];
                GetComponent<WarBattle_EnemyControllerScript>().RemoveUnit(_goCatchUnit);
                _goCatchUnit.GetComponent<TRPG_UnitScript>().KillUnit();
            }
            else
                m_lEnemies[i].GetComponent<TRPG_UnitScript>().CheckHP();
            if (m_lEnemies.Count <= 0)
            {
                m_bAllowInput = false;
                Win();
                return;
            }
        }
        for (int i = m_lGuests.Count - 1; i >= 0; i--)
        {
            if (m_lGuests[i].GetComponent<TRPG_UnitScript>().m_wuUnitData.m_fPercentRemaining < 0.05f)
            {
                GameObject _goCatchUnit = m_lGuests[i];
                GetComponent<WarBattle_EnemyControllerScript>().RemoveUnit(_goCatchUnit);
                _goCatchUnit.GetComponent<TRPG_UnitScript>().KillUnit();
            }
            else
                m_lGuests[i].GetComponent<TRPG_UnitScript>().CheckHP();
        }

        Invoke("DelayStartNextUnitTurn", 0.75f);

    }

    //Creating this minor delay so that if something is animating it gives it time before player gets distracted with other thing... kind of a hack, but... I think it should be fine for now
    void DelayStartNextUnitTurn()
    {
        if (m_bIsAllyTurn == true)
        {
            m_nState = (int)War_States.eMovement;
            m_goSelectedUnit = null;
            m_cPreviousUnitPosition = null;
            m_bAllowInput = true;

            //check to see if all of the units on this team have acted, if they have, make sure to end their factions turn
            foreach (GameObject _go in m_lAllies)
            {
                if (_go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn == false)
                    return;
            }
            //if we're here this means that every unit in this party has acted, time to end this factions turn
            EndFactionTurn();
        }
        else
        {
            //check to see if all of the units on this team have acted, if they have, make sure to end their factions turn
            foreach (GameObject _go in m_lEnemies)
            {
                if (_go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn == false)
                    return;
            }
            //if we're here this means that every unit in this party has acted, time to end this factions turn
            EndFactionTurn();
        }
    }

    public void BattleSceneEnded(FightSceneControllerScript.cWarUnit _defender, FightSceneControllerScript.cWarUnit _attacker)
    {
        if (m_bIsAllyTurn)
            EndUnitTurn(m_goSelectedUnit);
        else
            EndUnitTurn(GetComponent<WarBattle_EnemyControllerScript>().m_goCurrentUnitActing);
    }

    void LoadInMap()
    {
        m_goMapData = Instantiate(Resources.Load<GameObject>("WarBattleData/" + dc.m_szWarBattleDataPath));
        foreach (Transform child in m_goMapData.transform)
        {
            GameObject _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + child.name));
            _unit.transform.position = child.position;
            _unit.GetComponent<TRPG_UnitScript>().m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_unit.transform.position);
            if (child.tag == "Enemy")
            {
                _unit.tag = "Enemy";
                _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_szTeamName = "Baddie";
                Vector3 _localScale = _unit.GetComponentInChildren<Animator>().transform.localScale;
                _localScale.x *= -1;
                _unit.GetComponentInChildren<Animator>().transform.localScale = _localScale;

                m_lEnemies.Add(_unit);
            }
            else if (child.tag == "Ally")
            {
                _unit.tag = "Ally";
                _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_szTeamName = "Hero";
                _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nAttackRange = 2;
                m_lAllies.Add(_unit);
            }
            else if (child.tag == "Guest")
            {
                _unit.tag = "Guest";
                m_lGuests.Add(_unit);
            }
        }
    }

    void Win()
    {
        m_goBattleOverWindow.GetComponent<BattleOverScript>().ActivateEndWindow("You have won!", true);
    }

    void Lose()
    {
        m_goBattleOverWindow.GetComponent<BattleOverScript>().ActivateEndWindow("You have lost!", false);
    }
}

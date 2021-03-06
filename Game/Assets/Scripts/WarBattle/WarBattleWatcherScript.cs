﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WarBattleWatcherScript : MonoBehaviour
{
    enum War_States { eMovement, eWaitForMovement, eAttack, eMagic, eSystem, eEndingAction, eTurnOrderWindow}
    public enum Turn_Order { AllyTurn, EnemyTurn, GuestTurn}
    //hooks to gameobjects
    public GameObject m_goBattleScreen;
    public GameObject m_goActionWindow;
    public GameObject m_goSystemWindow;
    public GameObject m_goBattleOverWindow;
    public GameObject m_goHighlighter;
    public GameObject m_goSelector;
    public GameObject m_goCompanyUIWindowRoot;
    public GameObject m_goTurnOrderWindow;
    //game object for the map and starting positional data
    GameObject m_goMapData;
    DCScript dc;
    List<GameObject> m_lAllies = new List<GameObject>();
    List<GameObject> m_lEnemies = new List<GameObject>();
    List<GameObject> m_lGuests = new List<GameObject>();
    Vector2 m_vSelectorGridPos = Vector2.zero;
    public Turn_Order m_bIsAllyTurn = Turn_Order.AllyTurn;
    bool m_bAllowInput = false;
    //Currently selected unit on the map
    GameObject m_goSelectedUnit = null;
    //hook to the highlighted squares showing where the unit can move.. attack.. whatever (keeping it so that we can make sure to destroy them)
    List<GameObject> m_lHighlightedSquares = new List<GameObject>();
    //The previous location that the unit that is currently was moving was at, so that if they hit cancel in the action window it will move them back to their previous position like nothing happened
    CNode m_cPreviousUnitPosition = null;
    int m_nState = 0;
    string m_szBackgroundMusic = "Inon_BGM";

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
    void Start()
    {
        LoadInMap();
        GetComponent<WarBattle_EnemyControllerScript>().Initialize(gameObject, m_goActionWindow);
        m_bAllowInput = true;
        CAudioHelper.Instance.vPlayMusic(CAudioHelper.Instance.eFromName(m_szBackgroundMusic), true, true);

		MoveCursor(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_nState == (int)War_States.eTurnOrderWindow)
        {
            if (Input.anyKeyDown == true)
            {
                m_goTurnOrderWindow.SetActive(false);
                m_nState = (int)War_States.eMovement;
                StartFactionTurn();
            }
        }
        else if (m_bIsAllyTurn == Turn_Order.AllyTurn && m_bAllowInput == true)
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
        switch (_direction)
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
        SelectorChangedPos();
    }

	void LateUpdate()
	{
		CNode _base = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
		m_goSelector.transform.position = _base.worldPosition;
	}
    void BasicInputFunc()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveCursor(0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveCursor(3);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            MoveCursor(2);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
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

                        ShowHighlightedSquares(_go, _go.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_cUnitData.m_nMovementRange, Color.yellow, false);
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
                if (_destination.walkable == false)
                {
                    //early exit, if this node can't even be moved to, ignore it.
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
                Vector3[] _vaPath = CPathRequestManager.m_Instance.m_psPathfinding.FindPathImmediate(_unitNode.worldPosition, _destination.worldPosition);
                if (_vaPath == null)
                    return;
                int _nCost = 0;
                foreach (Vector3 _pos in _vaPath)
                {
                    CNode _tNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_pos);
                    _nCost += _tNode.movementPenalty;
                }
                if (_vaPath.Length + _nCost <= m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_cUnitData.m_nMovementRange)
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
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
        {
            m_nState = (int)War_States.eAttack;
            m_bAllowInput = true;
        }
    }

    public void SelectorChangedPos()
    {
        CNode _selNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goSelector.transform.position);
        foreach (GameObject _go in m_lAllies)
        {
			if (_go == null)
				continue;
            //Is this object on the same grid position as the selector?
            if (_selNode == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit())
            {
                m_goCompanyUIWindowRoot.SetActive(true);
                AdjustCompanyUI(_go.GetComponent<TRPG_UnitScript>().m_wuUnitData, _selNode.worldPosition);
                return;
            }
        }
        foreach (GameObject _go in m_lEnemies)
        {
			if (_go == null)
				continue;
            //Is this object on the same grid position as the selector?
            if (_selNode == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit())
            {
                m_goCompanyUIWindowRoot.SetActive(true);
                AdjustCompanyUI(_go.GetComponent<TRPG_UnitScript>().m_wuUnitData, _selNode.worldPosition);
                return;
            }
        }
        foreach (GameObject _go in m_lGuests)
        {
			if (_go == null)
				continue;
            //Is this object on the same grid position as the selector?
            if (_selNode == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit())
            {
                m_goCompanyUIWindowRoot.SetActive(true);
                AdjustCompanyUI(_go.GetComponent<TRPG_UnitScript>().m_wuUnitData, _selNode.worldPosition);
                return;
            }
        }
        m_goCompanyUIWindowRoot.SetActive(false);
    }

    void AdjustCompanyUI(FightSceneControllerScript.cWarUnit _unit, Vector3 _pos)
    {
        if (_unit == null)
        {
            return;
        }
        GameObject _CompanyUI = m_goCompanyUIWindowRoot.transform.Find("CompanyUI").gameObject;
        //First let's figure out the position of this unit in screen space to determine where best the Company UI window should be located.
        RectTransform _theCanvas = GameObject.Find("Canvas").GetComponent<RectTransform>();
        Vector2 _viewportPos = Camera.main.WorldToViewportPoint(_pos);
        Vector2 _objScreenPos = new Vector2(
            ((_viewportPos.x * _theCanvas.sizeDelta.x) - (_theCanvas.sizeDelta.x * 0.5f)),
            ((_viewportPos.y * _theCanvas.sizeDelta.y) - (_theCanvas.sizeDelta.y * 0.5f)));
        if (_objScreenPos.x < 0)
        {
            //Object is on the left side.
            if (_objScreenPos.y >= 0)
            {
                //Object is on the upper left quadrant.
                _CompanyUI.transform.position = m_goCompanyUIWindowRoot.transform.Find("Pos_4").transform.position;
            }
            else if (_objScreenPos.y < 0)
            {
                //Object is on the lower left quadrant.
                _CompanyUI.transform.position = m_goCompanyUIWindowRoot.transform.Find("Pos_2").transform.position;
            }
        }
        else if (_objScreenPos.x >= 0)
        {
            //Object is on the right side.
            if (_objScreenPos.y >= 0)
            {
                //Object is on the upper right quadrant.
                _CompanyUI.transform.position = m_goCompanyUIWindowRoot.transform.Find("Pos_3").transform.position;
            }
            else if (_objScreenPos.y < 0)
            {
                // Object is on the lower right quadrant
                _CompanyUI.transform.position = m_goCompanyUIWindowRoot.transform.Find("Pos_1").transform.position;
            }
        }

        //Let's see if this group has a leader, if it does, show their portrait, if not show a portrait for whichever army you're fighting (need to get that second option worked out)
		if (_unit.m_goPortrait != null)
        {
            //This one has a leader, portrait it up!
			_CompanyUI.transform.Find("LeaderPortrait").GetComponent<Image>().sprite = _unit.m_goPortrait;
            _CompanyUI.transform.Find("LeaderPortrait").gameObject.SetActive(true);
        }

        //All right, team name!
        _CompanyUI.transform.Find("CompanyName").GetComponent<Text>().text = _unit.m_szTeamName;

        //Attack power!
        _CompanyUI.transform.Find("CompanyATK").GetComponent<Text>().text = "ATK: " + _unit.m_cUnitData.m_nAttackPower.ToString();

        //Defense score
        _CompanyUI.transform.Find("CompanyDEF").GetComponent<Text>().text = "DEF: " + _unit.m_cUnitData.m_nDefensePower.ToString();
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
                if (_unitNode == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit())
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
                if (_unitNode == _go.GetComponent<TRPG_UnitScript>().GetIndexForUnit())
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
            if (_distance <= m_goSelectedUnit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_cUnitData.m_nAttackRange)
            {
                ClearHighlightedSquares();
                m_bAllowInput = false;
                m_nState = (int)War_States.eEndingAction;
                m_goBattleScreen.SetActive(true);
                m_goCompanyUIWindowRoot.SetActive(false);
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
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
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

    public void ShowHighlightedSquares(GameObject p_unit, int _rng, Color _col, bool _bShowUnwalkableNodes)
    {
        _col.a = 0.2f;
        ClearHighlightedSquares();
        List<CNode> _lNeighbors = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(p_unit.transform.position, _rng);
        foreach (CNode _neigh in _lNeighbors)
        {
            if (_bShowUnwalkableNodes == false)
            {
                if (_neigh.walkable == false)
                {
                    //Early exit, if we're not showing unwalkable nodes, and this node is unwalkable, don't show it.
                    continue;
                }
                Vector3[] _vaPathToTarget = CPathRequestManager.m_Instance.m_psPathfinding.FindPathImmediate(p_unit.transform.position, _neigh.worldPosition);
                if (_vaPathToTarget == null)
                {
                    //For some reason it's returning null path in some circumstances (meaning it couldn't find a path)  not sure if this is a bug that needs to be fixed, but let's just error check for now.
                    continue;
                }
                int _nCost = 0;
                foreach (Vector3 _pos in _vaPathToTarget)
                {
                    CNode _tNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_pos);
                    _nCost += _tNode.movementPenalty;
                }
                if (_vaPathToTarget.Length + _nCost > p_unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_cUnitData.m_nMovementRange)
                {
                    //"Early" exit, if we're not showing unwalkable nodes we're assuming this is for movement, and if this node it not within the actual movement range, don't show it.
                    continue;
                }
            }

            GameObject _movementHighlight = Instantiate(m_goHighlighter) as GameObject;
            _movementHighlight.GetComponent<SpriteRenderer>().enabled = true;
            _movementHighlight.transform.position = _neigh.worldPosition;
            _movementHighlight.GetComponent<SpriteRenderer>().color = _col;
            m_lHighlightedSquares.Add(_movementHighlight);
        }
    }

    public void EndFactionTurn()
    {
        m_bIsAllyTurn += 1;
        if (m_bIsAllyTurn > Turn_Order.GuestTurn)
            m_bIsAllyTurn = Turn_Order.AllyTurn;

        //Before starting the next factions turn, throw up the window to say who's turn it is.
        m_nState = (int)War_States.eTurnOrderWindow;
        m_goTurnOrderWindow.SetActive(true);
        string _szMessage = "";
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
            _szMessage = "Ally\nTurn";
        else if (m_bIsAllyTurn == Turn_Order.EnemyTurn)
            _szMessage = "Enemy\nTurn";
        else if (m_bIsAllyTurn == Turn_Order.GuestTurn)
            _szMessage = "Guest\nTurn";
        m_goTurnOrderWindow.GetComponentInChildren<Text>().text = _szMessage;
    }

	private void ResetClearList(List<int> _lList, List<GameObject> _units)
	{
		foreach (int n in _lList)
		{
			_units.RemoveAt (n);
		}
		_lList.Clear ();
	}
    void StartFactionTurn()
    {
		List<int> _lnIterOfObjectsToRemove = new List<int> ();
		int c = 0;
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
        {
            m_bAllowInput = true;
            m_nState = (int)War_States.eMovement;

            foreach (GameObject _go in m_lAllies)
			{
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = false;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", false);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lAllies);
			c = 0;
            foreach (GameObject _go in m_lEnemies)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lEnemies);
			c = 0;
            foreach (GameObject _go in m_lGuests)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lGuests);
			c = 0;
        }
        else if (m_bIsAllyTurn == Turn_Order.EnemyTurn)
        {
            foreach (GameObject _go in m_lAllies)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lAllies);
			c = 0;
            foreach (GameObject _go in m_lEnemies)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = false;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", false);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lEnemies);
			c = 0;
            foreach (GameObject _go in m_lGuests)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lGuests);
			c = 0;

            List<GameObject> _targets = new List<GameObject>();
            _targets.AddRange(m_lAllies);
            _targets.AddRange(m_lGuests);
            GetComponent<WarBattle_EnemyControllerScript>().StartFactionTurn(m_lEnemies, null, _targets);
        }
        else if (m_bIsAllyTurn == Turn_Order.GuestTurn)
        {
            foreach (GameObject _go in m_lAllies)
            {
				if (_go == null)
				{
					_lnIterOfObjectsToRemove.Add (c);
				}
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lAllies);
			c = 0;
            foreach (GameObject _go in m_lEnemies)
            {
				if (_go == null)
					_lnIterOfObjectsToRemove.Add (c);
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = true;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", true);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lEnemies);
			c = 0;
            foreach (GameObject _go in m_lGuests)
            {
				if (_go == null)
					_lnIterOfObjectsToRemove.Add (c);
				else
				{
					_go.GetComponent<TRPG_UnitScript> ().m_bHasActedThisTurn = false;
					_go.GetComponentInChildren<Animator> ().SetBool ("m_bHasActed", false);
				}
				c++;
            }
			ResetClearList (_lnIterOfObjectsToRemove, m_lGuests);
			c = 0;
            GetComponent<WarBattle_EnemyControllerScript>().StartFactionTurn(m_lGuests, m_lAllies, m_lEnemies);
        }
    }

    public void EndUnitTurn(GameObject p_unit)
    {
        p_unit.GetComponentInChildren<Animator>().SetBool("m_bHasActed", true);
        p_unit.GetComponent<TRPG_UnitScript>().EndTurn();
        // Let's check if anything died...
        for (int i = m_lAllies.Count - 1; i >= 0; i--)
        {
			if (m_lAllies [i] == null)
			{
				m_lAllies.RemoveAt (i);
				continue;
			}
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
			if (m_lEnemies [i] == null)
			{
				m_lEnemies.RemoveAt (i);
				continue;
			}
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
			if (m_lGuests [i] == null)
			{
				m_lGuests.RemoveAt (i);
				continue;
			}
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
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
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
        else if (m_bIsAllyTurn == Turn_Order.EnemyTurn)
        {
            //check to see if all of the units on this team have acted, if they have, make sure to end their factions turn
            foreach (GameObject _go in m_lEnemies)
            {
                if (_go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn == false)
                {
                    GetComponent<WarBattle_EnemyControllerScript>().UnitActionEnded();
                    return;
                }
            }
            //if we're here this means that every unit in this party has acted, time to end this factions turn
            EndFactionTurn();
        }
        else if (m_bIsAllyTurn == Turn_Order.GuestTurn)
        {
            //check to see if all of the units on this team have acted, if they have, make sure to end their factions turn
            foreach (GameObject _go in m_lGuests)
            {
                if (_go.GetComponent<TRPG_UnitScript>().m_bHasActedThisTurn == false)
                {
                    GetComponent<WarBattle_EnemyControllerScript>().UnitActionEnded();
                    return;
                }
            }
            //if we're here this means that every unit in this party has acted, time to end this factions turn
            EndFactionTurn();
        }
    }

    public void BattleSceneEnded(FightSceneControllerScript.cWarUnit _defender, FightSceneControllerScript.cWarUnit _attacker)
    {
        if (m_bIsAllyTurn == Turn_Order.AllyTurn)
            EndUnitTurn(m_goSelectedUnit);
        else
            EndUnitTurn(GetComponent<WarBattle_EnemyControllerScript>().m_goCurrentUnitActing);
    }

    void LoadInMap()
    {
        m_goMapData = Instantiate(Resources.Load<GameObject>("WarBattleData/" + dc.m_szWarBattleDataPath));
        GameObject _goCatchHelperObjectToDestroy = null;
        GameObject _goCatchAllyContainerToDestroy = null;
        foreach (Transform child in m_goMapData.transform)
        {
            if (child.name == "Designer_Helper")
            {
                _goCatchHelperObjectToDestroy = child.gameObject;
                continue;
            }
            if (child.name == "TerrainCosts")
                continue;
            GameObject _unit;
            //Is this an ally start position? (Allies are placed in specified positions determined by an order determined out of battle by the player, so this is handled differently than enemies/guests
            if (child.tag == "Ally")
            {
                _goCatchAllyContainerToDestroy = child.gameObject;
                List<FightSceneControllerScript.cWarUnit> _lAllyUnits = dc.GetWarUnits();

                if (_lAllyUnits.Count <= 0)
                {
                    //This is a debug battle, put in some debug ally units n'stuff(?)
                    Transform _debugAllyContainer = _goCatchHelperObjectToDestroy.transform.Find("DebugAllies");
                    Transform[] _debugAllies = _debugAllyContainer.GetComponentsInChildren<Transform>();
                    //set this to 1 instead of zero because the container above will contain itself, so skip over it.
                    int _nIter = 1;
                    foreach (Transform _allyChild in child.transform)
                    {
                        if (_nIter >= _debugAllies.Length)
                        {
                            //We've reached the last of the units, break out yo.
                            break;
                        }
                        _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + _debugAllies[_nIter].name));
                        _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.Initialize();
                        _unit.transform.position = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_allyChild.position).worldPosition;
                        _unit.GetComponent<TRPG_UnitScript>().m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_unit.transform.position);
                        _unit.tag = "Ally";
                        m_lAllies.Add(_unit);
                        ++_nIter;
                    }
                }
                else
                {
                    //This is a for realsies battle, load up each unit into it's numerical location.
                    int _nIter = 0;
                    foreach (Transform _allyChild in child.transform)
                    {
                        if (_nIter >= _lAllyUnits.Count)
                        {
                            //We've reached the last of the units, break out yo.
                            break;
                        }
                        if(_lAllyUnits[_nIter].m_szLeaderName != "")
                            _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + _lAllyUnits[_nIter].m_szTeamName));
                        else
                            _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + _lAllyUnits[_nIter].m_szTRPGDataPath));
                        _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData = _lAllyUnits[_nIter];
                        _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.Initialize();
                        _unit.transform.position = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_allyChild.position).worldPosition;
                        _unit.GetComponent<TRPG_UnitScript>().m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_unit.transform.position);
                        _unit.tag = "Ally";
                        m_lAllies.Add(_unit);
                        ++_nIter;
                    }
                }

                continue;
            }
            _unit = Instantiate(Resources.Load<GameObject>("Units/WarUnits/" + child.name));
            _unit.transform.position = child.position;
            _unit.GetComponent<TRPG_UnitScript>().m_wuUnitData.Initialize();
            _unit.GetComponent<TRPG_UnitScript>().m_cPositionOnGrid = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_unit.transform.position);
            //Is this an enemy?
            if (child.tag == "Enemy")
            {
                _unit.tag = "Enemy";
                _unit.transform.position = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(child.position).worldPosition;
                Vector3 _localScale = _unit.GetComponentInChildren<Animator>().transform.localScale;
                _localScale.x *= -1;
                _unit.GetComponentInChildren<Animator>().transform.localScale = _localScale;

                m_lEnemies.Add(_unit);
            }
            //Is this an enemy?
            else if (child.tag == "Guest")
            {
                _unit.tag = "Guest";
                _unit.transform.position = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(child.position).worldPosition;
                m_lGuests.Add(_unit);
            }
            Destroy(child.gameObject);
        }
        if (_goCatchHelperObjectToDestroy != null)
            Destroy(_goCatchHelperObjectToDestroy);
        if (_goCatchAllyContainerToDestroy != null)
            Destroy(_goCatchAllyContainerToDestroy);
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

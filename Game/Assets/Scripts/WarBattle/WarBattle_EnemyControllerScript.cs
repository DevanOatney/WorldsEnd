using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class WarBattle_EnemyControllerScript : MonoBehaviour
{
    public enum WB_AI_States { eInnactive, eStart, eCursorToMoveLocation, eMoveToLocation, eWaitToArrive, eSelectAction, eCursorToAttack, eCursorToMagic, eWaitForActionResolution }
    public enum WB_AI_Temper { eNormal, eCoward, eRunToLocation, eFocusFireTarget }
    //Hooks to things
    GameObject m_goWatcher;
    GameObject m_goActionWindow;

    List<GameObject> m_lSameTeam;
    List<GameObject> m_lEnemyTeam;
    //This should only have things in it if this faction is an NPC
    List<GameObject> m_lAlliesOfTeam;

    public WB_AI_States m_eState = WB_AI_States.eInnactive;
    WB_AI_Temper m_eTemper = WB_AI_Temper.eNormal;
    CNode m_cDesiredDestination = null;
    cTargetWeights m_goDesiredTarget = null;
    [HideInInspector]
    public GameObject m_goCurrentUnitActing = null;

    //The response floats are for slowing down the steps of the AI so that the player can see step by step what each unit is doing.
    float m_fResponseTimer = 0.0f;
    float m_fResponseBucket = 0.5f;

    class cTargetWeights
    {
        public int _nDistanceToUnit = 0;
        //Attack+Defense
        public int _nStatDifference = 0;
        public GameObject _goTarget = null;
        public Vector3[] _vPrefferedPath = null;
    }

    //Which unit in the list who's action we're on
    int m_nUnitIter = 0;
    
	// Use this for initialization
	void Start ()
    {
	
	}

    public void Initialize(GameObject _watcher, GameObject _actionWindow)
    {
        m_goWatcher = _watcher;
        m_goActionWindow = _actionWindow;
        m_eState = WB_AI_States.eInnactive;
    }

    /// <summary>
    /// Called once a round when a faction is about to start their turn.
    /// </summary>
    /// <param name="_sameTeam">  Units that are on the same team and will all act in this calculation</param>
    /// <param name="_allies">    Units that this team will not attack</param>
    /// <param name="_enemies">  Units that this team will attack/run from, depending on temperament.</param>
    /// <param name="_eTemperamant"> Personallity(WarBattle_EnemyControllerScript.WB_AI_TEMPER.),  eNormal - attack closest/weakest unit, eCoward - run as far away from enemies as possible.</param>
    public void StartFactionTurn(List<GameObject> _sameTeam, List<GameObject> _allies, List<GameObject> _enemies, WB_AI_Temper _eTemperamant = WB_AI_Temper.eNormal)
    {
        m_eState = WB_AI_States.eStart;
        m_eTemper = _eTemperamant;
        m_nUnitIter = 0;
        m_lSameTeam = _sameTeam;
        m_lEnemyTeam = _enemies;
        if (_allies != null)
            m_lAlliesOfTeam = _allies;
        else
            m_lAlliesOfTeam = new List<GameObject>();
    }


    public void UnitMovementFinished()
    {
        m_eState = WB_AI_States.eSelectAction;
    }

    //Called from the watcher when the battle scene animation ends
    public void UnitActionEnded()
    {
        m_nUnitIter += 1;
        if (m_nUnitIter >= m_lSameTeam.Count)
            m_eState = WB_AI_States.eInnactive;
        else
        {
            m_eState = WB_AI_States.eStart;
        }
    }

    public void RemoveUnit(GameObject _unit)
    {
        for (int i = m_lAlliesOfTeam.Count - 1; i >= 0; i--)
        {
            if (_unit == m_lAlliesOfTeam[i])
            {
                m_lAlliesOfTeam.RemoveAt(i);
                return;
            }
        }
        for (int i = m_lEnemyTeam.Count - 1; i >= 0; i--)
        {
            if (_unit == m_lEnemyTeam[i])
            {
                m_lEnemyTeam.RemoveAt(i);
                return;
            }
        }
        for (int i = m_lSameTeam.Count - 1; i >= 0; i--)
        {
            if (_unit == m_lSameTeam[i])
            {
                m_lSameTeam.RemoveAt(i);
                return;
            }
        }
    }

	// Update is called once per frame
	void Update ()
    {
        HandleState();
	}

    void HandleState()
    {
        switch (m_eState)
        {
            case WB_AI_States.eStart:
                {
                    //This is the start of the AI's turn. Need to iterate through each unit, giving them a desired location to move, and a desired action (that is valid)
                    
                    m_goCurrentUnitActing = m_lSameTeam[m_nUnitIter];
                    m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_bIsMyTurn = true;
                    m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goSelector.transform.position = m_goCurrentUnitActing.transform.position;
                    CalculateAction();
                    if (m_cDesiredDestination != null)
                    {
                        m_goWatcher.GetComponent<WarBattleWatcherScript>().ShowHighlightedSquares(m_goCurrentUnitActing, m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange, Color.yellow);
                        m_eState = WB_AI_States.eCursorToMoveLocation;
                    }
                }
                break;
            case WB_AI_States.eCursorToMoveLocation:
                {
                    m_fResponseTimer += Time.deltaTime;
                    if (m_fResponseTimer >= m_fResponseBucket)
                    {

                        CNode _selPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goSelector.transform.position);
                        if (_selPos.gridX < m_cDesiredDestination.gridX)
                        {
                            //Move Right
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(2);
                        }
                        else if (_selPos.gridX > m_cDesiredDestination.gridX)
                        {
                            //Move Left
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(1);
                        }
                        else if (_selPos.gridY < m_cDesiredDestination.gridY)
                        {
                            //Move Up
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(3);
                        }
                        else if (_selPos.gridY > m_cDesiredDestination.gridY)
                        {
                            //Move Down
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(0);
                        }
                        else
                        {
                            //Cursor has reached it's target.
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().ClearHighlightedSquares();
                            m_eState = WB_AI_States.eMoveToLocation;
                        }
                        m_fResponseTimer = 0.0f;
                    }
                }
                break;
            case WB_AI_States.eMoveToLocation:
                {
                    m_eState = WB_AI_States.eWaitToArrive;
                    m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().MoveToLocation(m_cDesiredDestination.worldPosition);
                }
                break;
            case WB_AI_States.eSelectAction:
                {
                    m_fResponseTimer += Time.deltaTime;
                    if (m_fResponseTimer >= m_fResponseBucket)
                    {
                        //Currently there isn't any magic, so we're never going to pick that one.
                        int _cursorDestination = 0;
                        if (m_goDesiredTarget == null)
                            _cursorDestination = 2;
                        int _currentCursorLoc = m_goActionWindow.GetComponent<ActionWindowScript>().m_nChoiceIter;
                        if (_currentCursorLoc < _cursorDestination)
                            m_goActionWindow.GetComponent<ActionWindowScript>().MoveDown();
                        else if (_currentCursorLoc > _cursorDestination)
                            m_goActionWindow.GetComponent<ActionWindowScript>().MoveUp();
                        else
                        {
                            if (_cursorDestination == 0)
                                m_eState = WB_AI_States.eCursorToAttack;
                            else if (_cursorDestination == 1)
                                m_eState = WB_AI_States.eCursorToMagic;
                            else if (_cursorDestination == 2)
                                m_eState = WB_AI_States.eWaitForActionResolution;
                            m_goActionWindow.GetComponent<ActionWindowScript>().Confirm();
                        }
                        m_fResponseTimer = 0.0f;
                        m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goSelector.transform.position = m_goCurrentUnitActing.transform.position;
                    }
                }
                break;
            case WB_AI_States.eCursorToAttack:
                {
                    m_fResponseTimer += Time.deltaTime;
                    if (m_fResponseTimer >= m_fResponseBucket)
                    {
                        CNode _selPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goSelector.transform.position);
                        CNode _tgtPos = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goDesiredTarget._goTarget.transform.position);
                        if (_selPos.gridX < _tgtPos.gridX)
                        {
                            //Move Right
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(2);
                        }
                        else if (_selPos.gridX > _tgtPos.gridX)
                        {
                            //Move Left
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(1);
                        }
                        else if (_selPos.gridY < _tgtPos.gridY)
                        {
                            //Move Up
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(3);
                        }
                        else if (_selPos.gridY > _tgtPos.gridY)
                        {
                            //Move Down
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().MoveCursor(0);
                        }
                        else
                        {
                            //Cursor has reached it's target.
                            m_eState = WB_AI_States.eWaitForActionResolution;
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().ClearHighlightedSquares();
                            m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goBattleScreen.SetActive(true);
                            CNode _startNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goCurrentUnitActing.transform.position);
                            
                            int _distanceToAttack = (int)Mathf.Sqrt(Mathf.Pow((_startNode.gridX - _tgtPos.gridX), 2) + Mathf.Pow((_startNode.gridY - _tgtPos.gridY), 2));
                            if(m_goCurrentUnitActing.tag == "Enemy")
                                m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goBattleScreen.GetComponent<FightSceneControllerScript>().SetupBattleScene(m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData, m_goDesiredTarget._goTarget.GetComponent<TRPG_UnitScript>().m_wuUnitData, _distanceToAttack);
                            else
                                m_goWatcher.GetComponent<WarBattleWatcherScript>().m_goBattleScreen.GetComponent<FightSceneControllerScript>().SetupBattleScene(m_goDesiredTarget._goTarget.GetComponent<TRPG_UnitScript>().m_wuUnitData, m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData, _distanceToAttack);

                        }
                        m_fResponseTimer = 0.0f;
                    }
                }
                break;
            case WB_AI_States.eCursorToMagic:
                {
                }
                break;
        }
    }

    void CalculateAction()
    {
        //Some early declarations that will be needed for anything
        CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goCurrentUnitActing.transform.position);
        FightSceneControllerScript.cWarUnit _thisUnitData = m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData;
        List<CNode> _lSameTeamNodes = new List<CNode>();
        foreach (GameObject _go in m_lSameTeam)
            _lSameTeamNodes.Add(CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_go.transform.position));
        List<CNode> _lAllyTeamNodes = new List<CNode>();
        foreach (GameObject _go in m_lAlliesOfTeam)
            _lAllyTeamNodes.Add(CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_go.transform.position));
        List<CNode> _lOtherTeamNodes = new List<CNode>();
        foreach (GameObject _go in m_lEnemyTeam)
            _lOtherTeamNodes.Add(CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_go.transform.position));

        //first, let's do an early exit check, if this unit is completely surrounded and unable to move, see if there's a unit in range to attack, if not, don't try to move, don't try to act, just end your turn
        List<CNode> _neighbors = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(_unitNode.worldPosition, 1);
        //List<CNode> _withinAttackRange = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(_unitNode.worldPosition, _thisUnitData.m_nAttackRange);
        bool _bCanIMove = false;
        //Just incase we have to index into one of these lists.
        int _nIterCounter = 0;
        foreach (CNode _node in _neighbors)
        {
            _nIterCounter = 0;
            //true if we found a unit in this node.
            bool _bMatchFound = false;
            foreach (CNode _checkNode in _lSameTeamNodes)
            {
                //check if this unit, on the same team, is in this node.
                if (_checkNode == _node)
                {
                    //Yep, can't move here.
                    _bMatchFound = true;
                    break;
                }
            }
            foreach (CNode _checkNode in _lAllyTeamNodes)
            {
                //check if this unit is from an allied team
                if (_checkNode == _node)
                {
                    //Yep, can't move here either.
                    _bMatchFound = true;
                    break;
                }
            }
            foreach (CNode _checkNode in _lOtherTeamNodes)
            {
                //Have we already found a unit in this spot?
                if (_bMatchFound == true)
                {
                    //Yep, nevermind.. get out of this loop
                    break;
                }
                //Let's see if an opposing team unit is in this node
                if (_checkNode == _node)
                {
                    //There is a unit from the other team in this node.
                    cTargetWeights _newTarget = new cTargetWeights();
                    _newTarget._goTarget = m_lEnemyTeam[_nIterCounter];
                    m_goDesiredTarget = CheckNewTarget(_newTarget);
                   
                }
                _nIterCounter += 1;
            }

            if (_bMatchFound == false)
            {
                //So, at this point we've check all of the units and didn't find anyone in this node.   Check to see if this is a walkable node, if it isn't, we obviously can't walk here.
                if (_node.walkable == false)
                    continue;
            }

            //if we got to here, there is a node that this unit can walk on.
            _bCanIMove = true;
        }


        //If there wasn't something to attack, check if we can even move, if we can't move, set the desired location to where the unit is, and call it good.
        if (_bCanIMove == false)
        {
            //For now at least, if there is a unit that was in range of this initial attack, go for it.
            if (m_goDesiredTarget != null)
            {
                m_cDesiredDestination = _unitNode;
                return;
            }
            m_goDesiredTarget = null;
            m_cDesiredDestination = _unitNode;
            return;
        }
        else
        {
            //reset the desired target as it will be accessed later.
            m_goDesiredTarget = null;
        }
        //So, at this point, there isn't a unit right next to us, but we can move.. grab every node that is in range of this units influence (It's movement range + it's attack range)
        List<CNode> _lNodesInInfluence = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(_unitNode.worldPosition, _thisUnitData.m_nMovementRange + _thisUnitData.m_nAttackRange);
        List<GameObject> _lTargetsInRange = new List<GameObject>();

        foreach (CNode _node in _lNodesInInfluence)
        {
            int _nTargetIter = 0;
            foreach (CNode _targetNode in _lOtherTeamNodes)
            {
                if (_node == _targetNode)
                {
                    _lTargetsInRange.Add(m_lEnemyTeam[_nTargetIter]);
                }
                _nTargetIter += 1;
            }
        }


        List<CNode> _movementRangeNodes = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(_unitNode.worldPosition, _thisUnitData.m_nMovementRange);
        //Add in it's own square so that it can choose not to move at all
        _movementRangeNodes.Add(_unitNode);
        if (_lTargetsInRange.Count > 0)
        {
            //We were able to find a unit to attack within range of our current unit. Let's see if there is an available spot to move in range of attacking that unit (at max range of our unit), that is within our movement range.           
            List<cTargetWeights> _lValidTargets = new List<cTargetWeights>();
            //Loop through and add to the valid targets list all of the targets that you could move to in one turn and attack.
            foreach (GameObject _target in _lTargetsInRange)
            {
                cTargetWeights _newTarget = CalculateBestPath(_target, _movementRangeNodes);
                if(_newTarget != null)
                   _lValidTargets.Add(_newTarget);
            }
            foreach (cTargetWeights _target in _lValidTargets)
            {
                m_goDesiredTarget = CheckNewTarget(_target);
            }
        }
        if (m_goDesiredTarget != null)
        {
            //we found our most desired target, that is also in range! :D 
            
            m_cDesiredDestination = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goDesiredTarget._vPrefferedPath[m_goDesiredTarget._vPrefferedPath.Length - 1]);
            return;
        }

        //All right, last but not least, if we're here this means that we can move, but there isn't a single attackable target within our range.
        List<cTargetWeights> _lTargets = new List<cTargetWeights>();
        foreach (GameObject _enemy in m_lEnemyTeam)
        {
            cTargetWeights _newTarget = CalculatePathsToTarget_OutOfRange(_enemy);
            if (_newTarget != null)
                _lTargets.Add(_newTarget);
        }
        foreach (cTargetWeights _target in _lTargets)
            m_goDesiredTarget = CheckNewTarget(_target);
        if (m_goDesiredTarget != null)
        {
            //We now need to trim down the path, incase it is too long (as at this point we're not caring about movement range)
            List<Vector3> _vaTrimmedPath = new List<Vector3>();
            for (int i = 0; i < m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange; ++i)
            {
                if (m_goDesiredTarget._vPrefferedPath.Length <= i)
                {
                    //We've reached the end of the desired path, before running out of movement space... so this really shouldn't happen, but I'm putting it here just incase
                    break;
                }
                if (m_goDesiredTarget._vPrefferedPath[i] != null)
                {
                    _vaTrimmedPath.Add(m_goDesiredTarget._vPrefferedPath[i]);
                    if (i + 1 >= m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_nMovementRange)
                    {
                        //we're at the end of the movement range, make this trimmed thing the path and call it good.
                        m_goDesiredTarget = null;
                        m_cDesiredDestination = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_vaTrimmedPath[_vaTrimmedPath.Count - 1]);
                        return;
                    }
                }
            }

            //This should be the catch all, this will return the most optimal unit to move to, even outside of the units movement range.
            m_cDesiredDestination = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goDesiredTarget._vPrefferedPath[m_goDesiredTarget._vPrefferedPath.Length - 1]);
        }
        else
        {
            //I... don't know what to say if we get here, I'm thinking if we ever land in here it was either an error, or there are no units this unit can find... 
            Debug.Log("No Action Calculated...");
        }
    }

    //So this is used at the end if the current unit acting can't do anything else, at this point, just return the shortest, valid, path to this unit.
    cTargetWeights CalculatePathsToTarget_OutOfRange(GameObject p_unit)
    {
        FightSceneControllerScript.cWarUnit _thisUnitData = m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData;
        CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goCurrentUnitActing.transform.position);
        List<CNode> _targetNeighborNodes = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(p_unit.transform.position, _thisUnitData.m_nAttackRange);
        List<Vector3[]> _lPaths = new List<Vector3[]>();
        foreach (CNode _targetsNode in _targetNeighborNodes)
        {
            if (_targetsNode.walkable == true)
            {
                //There is at least one valid location around this target.
                _lPaths.Add(CPathRequestManager.m_Instance.m_psPathfinding.FindPathImmediate(_unitNode.worldPosition, _targetsNode.worldPosition));
            }
        }
        //so now that we've found all of the valid paths to this target find the path with the lowest movement cost and set that as this units preferred path.
        int _closestValidDistance = int.MaxValue;
        Vector3[] _vBestPath = null;
        for (int x = _lPaths.Count - 1; x >= 0; --x)
        {
            int _movementCost = 0;
            for (int i = 0; i < _lPaths[x].Length; ++i)
            {
                CNode _pathNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_lPaths[x][i]);
                _movementCost += 1 + _pathNode.movementPenalty;
            }
            //So by here we have the total cost it would take to move to this position.  Check to see if it's less than any of the other paths.
            if (_movementCost <= _closestValidDistance)
            {
                //We have a new best!
                _vBestPath = _lPaths[x];
                _closestValidDistance = _movementCost;
            }
        }
        if (_vBestPath != null)
        {
            cTargetWeights _newTarget = new cTargetWeights();
            _newTarget._goTarget = p_unit;
            _newTarget._vPrefferedPath = _vBestPath;
            return _newTarget;
        }
        return null;
    }

    cTargetWeights CalculateBestPath(GameObject p_unit, List<CNode> _movementRangeNodes)
    {
        FightSceneControllerScript.cWarUnit _thisUnitData = m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData;
        CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goCurrentUnitActing.transform.position);
        List<CNode> _targetNeighborNodes = CPathRequestManager.m_Instance.m_psPathfinding.grid.GetNeighbours(p_unit.transform.position, _thisUnitData.m_nAttackRange);

        List<Vector3[]> _lPaths = new List<Vector3[]>();
        foreach (CNode _targetsNode in _targetNeighborNodes)
        {
            foreach (CNode _ourNode in _movementRangeNodes)
            {
                if (_targetsNode == _ourNode)
                {
                    //There is at least one valid location around this target.
                    _lPaths.Add(CPathRequestManager.m_Instance.m_psPathfinding.FindPathImmediate(_unitNode.worldPosition, _ourNode.worldPosition));
                    break;
                }
            }
        }

        //so now that we've found all of the valid paths to this target find the path with the lowest movement cost and set that as this units preferred path.
        int _closestValidDistance = int.MinValue;
        Vector3[] _vBestPath = null;
        for(int x = _lPaths.Count - 1; x >= 0; --x)
        {
            int _movementCost = 0;
            for (int i = 0; i < _lPaths[x].Length; ++i)
            {
                CNode _pathNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_lPaths[x][i]);
                _movementCost += 1 + _pathNode.movementPenalty;
                if (_thisUnitData.m_nMovementRange < _movementCost)
                {
                    _lPaths.RemoveAt(x);
                    break;
                }
            }
            if (_thisUnitData.m_nMovementRange >= _movementCost)
            {
                CNode _targetNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(p_unit.transform.position);
                int _distFromTarget = (int)Mathf.Sqrt(Mathf.Pow((_lPaths[x][_lPaths[x].Length -1].x - _targetNode.worldPosition.x), 2) + Mathf.Pow((_lPaths[x][_lPaths[x].Length - 1].y - _targetNode.worldPosition.y), 2));
                //check the distance, with a priority toward spots that are the furthest away but still in range;
                if (_distFromTarget <= _thisUnitData.m_nAttackRange && _distFromTarget >= _closestValidDistance)
                {
                    //if this is our first path, immediately make it the "best path" so far to avoid checking a null variable.
                    if (_vBestPath == null)
                    {
                        _closestValidDistance = _distFromTarget;
                        _vBestPath = _lPaths[x];
                    }
                    else if (_distFromTarget == _closestValidDistance)
                    { 
                        //So, this is at least on par so far with the best path we've found, now check which of the paths is the shorest, and that will be our "Best path"
                        if (_lPaths[x].Length < _vBestPath.Length)
                        {
                            //we've got a new winner, so far.
                            _closestValidDistance = _distFromTarget;
                            _vBestPath = _lPaths[x];
                        }
                    }
                    else
                    {
                        //If we're in here, this means that we've found a new optimal range, so make this the new baseline.
                        _closestValidDistance = _distFromTarget;
                        _vBestPath = _lPaths[x];
                    }
                }
            }
        }
        //First, just make sure one of the paths was at least within the movement range.
        if (_vBestPath != null)
        {
            //So having calculated which is the best path, time to check if this target is a better target than the last target (if there was one)
            cTargetWeights _newTarget = new cTargetWeights();
            _newTarget._goTarget = p_unit;
            _newTarget._vPrefferedPath = _vBestPath;
            return _newTarget;
        }
        return null;
    }

    //So this should be the meat an potatoes of checking which is the most optimal target to attack (distance, difference in attack/def, health of unit)
    cTargetWeights CheckNewTarget(cTargetWeights _newTarget)
    {
        FightSceneControllerScript.cWarUnit _unitStats = m_goCurrentUnitActing.GetComponent<TRPG_UnitScript>().m_wuUnitData;
        int _unitStatScore = _unitStats.m_nAttackPower + _unitStats.m_nDefensePower;
        FightSceneControllerScript.cWarUnit _targetStats = _newTarget._goTarget.GetComponent<TRPG_UnitScript>().m_wuUnitData;
        int _targetStatScore = _targetStats.m_nAttackPower + _targetStats.m_nDefensePower;
        _newTarget._nStatDifference = _unitStatScore - _targetStatScore;

        //Check if this unit even has to move on a path
        if (_newTarget._vPrefferedPath != null && _newTarget._vPrefferedPath.Length > 0)
        {
            CNode _unitNode = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(m_goCurrentUnitActing.transform.position);
            CNode _destination = CPathRequestManager.m_Instance.m_psPathfinding.grid.NodeFromWorldPoint(_newTarget._vPrefferedPath[_newTarget._vPrefferedPath.Length - 1]);
            Vector2 _strtDest = new Vector2(_unitNode.gridX, _unitNode.gridY);
            Vector2 _endDest = new Vector2(_destination.gridX, _destination.gridY);
            _newTarget._nDistanceToUnit = (int)Mathf.Sqrt(Mathf.Pow((_endDest.x - _strtDest.x), 2) + Mathf.Pow((_endDest.y - _strtDest.y), 2));
        }
        else
        {
            _newTarget._nDistanceToUnit = 0;
        }
        //So at this point we've created all of the needed data for the new target, check if there was a previous target.. if not, just return this new target as the current preferred target.
        if (m_goDesiredTarget == null)
            return _newTarget;

        //So we're going to add up the scores and see which one is higher.  Perhaps later we can add in multiplier weights to each to put a preference on one over the other on a character by character basis.
        float _oldScore = 0;
        _oldScore += m_goDesiredTarget._nStatDifference;
        _oldScore += m_goDesiredTarget._goTarget.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_fPercentRemaining;
        _oldScore -= m_goDesiredTarget._nDistanceToUnit;

        float _newScore = 0;
        _newScore += _newTarget._nStatDifference;
        _newScore += _newTarget._goTarget.GetComponent<TRPG_UnitScript>().m_wuUnitData.m_fPercentRemaining;
        _newScore -= _newTarget._nDistanceToUnit;

        if (_newScore > _oldScore)
            return _newTarget;
        else
            return m_goDesiredTarget;
    }
}

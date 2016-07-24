using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathfindingScript : MonoBehaviour 
{
	
	public CGrid grid;
    CPathRequestManager m_prmRequestManager;
	bool m_bAllowDiagonal = false;
	
	void Awake() 
	{
		grid = GetComponent<CGrid>();
        m_prmRequestManager = GetComponent<CPathRequestManager>();
	}

    public void StartFindPath(Vector3 p_PathStart, Vector3 p_PathEnd)
    {
        StartCoroutine(FindPath(p_PathStart, p_PathEnd, m_bAllowDiagonal));
    }

    public Vector3[] FindPathImmediate(Vector3 startPos, Vector3 targetPos, bool bAllowDiagonal = false)
    {
        CNode startNode = grid.NodeFromWorldPoint(startPos);
        CNode targetNode = grid.NodeFromWorldPoint(targetPos);

        //Early exit, if the start node and the target node are the same, just return a "path" that is just that single node.
        if (startNode == targetNode)
        {
            List<Vector3> _lQuickPath = new List<Vector3>();
            _lQuickPath.Add(startNode.worldPosition);
            return _lQuickPath.ToArray();

        }

        CHeap<CNode> openSet = new CHeap<CNode>(grid.MapSize);
        HashSet<CNode> closedSet = new HashSet<CNode>();
        openSet.Add(startNode);

        Vector3[] _vWaypoints = new Vector3[0];
        bool _bPathSuccess = false;
        if (targetNode.walkable)
        {
            while (openSet.Count > 0)
            {
                CNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);



                if (currentNode == targetNode)
                {
                    _bPathSuccess = true;
                    break;
                }

                foreach (CNode neighbour in grid.GetNeighbours(currentNode, bAllowDiagonal))
                {
                    if ((!neighbour.walkable || closedSet.Contains(neighbour)) && neighbour != startNode)
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
        }
        if (_bPathSuccess == true)
        {
            _vWaypoints = RetracePath(startNode, targetNode);
            return _vWaypoints;
        }
        return null;
    }

	public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, bool bAllowDiagonal) 
	{
		CNode startNode = grid.NodeFromWorldPoint(startPos);
		CNode targetNode = grid.NodeFromWorldPoint(targetPos);

        //Early exit, if the start node and the target node are the same, just return a "path" that is just that single node.
        if (startNode == targetNode)
        {
            List<Vector3> _lQuickPath = new List<Vector3>();
            _lQuickPath.Add(startNode.worldPosition);
            m_prmRequestManager.FinishedProcessingPath(_lQuickPath.ToArray(), true);
        }

        CHeap<CNode> openSet = new CHeap<CNode>(grid.MapSize);
		HashSet<CNode> closedSet = new HashSet<CNode>();
		openSet.Add(startNode);

		Vector3[] _vWaypoints = new Vector3[0];
        bool _bPathSuccess = false;
        if ( targetNode.walkable)
        {
            while (openSet.Count > 0)
            {
                CNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);



                if (currentNode == targetNode)
                {
                    _bPathSuccess = true;
                    break;
                }

                foreach (CNode neighbour in grid.GetNeighbours(currentNode, bAllowDiagonal))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                            openSet.UpdateItem(neighbour);
                    }
                }
            }
            yield return null;
        }
        if (_bPathSuccess == true)
        {
            _vWaypoints = RetracePath(startNode, targetNode);
        }
        m_prmRequestManager.FinishedProcessingPath(_vWaypoints, _bPathSuccess);
	}
	
	Vector3[] RetracePath(CNode startNode, CNode endNode) 
	{
		List<CNode> path = new List<CNode>();
		CNode currentNode = endNode;
		
		while (currentNode != startNode) 
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
        Vector3[] _vWaypoints = SimplifyPath(path);
        Array.Reverse(_vWaypoints);
        return _vWaypoints;
		
	}

    Vector3[] SimplifyPath(List<CNode> p_lPath)
    {
        List<Vector3> _lWaypoints = new List<Vector3>();
        //Vector2 _vOldDir = Vector2.zero;
        for (int i = 0; i < p_lPath.Count; ++i)
        {
           // Vector2 _vNewDir = new Vector2(p_lPath[i].gridX - p_lPath[i - 1].gridX, p_lPath[i].gridY - p_lPath[i - 1].gridY);
            //if (_vOldDir != _vNewDir)
            {
                _lWaypoints.Add(p_lPath[i].worldPosition);
            }
           // _vOldDir = _vNewDir;
        }
        return _lWaypoints.ToArray();
    }
	
	int GetDistance(CNode nodeA, CNode nodeB) 
	{
		int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if (dstX > dstY)
			return 14*dstY + 10* (dstX-dstY);
		return 14*dstX + 10 * (dstY-dstX);
	}
}




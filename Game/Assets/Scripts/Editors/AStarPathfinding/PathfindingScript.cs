using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathfindingScript : MonoBehaviour {
	
	public Transform seeker, target;
	
	CGrid grid;

	bool m_bAllowDiagonal = false;
	
	void Awake() 
	{
		grid = GetComponent<CGrid>();
	}
	
	void Update() 
	{
		if(Input.GetKeyDown(KeyCode.A))
		{
			m_bAllowDiagonal = !m_bAllowDiagonal;
			Debug.Log (m_bAllowDiagonal);
		}
		FindPath(seeker.position,target.position, m_bAllowDiagonal);
	}
	
	void FindPath(Vector3 startPos, Vector3 targetPos, bool bAllowDiagonal) 
	{
		CNode startNode = grid.NodeFromWorldPoint(startPos);
		CNode targetNode = grid.NodeFromWorldPoint(targetPos);
		
		List<CNode> openSet = new List<CNode>();
		HashSet<CNode> closedSet = new HashSet<CNode>();
		openSet.Add(startNode);
		
		while (openSet.Count > 0) 
		{
			CNode currentNode = openSet[0];
			for (int i = 1; i < openSet.Count; i ++) 
			{
				if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) 
				{
					currentNode = openSet[i];
				}
			}
			
			openSet.Remove(currentNode);
			closedSet.Add(currentNode);
			
			if (currentNode == targetNode) 
			{
				RetracePath(startNode,targetNode);
				return;
			}
			
			foreach (CNode neighbour in grid.GetNeighbours(currentNode, bAllowDiagonal)) 
			{
				if (!neighbour.walkable || closedSet.Contains(neighbour)) 
				{
					continue;
				}
				
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
				if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
				{
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;
					
					if (!openSet.Contains(neighbour))
						openSet.Add(neighbour);
				}
			}
		}
	}
	
	void RetracePath(CNode startNode, CNode endNode) 
	{
		List<CNode> path = new List<CNode>();
		CNode currentNode = endNode;
		
		while (currentNode != startNode) 
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();
		
		grid.path = path;
		
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




﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGrid : MonoBehaviour 
{
	
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
    public bool m_bShowGrid;
	CNode[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY;


	//optimzation, variables to buffer updating the collision data 
	float m_fUpdateBuffer = 0.25f;
	float m_fUpdateTimer = 0.0f;

    public int MapSize
    {
        get { return gridSizeX * gridSizeY; }
    }

	void Awake() 
	{
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt((gridWorldSize.x)/nodeDiameter);
		gridSizeY = Mathf.RoundToInt((gridWorldSize.y)/nodeDiameter);
		CreateGrid();
	}

	void Update()
	{
		if(m_fUpdateTimer >= m_fUpdateBuffer)
		{
			UpdateGrid();
			m_fUpdateTimer = 0.0f;
		}
		else
			m_fUpdateTimer += Time.deltaTime;
	}

	//initial creation of the grid
	void CreateGrid() 
	{
		grid = new CNode[gridSizeX,gridSizeY];
		UpdateGrid();
	}

	//updating the grids non-walkable area
	void UpdateGrid()
	{
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.up * gridWorldSize.y/2;
		
		for (int x = 0; x < gridSizeX; x ++) 
		{
			for (int y = 0; y < gridSizeY; y ++) 
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + 
					Vector3.up * (y * nodeDiameter + nodeRadius);
				bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius,unwalkableMask));
				//bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius));
				grid[x,y] = new CNode(walkable,worldPoint, x,y);
			}
		}
	}
	
	public List<CNode> GetNeighbours(CNode node, bool bAllowDiagonal) 
	{
		List<CNode> neighbours = new List<CNode>();
		
		for (int x = -1; x <= 1; x++) 
		{
			for (int y = -1; y <= 1; y++) 
			{
				if (x == 0 && y == 0)
					continue;
				if(bAllowDiagonal == false)
				{
					if(Mathf.Abs(x) == Mathf.Abs(y))
						continue;
				}
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				
				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY) 
				{
					neighbours.Add(grid[checkX,checkY]);
				}
			}
		}
		
		return neighbours;
	}
	
	
	public CNode NodeFromWorldPoint(Vector3 worldPosition) 
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.y + gridWorldSize.y/2) / gridWorldSize.y;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		
		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		return grid[x,y];
	}

    //Apparently you can't return null, so if this value returns a -500 in the Z value, it means that the position was invalid.
    public Vector3 WorldPointFromIndex(Vector2 _index)
    {
        if (_index.x >= gridSizeX || _index.y >= gridSizeY)
            return new Vector3(0, 0, -500) ;
        if (_index.x < 0 || _index.y < 0)
            return new Vector3(0, 0, -500);
        return grid[(int)_index.x, (int)_index.y].worldPosition;
    }

    public List<CNode> GetNeighborNodes(Vector3 _worldPos, int _nRange)
    {
        List<CNode> _neighborNodes = new List<CNode>();
        CNode _baseNode = NodeFromWorldPoint(_worldPos);
        if (_baseNode == null)
            return null;
        for (int x = -1*_nRange; x <= _nRange; x++)
        {
            for (int y = -1 * _nRange; y <= _nRange; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                if (Mathf.Abs(x) + Mathf.Abs(y) > _nRange)
                    continue;
                int checkX = _baseNode.gridX + x;
                int checkY = _baseNode.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    if(_neighborNodes.Contains(grid[checkX, checkY]) == false)
                        _neighborNodes.Add(grid[checkX, checkY]);
                }
            }
        }
        return _neighborNodes;
    }

	void OnDrawGizmos() 
	{
		Gizmos.DrawWireCube(transform.position,new Vector3(gridWorldSize.x,gridWorldSize.y,1));
        if (grid != null && m_bShowGrid == true) 
		{
			foreach (CNode n in grid) 
			{
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.color = new Color(Gizmos.color.r, Gizmos.color.g, Gizmos.color.b, 0.35f);
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
			}
		}
	}
}
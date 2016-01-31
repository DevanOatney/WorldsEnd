﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CNode : IHeapItem<CNode>{
	
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;
	
	public int gCost;
	public int hCost;
	public CNode parent;
    int m_nHeapIndex;
	
	public CNode(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) 
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
	
	public int fCost 
	{
		get 
		{
			return gCost + hCost;
		}
	}

    public int HeapIndex
    {
        get { return m_nHeapIndex; }
        set { m_nHeapIndex = value; }
    }

    public int CompareTo(CNode p_NodeToCompareTo)
    {
        int nCompVal = fCost.CompareTo(p_NodeToCompareTo.fCost);
        if (nCompVal == 0)
        {
            nCompVal = hCost.CompareTo(p_NodeToCompareTo.hCost);
        }
        return -nCompVal;
    }

}
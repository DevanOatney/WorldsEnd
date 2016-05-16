using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CPathRequestManager : MonoBehaviour 
{
    Queue<PathRequest> m_qPathRequestQ = new Queue<PathRequest>();
    PathRequest m_CurrentPathRequest;
    public static CPathRequestManager m_Instance;

    public PathfindingScript m_psPathfinding;
    bool m_bIsProcessingPath;

    void Awake()
    {
        m_Instance = this;
        m_psPathfinding = GetComponent<PathfindingScript>();
    }

    public static void RequestPath(Vector3 p_PathStart, Vector3 p_PathEnd, Action<Vector3[], bool> p_Callback)
    {
        PathRequest _newRequest = new PathRequest(p_PathStart, p_PathEnd, p_Callback);
        m_Instance.m_qPathRequestQ.Enqueue(_newRequest);
        m_Instance.TryProcessNextPath();
    }

    public static Vector3[] RequestImmediatePath(Vector3 p_PathStart, Vector3 p_PathEnd, bool bAllowDiagonal = false)
    {
        return m_Instance.m_psPathfinding.FindPathImmediate(p_PathStart, p_PathEnd, bAllowDiagonal);
    }

    void TryProcessNextPath()
    {
        if (!m_bIsProcessingPath && m_qPathRequestQ.Count > 0)
        {
            m_CurrentPathRequest = m_qPathRequestQ.Dequeue();
            m_bIsProcessingPath = true;
            m_psPathfinding.StartFindPath(m_CurrentPathRequest.m_sPathStart, m_CurrentPathRequest.m_sPathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] p_Path, bool success)
    {
        m_CurrentPathRequest.m_sCallback(p_Path, success);
        m_bIsProcessingPath = false;
        TryProcessNextPath();
    }
    
    struct PathRequest
    {
        public Vector3 m_sPathStart;
        public Vector3 m_sPathEnd;
        public Action<Vector3[], bool> m_sCallback;

        public PathRequest(Vector3 p_Start, Vector3 p_End, Action<Vector3[], bool> p_Callback)
        {
            m_sPathStart = p_Start;
            m_sPathEnd = p_End;
            m_sCallback = p_Callback;
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

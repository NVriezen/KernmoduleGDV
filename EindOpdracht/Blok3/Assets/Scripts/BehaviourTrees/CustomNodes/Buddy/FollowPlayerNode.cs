using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerNode : BaseNode
{
    public Transform target;

    public int updateInterval = 1;
    public float walkSpeed = 5;
    public float distanceMargin = 0.5f;

    private List<Vector3> currentPath = new List<Vector3>();
    private List<Vector3> previousPath = new List<Vector3>();

    private state currentState = state.failed;
    private Vector3 previousPlayerPosition = Vector3.zero;

    private AStar AObject;

    private void Awake()
    {
        
        if (FindObjectOfType<AStar>() == null)
        {
            Debug.LogError("No A* object in scene! Please add this component to the scene to use pathfinding.");
        } else
        {
            AObject = FindObjectOfType<AStar>().GetComponent<AStar>();
        }
    }

    private void SetPath()
    {
        List<Vector3> path = AObject.FindPath(transform, target.transform);
        if (path != null)
        {
            if (path != previousPath)
            {
                currentPath = path;
            }
        }
    }

    public override state Tick()
    {
        if (Vector3.Distance(transform.position, target.position) <= AObject.cellSize * 0.95f)
        {
            return currentState = state.succes;
        }

        if (target.position != previousPlayerPosition || currentPath.Count == 0)
        {
            if (currentState != state.running /*|| Time.frameCount % updateInterval == 0*/)
            {
                SetPath();
            }
        }

        if (currentPath.Count > 0)
        {
            float step = walkSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, currentPath[0], step);

            if (Vector3.Distance(transform.position, currentPath[0]) <= distanceMargin)
            {
                currentPath.RemoveAt(0);
            }
            if (currentPath.Count == 0)
            {
                return currentState = state.succes;
            }
        }
        else
        {
            return currentState = state.failed;
        }

        return currentState = state.running;
    }
}

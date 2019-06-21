using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetNode : BaseNode
{
    public Transform target;

    List<GameObject> availableTargets;

    public int updateInterval = 1;
    public float walkSpeed = 5;
    public float distanceMargin = 0.5f;

    private List<Vector3> currentPath = new List<Vector3>();
    private List<Vector3> previousPath = new List<Vector3>();

    private Vector3 previousTargetPosition = Vector3.zero;

    private AStar AObject;

    private state currentState = state.failed;

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

    private void Start()
    {
        availableTargets = GetComponent<ITargets>().getTargets();

        foreach (GameObject gameObject in availableTargets)
        {
            if (gameObject.GetComponent<IDamagable>() != null)
            {
                if (target == null)
                {
                    target = gameObject.transform;
                }
            }
        }

        //SetPath();
    }

    private bool GetTarget()
    {
        if (target != null)
        {
            return true;
        }

        availableTargets = GetComponent<ITargets>().getTargets();

        foreach (GameObject gameObject in availableTargets)
        {
            if (gameObject == null)
            {
                availableTargets.Remove(gameObject);
            }
            if (gameObject.GetComponent<IDamagable>() != null)
            {
                if (target == null)
                {
                    target = gameObject.transform;
                    return true;
                }
            }
        }

        return false;
    }

    IEnumerator SetPath()
    {
        if (target == null)
        {
            yield break;
        }
        do
        {
            //CoroutineUtility cd = new CoroutineUtility(this, AObject.FindPath(transform, target.transform));
            //yield return cd.coroutine;
            //List<Vector3> path = cd.result as List<Vector3>;
            List<Vector3> path = AObject.FindPath(transform, target.transform);
            if (path != null)
            {
                if (path != previousPath)
                {
                    currentPath = path;
                }
            }
            yield return new WaitForSeconds(updateInterval);
        } while (Vector3.Distance(transform.position, target.transform.position) > AObject.cellSize * 0.95f);
        yield return null;
    }

    public override state Tick()
    {
        if (!GetTarget())
        {
            return currentState = state.failed;
        }

        if (Vector3.Distance(transform.position, target.position) <= distanceMargin)
        {
            target = null;
            return currentState = state.succes;
        }

        if (target.position != previousTargetPosition || currentPath.Count == 0)
        {
            if (currentState != state.running /*|| Time.frameCount % updateInterval == 0*/)
            {
                StartCoroutine(SetPath());
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
                target = null;
                return currentState = state.succes;
            }
        } else
        {
            return currentState = state.failed;
        }

        return currentState = state.running;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetAStar : MonoBehaviour
{
    public GameObject target;
    public int updateInterval = 1;
    public float walkSpeed = 5;
    public float distanceMargin = 0.5f;

    private List<Vector3> currentPath = new List<Vector3>();
    private List<Vector3> previousPath = new List<Vector3>();

    private AStar AObject;

    private void Awake()
    {
        AObject = FindObjectOfType<AStar>().GetComponent<AStar>();
    }

    private void Start()
    {
        List<Vector3> path = AObject.FindPath(transform, target.transform);
        if (path != null)
        {
            if (path != previousPath)
            {
                currentPath = path;
            }
        }

        ////int i = 0;
        //GameObject point = new GameObject("PathObjectTrace");
        ////point.transform.position = Vector3.zero
        //foreach (Vector3 pos in currentPath)
        //{
        //    //GameObject point = new GameObject("Point" + ++i);
        //    //point.name = "Point" + ++i;
        //    //point.transform.position = pos;
        //    GameObject p = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Sphere), point.transform);
        //    p.transform.position = /*this.transform.position +*/ pos;
        //}
    }



    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            NewPath();
        }

        if (Time.frameCount % updateInterval == 0)
        {
            if (currentPath.Count > 0)
            {
                float step = walkSpeed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, currentPath[0], step);

                if (Vector3.Distance(transform.position, currentPath[0]) <= distanceMargin)
                {
                    currentPath.RemoveAt(0);
                }
            }
        }
    }

    void NewPath()
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

    //private void Update()
    //{
    //    if (Time.frameCount % updateInterval == 0)
    //    {
    //        List<Vector3> path = AObject.FindPath(transform, target.transform);
    //        if (path != null)
    //        {
    //            if (path != previousPath)
    //            {
    //                StartCoroutine(FollowPath(path));
    //            }
    //        }
    //    }
    //}

    //private IEnumerator FollowPath(List<Vector3> path)
    //{
    //    //for (int i = path.Count - 1/*FindStartingPoint(path)*/; i > 0; i--)
    //    for (int i = 0; i < path.Count; i++)
    //    {
    //        while (transform.position != path[i])
    //        {
    //            float step = walkSpeed * Time.deltaTime;
    //            Vector3.MoveTowards(transform.position, path[i], step);
    //            yield return null;
    //        }
    //        yield return null;
    //    }
    //    Debug.Log("Done following");
    //}

    private int FindStartingPoint(List<Vector3> path)
    {
        int index = 0;
        float currentSmallestDistance = (AObject.rows * AObject.columns * AObject.cellSize);
        for (int i = 0; i < path.Count; i++)
        {
            if (Vector3.Distance(this.transform.position, path[i]) < currentSmallestDistance)
            {
                index = i;
            }
        }

        return index;
    }
}

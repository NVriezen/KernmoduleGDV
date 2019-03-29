using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PF_Finding : MonoBehaviour
{
    private Grid gameGrid;
    private GameObject target;
    private float[,] grid;

    // Start is called before the first frame update
    void Start()
    {
        gameGrid = GameObject.FindObjectOfType<Grid>();
        FindPath(target.transform);
    }

    private void FindPath(Transform target)
    {
        Vector2 currentGridPos = GetGridPos(target);
        List<Vector2> openNodes = new List<Vector2>();
        List<Vector2> closedNodes = new List<Vector2>();
        float percentage = 0.9f;

        openNodes.Add(currentGridPos);
        while (openNodes.Count > 0)
        {
            float currentNode = grid[(int)openNodes[0].x, (int)openNodes[0].y];
            currentNode = 100;
            //check neighbours 
            //Vector2 neighbourLeft = 

            float neighbour1 = grid[(int)openNodes[0].x - 1, (int)openNodes[0].y] * percentage;
            float neighbour2 = grid[(int)openNodes[0].x + 1, (int)openNodes[0].y] * percentage;
            float neighbour3 = grid[(int)openNodes[0].x, (int)openNodes[0].y - 1] * percentage;
            float neighbour4 = grid[(int)openNodes[0].x, (int)openNodes[0].y + 1] * percentage;

            //openNodes.Add(neighbour1);
            //openNodes.Add(neighbour1);
            //openNodes.Add(neighbour1);
            //openNodes.Add(neighbour1);

            //add neigbours to list
            //remove current
            openNodes.RemoveAt(0);
        }

    }

    private Vector2 GetGridPos(Transform target)
    {
        float x = target.position.x / gameGrid.cellSize.x;
        float y = target.position.y / gameGrid.cellSize.z;
        return new Vector2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
    }
}

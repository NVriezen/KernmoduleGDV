using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GridNode : IComparer<GridNode>
{
    public GridNode parent;
    public int GScore = 0;
    public int HScore = 0;
    public int FScore = 0;
    public bool unwalkable = false;

    public Vector2Int gridPosition;

    public int Compare(GridNode x, GridNode y)
    {
        return x.FScore < y.FScore ? -1 : 1;
    }
}

public class AStar : MonoBehaviour
{
    [SerializeField] private LayerMask unwalkableLayerMask;
    public int rows;
    public int columns;
    public float cellSize;
    private GridNode[,] grid;
    private List<GridNode> closedList = new List<GridNode>();
    private List<GridNode> openList = new List<GridNode>();

    private List<GridNode> previousPath = new List<GridNode>();

    private void Awake()
    {
        grid = new GridNode[rows, columns];
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                grid[x, y] = new GridNode();
                grid[x, y].gridPosition = new Vector2Int(x, y);
                grid[x, y].GScore = int.MaxValue;
                grid[x, y].HScore = int.MaxValue;
                grid[x, y].FScore = int.MaxValue;
                grid[x, y].parent = grid[x, y];
            }
        }
    }

    private void Start()
    {
        CheckFieldForWalkable();
    }

    public List<Vector3> FindPath(Transform startPosition, Transform targetPosition)
    //public IEnumerator FindPath(Transform startPosition, Transform targetPosition)
    {
        openList = new List<GridNode>();
        closedList = new List<GridNode>();

        GridNode startNode = GetGridNodeAtWorldPos(startPosition.transform);
        GridNode targetNode = GetGridNodeAtWorldPos(targetPosition.transform);

        if (targetNode == null || startNode == null)
        {
            //yield break;
            return null;
        }

        GridNode currentNode = startNode;

        openList.Add(currentNode);
        currentNode.GScore = 0;
        currentNode.HScore = ManhattanDistance(startNode.gridPosition.x, startNode.gridPosition.y, targetNode.gridPosition);
        currentNode.FScore = ComputeFScore(currentNode);

        while (openList.Count > 0)
        {
            openList.Sort((x,y) => x.FScore < y.FScore ? -1 : 1);
            currentNode = openList[0];

            if (currentNode == targetNode)
            {
                TracePath(startNode, currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            int posX = 0;
            int posY = 0;
            for (int x = -1; x <= 1; x++)
            {
                posX = currentNode.gridPosition.x + x; //get actual position of neighbour in grid
                for (int y = -1; y <= 1; y++)
                {
                    posY = currentNode.gridPosition.y + y; //get actual position of neighbour in grid

                    if (posX < 0 || posX >= rows)  //check if this is actually inside the grid
                    {
                        continue;
                    }

                    if (posY < 0 || posY >= columns)
                    {
                        continue;
                    }

                    if (x == 0 && y == 0)
                    {
                        continue;
                    }

                    GridNode neighbour = grid[posX, posY];

                    if (closedList.Contains(neighbour))
                    {
                        continue;
                    }

                    if (neighbour.unwalkable)
                    {
                        continue;
                    }

                    int newGScore = currentNode.GScore + ManhattanDistance(posX, posY, currentNode.gridPosition);

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                    else if (/*currentNode.GScore*/ newGScore >= neighbour.GScore)
                    {
                        continue;
                    }

                    neighbour.HScore = ManhattanDistance(posX, posY, targetNode.gridPosition);
                    neighbour.GScore = newGScore;//currentNode.GScore + 1;
                    int newFScore = ComputeFScore(neighbour);

                    neighbour.parent = currentNode;
                    neighbour.FScore = newFScore;

                    if (neighbour == targetNode)
                    {
                        openList.Clear();
                        closedList.Clear();
                        /*yield */return TracePath(startNode, neighbour);//return TracePath(startNode, neighbour);
                    }
                }
            }
        }
        return null;
    }

    private List<Vector3> TracePath(GridNode beginNode, GridNode endNode)
    {
        List<Vector3> resultingPath = new List<Vector3>();
        GridNode currentNode = endNode;


        while (currentNode != beginNode)
        {
            previousPath.Add(currentNode);

            resultingPath.Add(new Vector3(GetWorldPosFromGridPos(currentNode).x, this.transform.position.y, GetWorldPosFromGridPos(currentNode).y));
            currentNode = currentNode.parent;
        }
        
        resultingPath.Reverse();

        return resultingPath;
    }

    private int ComputeFScore(GridNode node)
    {
        return node.GScore + node.HScore;
    }

    private int ManhattanDistance(int x, int y, Vector2 target)
    {
        int result = Mathf.FloorToInt(Mathf.Abs(target.x - x) + Mathf.Abs(target.y - y));

        return result;
    }

    public GridNode GetGridNodeAtWorldPos(Transform obj)
    {

        int x = Mathf.FloorToInt((obj.position.x / cellSize + (rows * 0.5f)) - (this.transform.position.x / cellSize));
        int y = Mathf.FloorToInt((obj.position.z / cellSize + (columns * 0.5f)) - (this.transform.position.y / cellSize));

        GridNode result = grid[x, y];

        return result;
    }

    public Vector2 GetWorldPosFromGridPos(GridNode node)
    {
        float x = (((node.gridPosition.x - rows * 0.5f)) * cellSize) + this.transform.position.x/*+ cellSize * 0.5f*/;
        float y = (((node.gridPosition.y - columns * 0.5f)) * cellSize) + this.transform.position.y /*+ cellSize * 0.5f*/;

        //return grid[x, y];
        return new Vector2(x, y); 
    }

    public void CheckFieldForWalkable()
    {
        foreach (GridNode node in grid)
        {
            //node.gridPosition
            if (Physics.CheckBox(new Vector3(GetWorldPosFromGridPos(node).x, transform.position.y, GetWorldPosFromGridPos(node).y), new Vector3(cellSize*0.5f, cellSize*0.5f, cellSize*0.5f), transform.rotation, unwalkableLayerMask))
            {
                node.unwalkable = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Color gizmoLineColor = new Color(0.8f, 0.8f, 0.4f, 1f);
        Color dimColor = new Color(gizmoLineColor.r, gizmoLineColor.g, gizmoLineColor.b, 0.25f * gizmoLineColor.a);

        int minWorldX = Mathf.FloorToInt(rows * -0.5f);
        int maxWorldX = Mathf.FloorToInt(rows * 0.5f);
        int minWorldY = Mathf.FloorToInt(columns * -0.5f);
        int maxWorldY = Mathf.FloorToInt(columns * 0.5f);

        if (grid != null)
        {
            foreach (GridNode node in grid)
            {
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                if (node.unwalkable)
                {
                    Gizmos.color = new Color(1, 0, 0, 0.5f);
                } else if (previousPath.Contains(node))
                {
                    Gizmos.color = new Color(0,0,1,0.5f);
                }
                Gizmos.DrawCube(new Vector3(GetWorldPosFromGridPos(node).x, transform.position.y, GetWorldPosFromGridPos(node).y), new Vector3(cellSize, cellSize, cellSize));
            }
        }

        for (int x = minWorldX; x < maxWorldX + 1; x++)
        {
            Gizmos.color = (x % 5 == 0 ? gizmoLineColor : dimColor);
            if (x == 0)
                Gizmos.color = dimColor * 2;

            Vector3 pos1 = new Vector3(x, 0, minWorldY) * cellSize;
            Vector3 pos2 = new Vector3(x, 0, maxWorldY) * cellSize;

            Gizmos.DrawLine(pos1, pos2);
        }

        for (int y = minWorldY; y < maxWorldY + 1; y++)
        {
            Gizmos.color = (y % 5 == 0 ? gizmoLineColor : dimColor);
            if (y == 0)
                Gizmos.color = dimColor * 2;

            Vector3 pos1 = new Vector3(minWorldX, 0, y) * cellSize;
            Vector3 pos2 = new Vector3(maxWorldX, 0, y) * cellSize;

            Gizmos.DrawLine(pos1, pos2);
        }
    }
}
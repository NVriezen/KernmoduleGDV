using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid2D : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;

	private Node[,] grid;
	private float nodeDiameter;
	private int gridSizeX, gridSizeY;


	void Start () {
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt ( gridWorldSize.x / nodeDiameter );
		gridSizeY = Mathf.RoundToInt ( gridWorldSize.y / nodeDiameter );
		CreateGrid ();
	}


	void CreateGrid () {
		grid = new Node[gridSizeX, gridSizeY];
		Vector3 WorldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.up * gridWorldSize.y / 2;

		for ( int x = 0; x < gridSizeX; x++ ) {
			for ( int y = 0; y < gridSizeY; y++ ) {
				Vector3 worldPoint = WorldBottomLeft + Vector3.right * ( x * nodeDiameter + nodeRadius ) + Vector3.up * ( y * nodeDiameter + nodeRadius );
				bool walkable = !Physics.CheckSphere ( worldPoint, nodeRadius - 0.1f, unwalkableMask ); //Check for multiple masks and set multiple bools for walkable, interactable, grass, etc.
				grid [ x, y ] = new Node ( walkable, worldPoint, x, y );
				//disable the colliders for checking nodes. They are not neccesary anymore!
				//Hope it gives us a tiny bit performance because of non physics checking.
			}
		}
	}


	public List<Node> GetNeigbours ( Node node ) {
		List<Node> neighbours = new List<Node> ();

		for ( int x = -1; x <= 1; x++ ) {
			for ( int y = -1; y <= 1; y++ ) {
				if ( x == 0 && y == 0 ) {
					continue;
				}
			
				int diagonalNode = Mathf.Abs ( x + y );
				if ( diagonalNode == 0 || diagonalNode == 2 ) { //to prevent form going diagonally
					continue;
				}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if ( checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY ) {
					neighbours.Add ( grid [ checkX, checkY ] );
				}
			}
		}

		return neighbours;
	}


	public Node NodeFromWorldPoint ( Vector3 worldPosition ) {
		float percentX = ( ( worldPosition.x - transform.position.x ) + gridWorldSize.x * 0.5f ) / nodeDiameter;
		float percentY = ( ( worldPosition.y - transform.position.y ) + gridWorldSize.y * 0.5f ) / nodeDiameter;

		percentX = Mathf.Clamp ( percentX, 0, gridWorldSize.x - 1 );
		percentY = Mathf.Clamp ( percentY, 0, gridWorldSize.y - 1 );

		int x = Mathf.FloorToInt ( percentX );
		int y = Mathf.FloorToInt ( percentY );
		return grid [ x, y ];
	}


	public static List<Node> GetPath () {
		return path;
	}


	public static List<Node> path;


	void OnDrawGizmos () {
		Gizmos.DrawWireCube ( transform.position, new Vector3 ( gridWorldSize.x, gridWorldSize.y, 1 ) );

		if ( grid != null ) {
			Node playerNode = NodeFromWorldPoint ( player.position );
			foreach ( Node n in grid ) {
				Gizmos.color = ( n.walkable ) ? Color.white : Color.red;
				if ( playerNode == n ) {
					Gizmos.color = Color.cyan;
				}
				if ( path != null ) {
					if ( path.Contains ( n ) ) {
						Gizmos.color = Color.black;
					}
				}
				Gizmos.DrawCube ( n.worldPosition, Vector3.one * ( nodeDiameter - 0.5f ) );
			}
		}
	}
}

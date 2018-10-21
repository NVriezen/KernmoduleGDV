using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public bool walkable { get; set; }
	public Vector3 worldPosition { get; set; }
	public int gridX { get; set; }
	public int gridY { get; set; }
	public int gCost { get; set; }
	public int hCost { get; set; }
	public Node parent { get; set; }


	public Node ( bool _walkable, Vector3 _worldPos, int _gridX, int _gridY ) { //One bool or array of bools? Or decorator pattern???
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}


	public int fCost {
		get {
			return gCost + hCost;
		}
	}
}

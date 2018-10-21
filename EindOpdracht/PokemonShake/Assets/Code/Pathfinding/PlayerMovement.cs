using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public GameObject indicator;
	public float deltaWalkTime;
	public Vector3 yPlayerOffset;
	public List<Node> path { get; set; }
	public static bool activeMenu { get; set; }

	private Camera cam;
	private Vector3 startPosition;
	private Vector3 previousPos;
	private int currentNode;


	private void Start () {
		cam = Camera.main;
		path = Grid2D.path;
		activeMenu = false;
	}


	private void Update () {
		if ( !activeMenu ) {
			if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Began ) {
				Vector3 deltaPos = cam.ScreenToWorldPoint ( Input.GetTouch ( 0 ).position );
				indicator.transform.position = new Vector3 ( Mathf.Sign ( deltaPos.x ) * ( Mathf.Abs ( ( int ) deltaPos.x ) + 0.5f ), Mathf.Sign ( deltaPos.y ) * ( Mathf.Abs ( ( int ) deltaPos.y ) + 0.5f ), 0 );
				currentNode = 0;
			}
			FollowPath ();
		}
	}


	void FollowPath () {
		path = Grid2D.GetPath ();
		if ( path != null ) {
			this.transform.position = Vector3.Lerp ( this.transform.position, path [ currentNode ].worldPosition, deltaWalkTime * Time.deltaTime ); //Why causes this smooth walking?
			if ( this.transform.position == path [ currentNode ].worldPosition && currentNode < path.Count - 2 ) { //Why currentNode < path.count - 2????
				++currentNode;
			}
			//Check currentNode for grass
			//If grass, call function for encountering Pokemon
		}
	}
}

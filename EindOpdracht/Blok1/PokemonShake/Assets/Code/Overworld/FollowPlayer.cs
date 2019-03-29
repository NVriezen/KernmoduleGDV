using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public GameObject target;
	public Vector3 deltaPos;

	// Update is called once per frame
	void Update () {
		this.transform.position = new Vector3 ( target.transform.position.x, target.transform.position.y, -10 ) + deltaPos;
	}
}

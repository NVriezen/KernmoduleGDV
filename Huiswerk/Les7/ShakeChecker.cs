using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeChecker : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public float shakeThreshold;
	public int frameSkip;
	public static int shakes { get; set; }
	public static bool holding { get; set; }
	public static bool shaking { get; set; }


	void Update () {
		//shakes
		holding = false;
		shaking = false;
		if ( Input.acceleration.magnitude >= shakeThreshold && Time.frameCount % frameSkip == 0 ) {
			//shake happening
			//Debug.Log("Shaken: " + shakes);
			shaking = true;
			shakes++;
			//CheckRumble();
		}

		//Check if holding
		if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Stationary ) {
			holding = true;
		}
		//RumbleActivator.CreateRumbleSequence();
	}

	/*
	public void CheckRumble(){
		if ((Time.frameCount % (Random.Range (1.5f, 3))) == 0) { //determine if rumbling
			if (battleManager.LowerHealth (playerHealth)) {
				Fainted ();
			}//lower HP player

		} else if (holding) { //Defending with stats
			battleManager.LowerHealth (enemyHealth); //lower HP enemy
		} else { //Not defending critical hit

		}
	}
	*/


}

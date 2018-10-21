using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RumbleActivator : MonoBehaviour {

	public long[] rumbles { get; set; } //generate this based on level
	public static bool vibrating { get; set; }


	public RumbleActivator(){
		rumbles = new long[] { 5, 3, 4, 2, 3, 4 };
		vibrating = false;
	}


	public void CreateRumbleSequence ( int level = 1 ) { //We want some kind of input. Rumble needs to be more difficult when pokemon is more powerfull
		Debug.Log ( "sequence started" );
		//Vibration.CreateWaveform(rumbles, 2); //For api 26 and higher, but cant test right now
		StartCoroutine ( Vibrate ( rumbles, 2 ) );
	}


	IEnumerator Vibrate ( long[] timings, int repeat ) {
		Debug.Log ( "Coroutine Started" );
		for ( int i = 0; i < timings.Length; i++ ) {
			//Debug.Log ("i " + timings.Length);
			//Debug.Log ("Timings: " + timings[0]);
			for ( long j = 0; j < timings [ i ]; j++ ) {
				Debug.Log ( "Timings: " + timings [ i ] );
				if ( i % 2 != 0 ) {
					vibrating = true;
					Handheld.Vibrate ();
					Debug.Log ( "Vibrating NOW" );
					yield return new WaitForSeconds ( 0.8f );
				} else {
					vibrating = false;
					Debug.Log ( "Waiting NOW" );
					yield return new WaitForSeconds ( 1 );
				}
			}
		}
		vibrating = false;
		CreateRumbleSequence ();
	}
}

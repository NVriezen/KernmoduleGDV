using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeObject : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public Text day;
	public Text dayTime;
	public Text time;

	private System.DateTime currentTime = System.DateTime.Now;


	void Start () {
		day.text = currentTime.DayOfWeek.ToString ().ToUpper ();
		if ( currentTime.Hour >= 18 && currentTime.Hour < 6 ) {
			dayTime.text = "NITE";
		} else {
			dayTime.text = "DAY";
		}
		time.text = currentTime.Hour.ToString ( "00" ) + ":" + currentTime.Minute.ToString ( "00" );
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

	public static GameObject instance { get; set; }

	private int lastLevel;
	private int currentLevel;


	void Awake () {
		if ( instance == null ) {
			instance = this.gameObject;
		} else {
			Destroy ( this.gameObject );
		}
		DontDestroyOnLoad ( this.gameObject );
	}
}

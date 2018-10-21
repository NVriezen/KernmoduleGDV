using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Began ) {
			SceneManager.LoadSceneAsync ( 6 );
		}
	}
}

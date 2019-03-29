using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorTrigger : MonoBehaviour {

	[SerializeField] private int level;


	void OnTriggerEnter ( Collider collider ) {
		if ( collider.gameObject.name == "Ethan" ) {
			SceneManager.LoadSceneAsync ( level );
		}
	}
}

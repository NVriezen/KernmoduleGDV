using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public GameObject MenuPanel;
	public GameObject[] MenuOptions = new GameObject[7];
	public GameObject pokemonObject;
	public float areaX;

	private Vector3 previousMousePos;
	private TrainerTeam pokemonTeam;


	void Start () {
		MenuPanel.SetActive ( false );
		foreach ( GameObject option in MenuOptions ) {
			option.SetActive ( false );
		}
		pokemonTeam = GameObject.FindObjectOfType<TrainerTeam> ();
	}


	void Update () {
		if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Moved && ( previousMousePos.x >= ( Screen.width - areaX ) ) ) {
			Vector3 heading = ( Input.mousePosition - previousMousePos ).normalized;
			//Debug.Log (heading);
			if ( heading.x < 0 ) {
				//activate menu
				Debug.Log ( "menu activate" );
				ActivateMenu ();
				//freeze player
			}
		} else if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Moved && ( Input.mousePosition.x >= ( Screen.width - areaX * 30 ) ) && !PlayerMovement.activeMenu ) {
			Vector3 heading = ( Input.mousePosition - previousMousePos ).normalized;
			//Debug.Log (heading);
			if ( heading.x > 0 ) {
				//activate menu
				Debug.Log ( "menu disable" );
				DisableMenu ();
				//Unfreeze player
			}
		} else if ( Input.touchCount > 0 && Input.GetTouch ( 0 ).phase == TouchPhase.Began ) {
			previousMousePos = Input.mousePosition;
		}
	}


	public void ActivateMenu () {
		//animate it from right side in view
		PlayerMovement.activeMenu = true;
		MenuPanel.SetActive ( true );
	}


	public void DisableMenu () {
		//animate it from view to right side
		PlayerMovement.activeMenu = false;
		MenuPanel.SetActive ( false );
	}


	public void LoadLevel ( int level ) {
		UnityEngine.SceneManagement.SceneManager.LoadScene ( level );
	}


	public void EnablePokemonMenu () {
		MenuOptions [ 1 ].SetActive ( !MenuOptions [ 1 ].activeInHierarchy );
		//get the players team and instantiate the correct pokemon
		///Debug///
		/// 
		if ( TrainerTeam.pokeTeam.Count == 0 ) {
			pokemonTeam.AddPokemon ( PokemonBase.RandomPokemon ( 158 ) );
		}

		if ( MenuOptions [ 1 ].GetComponentInChildren<UnityEngine.UI.ContentSizeFitter> ().GetComponentsInChildren<Transform>().Length < TrainerTeam.pokeTeam.Count + 1 ) {
			for ( int i = 0; i < TrainerTeam.pokeTeam.Count; i++ ) {
				GameObject listPoke = Instantiate ( pokemonObject, MenuOptions [ 1 ].GetComponentInChildren<UnityEngine.UI.ContentSizeFitter> ().gameObject.transform );
				Debug.Log ( TrainerTeam.pokeTeam [ 0 ].Name ); //printing name
				listPoke.GetComponentInChildren<UnityEngine.UI.Text> ().text = TrainerTeam.pokeTeam [ i ].Name;
			}
		}
	}
}

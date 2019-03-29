using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour {

	public static PokemonBase selectedPokemon { get; set; }

	private BattleManager manager;


	void ActivateMenu () {
		selectedPokemon = null;
		//activate the UI to switch pokemon
	}


	IEnumerator PokemonSet () {
		while ( selectedPokemon == null ) {
			yield return null;
		}
		if ( !selectedPokemon.fainted ) {
			//SetActiveTrainerPokemon (selectedPokemon);
		} else {
			Debug.Log ( "Pokemon is fainted and cant be used in battle" ); //output error message in game
		}
	}

	//When clicked this is the pokemon to give back
	public void OnTrainerSwitch () {
		ActivateMenu ();
		StartCoroutine ( PokemonSet () );
		//Check if pokemon is not fainted
		//manager.SetPokemon();
	}


	public void OnEnemySwitch () { //Maybe use decorator pattern to specify if wild pokemon, trainer or friend trainer
		//Check if pokemon is not fainted
		//manager.SetPokemon();
		EnemyFainted ();
	}


	public void TrainerFainted () {
		//change pokemon
		//check whose pokemon it is
	}


	public void EnemyFainted () {
		//change pokemon
		//check whose pokemon it is

	}
}

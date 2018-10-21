using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour {

	public PokemonBattleContainer activePlayerPokemon { get; set; }
	public PokemonBattleContainer activeEnemyPokemon { get; set; }

	private RumbleActivator rumbleManager;


	void Start () {
		activePlayerPokemon = GameObject.Find("Player").GetComponent<PokemonBattleContainer>();
		activeEnemyPokemon = GameObject.Find("Enemy").GetComponent<PokemonBattleContainer>();
		rumbleManager = FindObjectOfType<RumbleActivator> ();
		//EventManager.TriggerEvent ("Battle");
		rumbleManager.CreateRumbleSequence ( activeEnemyPokemon.GetLevel () );
	}


	void Update () {
		if ( ShakeChecker.shaking && ( ( ShakeChecker.shakes % activePlayerPokemon.CalculateShakeRate () ) == 0 ) ) {
			Debug.Log ( RumbleActivator.vibrating );
			if ( RumbleActivator.vibrating ) {
				Debug.Log ( "damage done" );
				//LowerHealth (playerHealth, false);
				activePlayerPokemon.DecreaseHealth ( false );
			} else {
				Debug.Log ( "damage done2" );
				//LowerHealth (enemyHealth);
				activeEnemyPokemon.DecreaseHealth ();
			}
		} else if ( RumbleActivator.vibrating && ShakeChecker.holding && ( Time.frameCount % 10 ) == 0 ) {
			Debug.Log ( "damage done3" );
			activePlayerPokemon.DecreaseHealth ( true );
		} else if ( RumbleActivator.vibrating && !ShakeChecker.holding && ( Time.frameCount % 3 ) == 0 ) {
			activePlayerPokemon.DecreaseHealth ( false );
		}

		if ( activeEnemyPokemon ) {

		}
	}

	/*
	public bool LowerHealth(Transform healthBar, bool isDefending = false){ //If not isDefending given, randomize the outcome
		//get pokemons total health
		//calculate percentage of total
		Debug.Log("here");
		if (healthBar.localScale.x >= 0.001f) {
			healthBar.localScale -= new Vector3(CalculateDamage()*0.01f, 0, 0); //Still need to implement defending!!
		}

		if (healthBar.localScale.x <= 0.001f){
			//Fainted (); //give correct PokemonBase
			return true;
		}

		return false;

	}


	private int CalculateDamage(){
		return 1;
	}
	*/
}

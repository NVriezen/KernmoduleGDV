using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildPokemonContainer : PokemonBattleContainer {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public Transform healthBar;

	private OpponentTeam pokeTeam;
	private PokemonBase activePokemon;
	private SpriteRenderer activeSprite;


	public override void Awake () {
		pokeTeam = this.gameObject.GetComponent<OpponentTeam> ();
		Debug.Log ( pokeTeam.gameObject.name );
		if ( pokeTeam.pokeTeam.Count == 0 ) {
			PokemonBase pokepon = PokemonBase.RandomPokemon ( DebugNumber () );
			pokeTeam.pokeTeam.Add ( pokepon ); //DEBUG//
		}
		//PokemonBase sentretje = PokemonBase.RandomPokemon (163);
		//pokeTeam.pokeTeam.Add(sentretje); //DEBUG//
		activeSprite = gameObject.transform.GetChild ( 0 ).GetChild ( 0 ).GetComponent<SpriteRenderer> ();
		activePokemon = pokeTeam.pokeTeam [ 0 ];
		activeSprite.sprite = Resources.Load<Sprite> ( "Sprites/Front/Normal/" + activePokemon.DexNum );
	}


	/// <summary>
	/// DEBUGGING
	/// </summary>
	/// <returns>The number.</returns>
	public int DebugNumber () {
		switch ( Random.Range ( 0, 6) ) {
		case 1:
			return 16;
		case 2:
			return 19;
		case 3:
			return 158;
		case 4:
			return 161;
		case 5:
			return 163;
		default:
			return 161;
		}
	}


	/// END

	public override void SwitchPokemon ( PokemonBase poke ) {
		if ( !poke.fainted ) {
			activePokemon = poke;
		}
	}


	public override bool DecreaseHealth ( bool isDefending = false ) {
		Debug.Log ( "Decreasing Enemy Health" );
		if ( healthBar.localScale.x >= 0.001f ) {
			healthBar.localScale -= new Vector3 ( CalculateDamage () * 0.01f, 0, 0 ); //Still need to implement defending!!
		}
		if ( healthBar.localScale.x <= 0.001f ) {
			Fainted ();
			return true; //TriggerEvent
		}
		return false;
	}


	public override int CalculateDamage () {
		return 1;
	}


	public override int CalculateShakeRate () {
		return ( int ) ( activePokemon.Attack * 0.1f );
	}


	public override void Fainted () {
		//switch it out;
		activePokemon.fainted = true;
		//Call UI manager
		UnityEngine.SceneManagement.SceneManager.LoadScene ( 1 ); //DEBUG//
	}


	public override int GetLevel () {
		//switch it out;
		return activePokemon.Level;
		//Call UI manager
	}


}

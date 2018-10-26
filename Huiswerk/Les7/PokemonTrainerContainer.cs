using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonTrainerContainer : PokemonBattleContainer {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public Transform healthBar;

	private PokemonBase activePokemon;
	private SpriteRenderer activeSprite;


	public override void Awake () {
		if ( TrainerTeam.pokeTeam.Count == 0 ) {
			PokemonBase pokepon = PokemonBase.RandomPokemon ( 163 );
			TrainerTeam.pokeTeam.Add ( pokepon ); //DEBUG//
		}
		activeSprite = gameObject.transform.GetChild ( 0 ).GetChild ( 0 ).GetComponent<SpriteRenderer> ();
		activePokemon = TrainerTeam.pokeTeam [ 0 ];
		if ( activePokemon.Type1 == null ) {
			activeSprite.sprite = Resources.Load<Sprite> ( "Sprites/Back/Normal/NoEntry" );
		} else {
			activeSprite.sprite = Resources.Load<Sprite> ( "Sprites/Back/Normal/" + activePokemon.DexNum );
		}
	}


	public override void SwitchPokemon ( PokemonBase poke ) {
		if ( !poke.fainted ) {
			activePokemon = poke;
		}
	}


	public override bool DecreaseHealth ( bool isDefending = false ) {
		if ( healthBar.localScale.x >= 0.001f ) {
			healthBar.localScale -= new Vector3 ( CalculateDamage () * 0.01f, 0, 0 ); //Still need to implement defending!!
		}
		if ( healthBar.localScale.x <= 0.001f ) {
			Fainted ();
			return true;
		}
		return false;
	}


	public override int CalculateDamage () {
		return 1;
	}


	public override int CalculateShakeRate () {
		//if ( ( int ) ( activePokemon.Attack * 0.1f ) >= 0 ) {
			
		//}
		return 1;
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

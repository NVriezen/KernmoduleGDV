using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainerTeam : PokemonTeam {

	public static List<PokemonBase> pokeTeam = new List<PokemonBase>();

	private static TrainerTeam instance;


	void Awake () {
		if ( instance == null ) {
			instance = this;
		} else {
			Destroy ( transform.root.gameObject );
		}
		//DontDestroyOnLoad (transform.root.gameObject);
	}


	public override void AddPokemon ( PokemonBase pokemon ) {
		if ( pokeTeam.Count < 6 ) {
			pokeTeam.Add ( pokemon );
		} else {
			//give error message or send to PC
			//We can let this method return a string which gives the user info (via json)
		}

	}


	public override void AddPokemon ( int index ) {

	}


	public override void RemovePokemon ( int index ) {

	}
}


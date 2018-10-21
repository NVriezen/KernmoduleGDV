using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentTeam : PokemonTeam {
	
	public List<PokemonBase> pokeTeam;

	private OpponentTeam instance;


	public OpponentTeam(){
		pokeTeam = new List<PokemonBase> ();
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

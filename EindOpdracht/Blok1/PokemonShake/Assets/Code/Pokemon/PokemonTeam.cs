using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PokemonTeam : MonoBehaviour {

	public abstract void AddPokemon ( PokemonBase pokemon );


	public abstract void AddPokemon ( int index );


	public abstract void RemovePokemon ( int index );
}

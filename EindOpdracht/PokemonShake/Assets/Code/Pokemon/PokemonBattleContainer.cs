using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PokemonBattleContainer : MonoBehaviour {

	public abstract void Awake ();


	public abstract void SwitchPokemon ( PokemonBase poke );


	public abstract bool DecreaseHealth ( bool isDefending = false );


	public abstract int CalculateDamage ();


	public abstract int CalculateShakeRate ();


	public abstract void Fainted ();


	public abstract int GetLevel ();


}

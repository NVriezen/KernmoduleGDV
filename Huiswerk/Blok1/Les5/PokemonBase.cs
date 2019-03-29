using System.Collections;
using UnityEngine;

public class PokemonBase : MonoBehaviour {

	public int DexNum { get; set; }
	public int DexNumJohto { get; set; }
	public int Health { get; protected set; }
	public int CurrentHealth { get; set; }
	public string Type1 { get; protected set; }
	public string Type2 { get; protected set; }
	public int Attack { get; protected set; }
	public int Defense { get; protected set; }
	public int Speed { get; protected set; }
	public int Level { get; set; }
	public int Exp { get; set; }
	public bool Gender { get; protected set; }
	public bool fainted;
	public bool Shiny{ get; protected set;}
	public string Name { get; set; } 

	private static float shinyRate = 8192;

	//Debugging
	public static PokemonBase RandomPokemon (int dexNum){
		GameObject temp = new GameObject ();
		PokemonDataManager data = new PokemonDataManager();
		Pokemon pokemonData = data.GetPokemon (dexNum);
		PokemonBase poke = temp.AddComponent<PokemonBase> ();

		poke.DexNum = pokemonData.No;
		poke.DexNumJohto = pokemonData.JohtoNo;
		poke.Type1 = pokemonData.Type1;
		poke.Type2 = pokemonData.Type2;
		poke.Health = pokemonData.BaseStats.HP;
		poke.Attack = pokemonData.BaseStats.Attack;
		poke.Defense = pokemonData.BaseStats.Defense;
		poke.Speed = pokemonData.BaseStats.Speed;
		poke.Name = pokemonData.Name;

		if (Random.Range (0.0f, 1.0f) == 0.5f) {
			poke.Gender = true;
		} else {
			poke.Gender = false;
		}
				
		if (Random.Range (0, shinyRate) == 0) {
			poke.Shiny = true;
		} else {
			poke.Shiny = false;
		}
		return poke;
	}

	PokemonBase NewPokemon(){
		PokemonBase poke = new PokemonBase ();
		poke.Level = 5;
		poke.Exp = 0;
		return poke;
	}
	PokemonBase NewPokemon(int dexNum){
		PokemonBase poke = new PokemonBase ();
		poke.Level = 5;
		poke.Exp = 0;
		return poke;
	}

	void SetStats(){
		//Get everything from the JSON via DexNum;
	}
}

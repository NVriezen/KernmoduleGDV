using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class MyData{
	public Pokemon[] Pokemon;
}

[System.Serializable]
public class Pokemon{
	public int No;
	public int JohtoNo;
	public string Name;
	public string Type1;
	public string Type2;
	public float GenderChance;
	public string Classification;
	public string MenuType;
	public float Height;
	public float Weight;
	public BaseStats BaseStats;
}

[System.Serializable]
public class BaseStats{
	public int Attack;
	public int Defense;
	public int HP;
	public int Speed;
}

public class PokemonDataManager : MonoBehaviour {

	public Pokemon[] pokeArray;
	object myLoadJson;

	PokemonDataManager pokemonDataManager;

	public Pokemon[] loadDataResources(){
		#if UNITY_ANDROID
		string filePath = Application.streamingAssetsPath + "/PokemonData.json";
		#elif UNITY_IOS
		string filePath = Path.Combine(Application.streamingAssetsPath + "/Raw", "PokemonData.json");
		#elif UNITY_STANDALONE_WIN
		string filePath = Path.Combine(Application.streamingAssetsPath, "PokemonData.json");
		#endif

		WWW www = new WWW (filePath);
		while (!www.isDone) {}
		var json = www.text;
		Pokemon[] pokemons = JsonUtility.FromJson<MyData>(json).Pokemon;

		return pokemons;
	}

	public Pokemon GetPokemon(int dexNum){
		pokeArray = loadDataResources();

		//for loop, in future can be replaced by just accessing by index
		for (int i = 0; i < pokeArray.Length; i++){
			if (pokeArray[i].No == dexNum){
				return pokeArray[i];
			}
		}
		return null;
	}
}

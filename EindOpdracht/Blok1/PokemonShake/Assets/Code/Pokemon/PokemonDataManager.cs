﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class MyData {
	public Pokemon[] Pokemon;
}


[System.Serializable]
public class Pokemon {
	//Geen gebruik van Proprties vanwege het feit dat JSON anders niet werkt!
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
public class BaseStats {
	//Geen gebruik van Proprties vanwege het feit dat JSON anders niet werkt!
	public int Attack;
	public int Defense;
	public int HP;
	public int Speed;
}


public class PokemonDataManager : MonoBehaviour {

	public Pokemon[] pokeArray;

	private object myLoadJson;
	private PokemonDataManager pokemonDataManager;


	public Pokemon[] loadDataResources () {
		#if UNITY_ANDROID && !UNITY_EDITOR
		string filePath = Application.streamingAssetsPath + "/PokemonData.json";//Path.Combine(filePath, "/PokemonData.json");
		#elif UNITY_IOS
		string filePath = Path.Combine(Application.streamingAssetsPath + "/Raw", "PokemonData.json");
		#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, "PokemonData.json");
		#endif

		WWW www = new WWW ( filePath );
		while ( !www.isDone ) {
			Debug.Log ( "waiting to be done downloading" );
		}

		var json = www.text;
		Pokemon[] pokemons = JsonUtility.FromJson<MyData> ( json ).Pokemon;
		return pokemons;
	}


	public Pokemon GetPokemon ( int dexNum ) {
		pokeArray = loadDataResources ();
		//for loop, in future can be replaced by just accessing by index
		return pokeArray [ dexNum - 1 ];
	}


	IEnumerator GetStreamAsset ( string js ) {
		#if UNITY_ANDROID && !UNITY_EDITOR
		string filePath = Path.Combine ( "file://", Application.streamingAssetsPath );
		filePath = Path.Combine ( filePath, "/PokemonData.json" );
		#elif UNITY_IOS
		string filePath = Path.Combine(Application.streamingAssetsPath + "/Raw", "PokemonData.json");
		#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
		string filePath = Path.Combine(Application.streamingAssetsPath, "PokemonData.json");
		#endif
		WWW www = new WWW ( filePath );
		yield return www;
		js = www.text;

		yield break;
	}
}

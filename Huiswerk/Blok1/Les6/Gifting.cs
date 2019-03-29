using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gifting : WifiDirectBase {

	//Geen gebruik van Proprties vanwege het niet kunnen toewijzen van een component in de inspector!
	public GameObject buttonList;
	public GameObject addrButton;
	public GameObject addrCanvas;
	public GameObject receivedPokemon;

	private PokemonTeam pokemonTeam;


	// Adds listeners to the color sliders and calls the initialize script on the library
	void Start () {
		addrCanvas.SetActive ( true );
		pokemonTeam = GameObject.FindObjectOfType<PokemonTeam> ();
		base.initialize ( this.gameObject.name );
	}

	//when the WifiDirect services is connected to the phone, begin broadcasting and discovering services
	public override void onServiceConnected () {
		addrCanvas.SetActive ( true );
		Dictionary<string, string> record = new Dictionary<string, string> ();
		record.Add ( "PokemonShake", "GiftShake" );
		base.discoverServices ();
		base.broadcastService ( "Gifting", record );
	}

	//On finding a service, create a button with that service's address
	public override void onServiceFound ( string addr ) {
		GameObject newButton = Instantiate ( addrButton );
		newButton.GetComponentInChildren<Text> ().text = addr;
		newButton.transform.SetParent ( buttonList.transform, false );
		newButton.GetComponent<Button> ().onClick.AddListener ( () => {
			this.makeConnection ( addr );
		} );
	}

	//When the button is clicked, connect to the service at its address
	private void makeConnection ( string addr ) {
		base.connectToService ( addr );
	}

	//When connected, begin sending random pokemon number
	public override void onConnect () {
		//addrCanvas.SetActive(false);
		base.sendMessage ( Random.Range ( 1, 252 ).ToString () );
	}


	public int DebugNumber () {
		switch ( Random.Range ( 1, 252 ) ) {
		case 16:
			return 16;
		case 19:
			return 19;
		case 158:
			return 158;
		case 161:
			return 161;
		case 163:
			return 163;
		default:
			return 161;
		}
	}

	//When receiving a new message
	public override void onMessage ( string message ) {
		int dexNum;
		if ( !int.TryParse ( message, out dexNum ) ) {
			dexNum = 0;
		}
		Debug.Log ( "We received message and got a dexnum: " + dexNum );
		PokemonBase pokemon = receivedPokemon.GetComponent<PokemonBase> ();
		Debug.Log ( "PokemonBase is set" );
		pokemon = PokemonBase.RandomPokemon ( dexNum );
		Debug.Log ( "Random Pokemon has been generated with dex number: " + dexNum + "\n It's name is " + pokemon.Name );
		TextMesh pokeText = receivedPokemon.GetComponentInChildren<TextMesh> ();
		pokeText.text = ( "#" + pokemon.DexNum + " " + pokemon.Name );

		if ( Resources.Load<Sprite> ( "Sprites/Front/Normal/" + dexNum ) != null ) {
			receivedPokemon.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ( "Sprites/Front/Normal/" + dexNum );
			Debug.Log ( "Sprite loaded from " + dexNum );
		} else {
			receivedPokemon.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> ( "Sprites/Front/Normal/NoEntry" );
			Debug.Log ( "Sprite loaded, unfortuantely NoEntry" );
		}

		addrCanvas.SetActive ( false );
		pokemonTeam.AddPokemon ( pokemon );
		//save game????
		//Play Animation
	}

	//Kill Switch
	public override void onServiceDisconnected () {
		base.terminate ();
		Application.Quit ();
	}

	//Kill Switch
	public void OnApplicationPause ( bool pause ) {
		if ( pause ) {
			this.onServiceDisconnected ();
		}
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {

	public string name;
	protected Sprite texture;
	protected Recipe[] craftingRecipes; //Geen idee waarom deze er is

	public void use(){
		//doe fancy stuff. 
		//volgens mij is het de bedoeling hier meerdere verschillende subclasses van te hebben
	}
}

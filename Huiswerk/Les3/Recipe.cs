using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recipe : MonoBehaviour {

	public Item[] ingredients;
	public Item resultItem;

	public bool canCraft(Item[] playerInv){
		//if each ingredient is present in playerInv;
		return true;
	}

	public Item craftRecipe(){
		return resultItem;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public int score;
	protected int itemSelected;

	//interface
	protected float HP;
	protected Item[] loot;
	public string name {
		get;
		set;
	}
	protected Sprite texture;

	public void canCraft(Recipe recipe){
		if (recipe.canCraft (loot)) {
			//iets met itemSelected om recept te gebruiken?
			//Daarvoor mis ik wel een functie
			recipe.craftRecipe();
		}
	}

	public void damage (int val){
		HP -= val;
		if (HP <= 0){
			kill ();
		}
	}

	private void kill(){
		//Get the loot and drop it
		getLoot ();
	}

	public Item[] getLoot() {
		return loot;
	}

	public float getHP(){
		return HP;
	}

	public void move(Vector2 dir){
		//input checken en Rigidbody2d.AddForce of met transform.position
	}
}

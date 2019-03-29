using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	//interface
	protected float HP;
	protected Item[] loot;
	public string name {
		get;
		set;
	}
	protected Sprite texture;

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
		//just add velocity or something
	}
}

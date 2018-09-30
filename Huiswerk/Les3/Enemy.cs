using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, BaseEntity {

	public int score;
	public Vector2[] movePattern;
	public float[] moveDuration;

	//interface
	protected float HP = 50;
	protected Item[] loot;
	public string name {
		get;
		set;
	}
	protected Sprite texture;

	//Ik miste de projectilePrefab
	public GameObject projectile;

	public void attack(){
		//some animations playing
		//I assume they shoot projectiles.
		//Instantiate(projectile);
	}

	public void damage (int val){
		HP -= val;
		if (HP <= 0){
			kill ();
		}
	}

	public void kill(){
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
		Vector2.MoveTowards (transform.position, dir, /* iets doen met moveDuration */ moveDuration[0]);
	}
}

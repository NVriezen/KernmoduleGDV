using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BaseEntity {

	string name { get; set; }

	void damage(int val);
	void kill ();
	Item[] getLoot ();
	float getHP ();
	void move(Vector2 dir);

}

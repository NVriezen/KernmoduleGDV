using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class WallTexturePicker : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Texture2D[] texs = Resources.LoadAll<Texture2D>("Textures");
        GetComponent<Renderer>().sharedMaterial.mainTexture = texs[Random.Range(0,texs.Length)];
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {

	private MainManager instance;

	public float startUserTime {
		get;
		protected set;
	}
	public float endUserTime {
		get;
		protected set;
	}

	private string[ , ] userTimeArray;
	private int currentPlayer = 0;
	private float userTime;
	private string userName;


	// Use this for initialization
	private void Awake () {
		if (Display.displays.Length > 1) Display.displays[1].Activate();
		if (Display.displays.Length > 2) Display.displays[2].Activate();
		if (this != instance && instance == null) instance = this;
		else {
			Destroy (this.gameObject);
		}
		DontDestroyOnLoad (instance.gameObject);
	}


	// Update is called once per frame
	private void Update () {
		if (Input.GetMouseButtonDown (1)) {
			userTime = 899;
			SaveScore ();
		}
		
	}


	public void OnEndEdit(){
		userName = GameObject.Find("InputField").GetComponent<InputField> ().text;
		GameObject.Find ("confirmationText").GetComponent<Animator> ().Play ("ConfirmationAnimation");
		Debug.Log (userName);
	}


	public void OnLogin(){
		OnEndEdit ();
		SceneManager.LoadSceneAsync (1);
	}


	public void SaveScore(){
		userTime = (EndUserTime - startUserTime);

		float[] oldTime = PlayerPrefsX.GetFloatArray("HighscoresTimes", 900, 100);
		string[] oldName = PlayerPrefsX.GetStringArray("HighscoresNames", "AI 1.2", 100);

		for (int i = 0; i < oldTime.Length; i++){
			float oldT = oldTime[i];
			string oldN = oldName [i];
			if (userTime < oldTime[i]) {
				oldTime[i] = userTime;
				oldName [i] = userName;
				userTime = oldT;
				userName = oldN;
			}
		}

		PlayerPrefsX.SetFloatArray ("HighscoresTimes", oldTime);
		PlayerPrefsX.SetStringArray ("HighscoresNames", oldName);
	}


}

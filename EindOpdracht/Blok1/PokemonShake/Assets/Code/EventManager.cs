using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

	public static EventManager instance { get; set; }

	private Dictionary<string, UnityEvent> events;


	void Awake () {
		if ( instance == null ) {
			instance = this;
		} else {
			Destroy ( this.gameObject );
		}
		DontDestroyOnLoad ( this.gameObject );
		instance.Init ();
	}


	void Init () {
		if ( events == null ) {
			events = new Dictionary<string, UnityEvent> ();
		}
	}


	public static void StartListening ( string eventName, UnityAction listener ) {
		UnityEvent thisEvent = null;
		if ( instance.events.TryGetValue ( eventName, out thisEvent ) ) {
			thisEvent.AddListener ( listener );
		} else {
			thisEvent = new UnityEvent ();
			thisEvent.AddListener ( listener );
			instance.events.Add ( eventName, thisEvent );
			Debug.Log ( instance.events.Count );
		}
	}


	public static void StopListening ( string eventName, UnityAction listener ) {
		UnityEvent thisEvent = null;
		if ( instance.events.TryGetValue ( eventName, out thisEvent ) ) {
			thisEvent.RemoveListener ( listener );
		} else {
			return;
		}
	}


	public static void TriggerEvent ( string eventName ) {
		UnityEvent thisEvent = null;
		if ( instance.events.TryGetValue ( eventName, out thisEvent ) ) {
			thisEvent.Invoke ();
		}
	}


}

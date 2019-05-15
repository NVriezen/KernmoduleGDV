using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager {

    private static Dictionary<string, System.Action> eventDictionary = new Dictionary<string, System.Action>();

    public static void StartListening(string eventName, System.Action listener)
    {
        System.Action thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent += listener;
            return;
        }

        thisEvent = new System.Action(listener);
        eventDictionary.Add(eventName, thisEvent);
    }

    public static void StopListening(string eventName, System.Action listener)
    {
        System.Action thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent -= listener;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Debug.Log("event firing - " + eventName);
        System.Action thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke();
            Debug.Log("Fired");
        }
    }
}

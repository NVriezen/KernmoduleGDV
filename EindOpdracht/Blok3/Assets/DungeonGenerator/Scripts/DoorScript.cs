using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
    public void OnTriggerDoor()
    {
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        float t = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos - transform.up * 5;
        while (t < 1)
        {
            t += Time.deltaTime;
            yield return null;
            transform.localPosition = Vector3.Lerp(startPos, endPos, t);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorOnInput : MonoBehaviour {

    public GameObject cam;
    public LayerMask doorLayer;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 5, doorLayer))
            {
                DoorScript door = hit.collider.gameObject.GetComponentInParent<DoorScript>();
                if (door != null)
                {
                    door.OnTriggerDoor();
                }
            }

        }
	}
}

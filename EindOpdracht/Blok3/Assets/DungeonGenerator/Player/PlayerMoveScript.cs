using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveScript : MonoBehaviour {
    private Rigidbody body;
    public Camera cam;
    public float moveSpeed = 5f;

    private float angleX = 0;
    private float angleY = 0;

    public float lookSensX = 15f;
    public float lookSensY = 15f;
    public float maxAngle = 85f;

    private float mouseX, mouseY, hor, vert;
    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
        hor = Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");

        angleX += mouseX * lookSensX;
        angleY += mouseY * lookSensY;
        angleY = Mathf.Clamp(angleY, -maxAngle, maxAngle);

        body.transform.rotation = Quaternion.Euler(0, angleX, 0);
        cam.transform.localRotation = Quaternion.Euler(-angleY, 0, 0);
    }

    void FixedUpdate()
    {
        body.MovePosition(body.position + (vert * body.transform.forward + hor * body.transform.right).normalized * moveSpeed * Time.deltaTime);
    }
}

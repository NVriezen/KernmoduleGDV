using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCamera : MonoBehaviour
{

    public float sensitivityX = 15;
    public float sensitivityY = 15;
    public float moveSpeed = 5;
    float angleX = 0;
    float angleY = 0;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        angleX += mouseX * sensitivityX;
        angleY += mouseY * sensitivityY;
        transform.rotation = Quaternion.Euler(-angleY, angleX, 0);

        float vert = Input.GetAxis("Vertical");
        float hor = Input.GetAxis("Horizontal");

        transform.position += (transform.forward * vert + transform.right * hor).normalized * moveSpeed * Time.deltaTime;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DiveNode : BaseNode
{
    [SerializeField] private Transform groundPlane = null;
    [SerializeField] private float force = 100;
    [SerializeField] private float rotationSpeed = 10;

    private Rigidbody rigidBody;

    //Debug
    int ticks = 0;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override state Tick()
    {
        Vector3 targetLocation;
        if (GetComponent<ITargets>() != null && GetComponent<ITargets>().getTargets() != null)
        {
            targetLocation = (GetComponent<ITargets>().getTargets()[0].transform.position /*- new Vector3(transform.localScale.x, (transform.localScale.x + transform.localScale.y), transform.localScale.z)*/);
            //targetLocation += new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z) * 2;
        }
        else
        {
            targetLocation = (groundPlane.position - new Vector3(transform.localScale.x, (transform.localScale.x + transform.localScale.y), transform.localScale.z));
        }

        rigidBody.isKinematic = false;

        if (transform.position.y < targetLocation.y - (transform.localScale.x + transform.localScale.y))
        {
            rigidBody.isKinematic = true;
            rigidBody.isKinematic = false;
            return state.succes;
        }

        if (groundPlane == null)
        {
            Debug.LogError("DiveNode: No Groundplane found!");
            return state.failed;
        }
        
        Vector3 heading = targetLocation - transform.position;
        //heading = Vector3.Scale(heading, new Vector3(0,1,0));

        transform.rotation = Quaternion.LookRotation(heading, Vector3.up);
        rigidBody.AddForce(heading * force, ForceMode.Force);
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetLocation), rotationSpeed);
        

        return state.running;
    }
}

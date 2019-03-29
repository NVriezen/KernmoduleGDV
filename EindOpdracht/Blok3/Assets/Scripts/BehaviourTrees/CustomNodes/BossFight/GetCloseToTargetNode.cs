using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GetCloseToTargetNode : BaseNode
{
    [SerializeField] private float force = 100;
    [SerializeField] private float distanceTarget = 2;

    private Transform target;
    private Rigidbody rigidBody;


    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public override state Tick()
    {
        if (target == null)
        {
            target = GetComponent<ITargets>().getTargets()[0].transform;
        }

        Vector3 heading = target.position - transform.position;
        heading = Vector3.Scale(heading, new Vector3(1, 0, 1));
        
        rigidBody.AddForce(heading * force, ForceMode.Force);

        Debug.Log("Distance x: " + Mathf.Abs(target.position.x - transform.position.x) + ", distance z: " + Mathf.Abs(target.position.z - transform.position.z));
        if (Mathf.Abs(target.position.x - transform.position.x) <= distanceTarget && Mathf.Abs(target.position.z - transform.position.z) <= distanceTarget)
        {
            return state.succes;
        }

        return state.running;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackNode : BaseNode
{
    [SerializeField] private float force = 100;
    [SerializeField] private float distanceTarget = 2;

    private Transform target;
    private Rigidbody rigidBody;
    private bool waitOneFrame = false;


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


        if (rigidBody.velocity.y <= 0 && waitOneFrame)
        {
            waitOneFrame = false;
            return state.succes;
        }

        waitOneFrame = false;

        rigidBody.isKinematic = false;

        Vector3 heading = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(heading);

        rigidBody.AddForce(heading * force, ForceMode.VelocityChange);

        waitOneFrame = true;

        return state.running;
    }
}

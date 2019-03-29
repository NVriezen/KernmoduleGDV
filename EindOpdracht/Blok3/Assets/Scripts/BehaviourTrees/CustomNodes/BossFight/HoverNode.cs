using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverNode : BaseNode
{
    [SerializeField] private float hoverTime = 800;
    //[SerializeField] private float distanceTarget = 2;

    private Transform target;
    private Rigidbody rigidBody;

    private int ticks = 0;


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

        rigidBody.isKinematic = true;
        //some nice hovering animation

        Vector3 heading = target.position - transform.position;
        transform.rotation = Quaternion.LookRotation(heading);

        if (ticks < hoverTime)
        {
            ticks++;
            return state.running;
        }

        ticks = 0;
        return state.succes;
    }
}
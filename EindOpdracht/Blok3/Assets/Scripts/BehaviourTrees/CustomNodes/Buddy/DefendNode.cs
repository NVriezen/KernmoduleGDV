using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendNode : BaseNode
{
    public float defendCondition = 40;
    public float runAwayDistance = 10;

    //BehaviourBoard mainBoard;
    List<GameObject> availableTargets;
    Transform target;
    IDamagable agent;

    public float speed = 5;

    private void Start()
    {
        availableTargets = GetComponent<ITargets>().getTargets();
        agent = GetComponent<IDamagable>();
    }

    private bool GetTarget()
    {
        if (target != null)
        {
            return true;
        }

        availableTargets = GetComponent<ITargets>().getTargets();

        foreach (GameObject gameObject in availableTargets)
        {
            if (gameObject == null)
            {
                availableTargets.Remove(gameObject);
            }
            if (gameObject.GetComponent<IDamagable>() != null)
            {
                target = gameObject.transform;
                return true;
            }
        }

        return false;
    }

    public override state Tick()
    {
        if (GetComponent<IDamagable>().GetHealth() > defendCondition)
        {
            return state.failed;
        }

        if (!GetTarget())
        {
            return state.failed;
        }

        if (Vector3.Distance(transform.position, target.position) >= runAwayDistance)
        {
            return state.failed;
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, transform.position + (transform.position - target.transform.position).normalized, step);
        //agent.RecoverHealth();
            
        return state.succes;
    }
}

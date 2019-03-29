using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : /*MonoBehaviour,*/ IAttacker, IDamagable
{
    public float moveSpeed = 5;
    public float health = 100;
    //public new float attackPower = 10;
    public GameObject target;

    public void Awake()
    {
        if (GetComponent<TargetComponent>() == null)
        {
            target = null;
        }
        else
        {
            target = GetComponent<TargetComponent>().target;
        }
    }

    public HashSet<KeyValuePair<string, object>> GetWorldState()
    {
        HashSet<KeyValuePair<string, object>> worldData = new HashSet<KeyValuePair<string, object>>();

        //target
        //worldData.Add();
        worldData.Add(new KeyValuePair<string, object>("hasEnoughHealth", health > 20));
        worldData.Add(new KeyValuePair<string, object>("hasTarget", target != null));
        //list of targets in range
        //health
        //

        return worldData;
    }

    public HashSet<KeyValuePair<string, object>> CreateGoalState()
    {
        HashSet<KeyValuePair<string, object>> goal = new HashSet<KeyValuePair<string, object>>(); //dummy

        goal.Add(new KeyValuePair<string, object>("KillEnemy", true));

        return goal; //dummy
    }

    public void PlanFailed(HashSet<KeyValuePair<string, object>> failedGoal)
    {
        Debug.Log("Buddy: The goal: " + failedGoal + " has failed.");
    }

    public void PlanFound(HashSet<KeyValuePair<string, object>> goal, Queue<GOAPAction> actions)
    {
        Debug.Log("Buddy: A plan had been found. The goal is: " + goal + " with the following actions to get to that goal: " + actions);
    }

    public void ActionsFinished()
    {
        Debug.Log("Buddy: The actions have been finished.");
    }

    public void PlanAborted(GOAPAction aborter)
    {
        Debug.Log("Buddy: Plan has been aborted.");
    }

    public bool MoveAgent(GOAPAction nextAction)
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(this.transform.position, nextAction.target.transform.position, step);

        if (transform.position.Equals(nextAction.target.transform.position))
        {
            nextAction.SetInRange(true);
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool ReceiveDamage(float attackPower)
    {
        health -= attackPower;
        if (health <= 0)
        {
            OnDeath();
            return true;
        }
        return false;
    }

    public void OnDeath()
    {
        Debug.Log("Enemy has died :)");
        Destroy(gameObject);
        gameObject.SetActive(false);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool RecoverHealth()
    {
        health += 1;

        return true;
    }
}
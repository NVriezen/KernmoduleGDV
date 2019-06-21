using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buddy : /*MonoBehaviour,*/ IAttacker, IGOAP, IDamagable
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float health = 100;
    [SerializeField] private float attackingPower = 10;
    [SerializeField] private GameObject target;
    [SerializeField] private float healthRecoveryValue = 2;

    public void Awake()
    {
        attackPower = attackingPower;
        if (GetComponent<TargetComponent>() == null)
        {
            target = null;
        } else
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
        } else
        {
            return false;
        }
    }

    public bool ReceiveDamage(Object caller, float attackPower)
    {
        health -= attackPower;
        UserNotifier.instance.UpdateBuddyHealth(health);
        if (health <= 0)
        {
            OnDeath();
            return true;
        }
        return false;
    }

    public void OnDeath()
    {
        Debug.Log("Buddy has died :(");
        gameObject.SetActive(false);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool RecoverHealth()
    {
        health += healthRecoveryValue;

        return true;
    }
}

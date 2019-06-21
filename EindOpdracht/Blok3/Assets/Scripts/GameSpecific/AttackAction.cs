using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : GOAPAction
{
    private bool attacked = false;
    private bool targetKilled = false;

    public AttackAction()
    {
        AddPrecondition("hasTarget",  true);
        AddEffect("hasAttacked", true);
        AddEffect("KillEnemy", true);
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        //should be in range of target
        //must be able to attack
        return true;
    }

    public override bool IsDone()
    {
        return targetKilled;
    }

    public override bool Perform(GameObject agent)
    {
        GameObject target = agent.GetComponent<TargetComponent>().target;

        if (target.GetComponent<IDamagable>().ReceiveDamage(this.gameObject, agent.GetComponent<IAttacker>().attackPower))
        {
            targetKilled = true;
        }
        //Play cool animation and stuff
        attacked = true;

        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        attacked = false;
        targetKilled = false;
    }
}

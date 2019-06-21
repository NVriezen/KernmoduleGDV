using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNode : BaseNode
{
    //BehaviourBoard mainBoard;
    private List<IDamagable> availableTargets = new List<IDamagable>();
    private IDamagable target;
    private IAttacker attackStats;

    private void Start()
    {
        attackStats = GetComponent<IAttacker>();
    }

    private void GetAvailableTargets()
    {
        availableTargets = new List<IDamagable>();

        List<GameObject> targetGameObjectList = GetComponent<ITargets>().getTargets();
        //targetGameObjectList.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position) < Vector3.Distance(transform.position, y.transform.position) ? -1 : 1);

        foreach (GameObject gameObject in targetGameObjectList)
        {
            availableTargets.Add(gameObject.GetComponent<IDamagable>());
        }
    }

    public override state Tick()
    {
        if (availableTargets.Count == 0)
        {
            GetAvailableTargets();
        }
        
        if (target == null)
        {
            List<IDamagable> newTargetsList = new List<IDamagable>();
            foreach (IDamagable damagable in availableTargets)
            {
                if (damagable == null)
                {
                    continue;
                }
                newTargetsList.Add(damagable);
            }

            foreach (IDamagable damagable in availableTargets)
            {
                target = damagable;
                break;
            }
            availableTargets = newTargetsList;

            if (target == null)
            {
                return state.failed;
            }
        }
        
        bool result = target.ReceiveDamage(this.gameObject, attackStats.attackPower);
        if (result)
        {
            availableTargets.Remove(target);
            target = null;
        }

        return state.succes;
        //else
        //{
        //    return state.running;
        //}

        //return state.failed;
    }
}

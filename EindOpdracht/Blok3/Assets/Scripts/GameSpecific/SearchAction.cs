using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchAction : GOAPAction
{
    private ITargetable actionTarget = null;
    public int actionInterval = 2;
    public float searchRadius = 1;

    public SearchAction()
    {
        //AddPrecondition("", );
        AddEffect("hasTarget", true);
    }

    public override bool CheckProceduralPrecondition(GameObject agent)
    {
        //in future: check if not already attacking or doing something else
        return true;
    }

    public override bool IsDone()
    {
        return actionTarget != null;
    }

    public override bool Perform(GameObject agent)
    {
        if (Time.frameCount % actionInterval == 0)
        {
            Collider[] objectsInRange = Physics.OverlapSphere(agent.transform.position, searchRadius);
            foreach (Collider c in objectsInRange)
            {
                if (c.GetComponent<ITargetable>() != null)
                {
                    actionTarget = c.GetComponent<ITargetable>();
                    return true;
                }
            }
        }
        return false;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        actionTarget = null;
    }
}

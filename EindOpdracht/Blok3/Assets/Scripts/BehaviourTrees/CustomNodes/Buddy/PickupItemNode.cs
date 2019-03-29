using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItemNode : BaseNode
{    
    List<GameObject> availableTargets;
    GameObject target;

    private void Start()
    {
        availableTargets = GetComponent<ITargets>().getTargets();
    }

    public override state Tick()
    {
        foreach (GameObject gameObject in availableTargets)
        {
            if (gameObject.GetComponent<IDamagable>() != null)
            {
                if (target == null)
                {
                    target = gameObject;
                }
            }
        }

        return state.failed;
    }
}

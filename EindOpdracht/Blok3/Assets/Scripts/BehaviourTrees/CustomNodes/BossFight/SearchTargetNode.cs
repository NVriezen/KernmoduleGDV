using System.Collections.Generic;
using UnityEngine;

public class SearchTargetNode : BaseNode, ITargets
{
    private Transform target;

    public override state Tick()
    {
        if (target != null)
        {
            return state.succes;
        }

        target = GameObject.FindObjectOfType<Player>().transform;

        if (target == null)
        {
            return state.failed;
        }

        return state.succes;
    }

    public List<GameObject> getTargets()
    {
        if (target == null)
        {
            return null;
        }

        return new List<GameObject>()
        {
            target.gameObject
        };
    }
}

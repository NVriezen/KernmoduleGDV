using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a list of objects nearby.
/// The list is ordered by distance.
/// </summary>
public class SearchNearbyNode : BaseNode, ITargets
{
    //private Transform target;
    public List<GameObject> targets;
    public float searchRadius = 1;


    public override state Tick()
    {
        Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, searchRadius);

        if (hitColliders.Length == 0)
        {   
            return state.failed;
        }

        targets = new List<GameObject>();

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject == this.gameObject)
            {
                continue;
            }
            //if (!targets.Contains(collider.gameObject))
            //{
            targets.Add(collider.gameObject);
            //}
        }

        if (targets.Count == 0)
        {
            return state.failed;
        }

        targets.Sort((x, y) => Vector3.Distance(transform.position, x.transform.position) < Vector3.Distance(transform.position, y.transform.position) ? -1 : 1);

        return state.succes;
    }

    public List<GameObject> getTargets()
    {
        return targets;
    }
}

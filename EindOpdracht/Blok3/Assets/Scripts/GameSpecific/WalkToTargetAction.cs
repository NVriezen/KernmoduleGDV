//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class WalkToTargetAction : GOAPAction
//{
//    //public int actionInterval = 2;
//    //public float searchRadius = 1;

//    //public WalkToTargetAction()
//    //{
//    //    AddPrecondition("hasTarget", true);
//    //    AddEffect("inRange", true);
//    //}

//    //public override bool CheckProceduralPrecondition(GameObject agent)
//    //{
        
//    //    return true;
//    //}

//    //public override bool IsDone()
//    //{
//    //    return true;
//    //}

//    //public override bool Perform(GameObject agent)
//    //{
//    //    if (Time.frameCount % actionInterval == 0)
//    //    {
//    //        Collider[] objectsInRange = Physics.OverlapSphere(agent.transform.position, searchRadius);
//    //        foreach (Collider c in objectsInRange)
//    //        {
//    //            if (c.GetComponent<ITargetable>() != null)
//    //            {
//    //                actionTarget = c.GetComponent<ITargetable>();
//    //                return true;
//    //            }
//    //        }
//    //    }
//    //    return false;
//    //}

//    //public override bool RequiresInRange()
//    //{
//    //    return false;
//    //}

//    //public override void Reset()
//    //{
//    //    actionTarget = null;
//    //}
//}

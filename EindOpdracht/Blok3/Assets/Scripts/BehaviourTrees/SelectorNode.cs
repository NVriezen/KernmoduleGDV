using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorNode : CompositeNode
{
    public List<BaseNode> selectorNodes = new List<BaseNode>();

    public override state Tick()
    {
        if (childStatus == state.running)
        {
            childStatus = runningNode.Tick();

            if (childStatus == state.succes) //TODO: Check if this is correct for selector node
            {
                lastRunningNode = -1;
            }

            return childStatus;
        }

        for (int i = lastRunningNode + 1; i < selectorNodes.Count; i++)
        {
            childStatus = selectorNodes[i].Tick();

            if (childStatus == state.running)
            {
                runningNode = selectorNodes[i];
                lastRunningNode = i;
                return state.running;
            }
            else if (childStatus == state.succes)
            {
                return state.succes;
            }
        }

        lastRunningNode = -1;


        //foreach (BaseNode baseNode in selectorNodes)
        //{
        //    childStatus = baseNode.Tick();

        //    if (childStatus == state.running)
        //    {
        //        runningNode = baseNode;
        //        return state.running;
        //    } else if (childStatus == state.succes)
        //    {
        //        return state.succes;
        //    }
        //}

        return state.failed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    public List<BaseNode> sequenceNodes = new List<BaseNode>();

    //private int lastRunningNode = -1;

    public override state Tick()
    {
        if (childStatus == state.running)
        {
            childStatus = runningNode.Tick();
            return childStatus;
        }

        for (int i = lastRunningNode + 1; i < sequenceNodes.Count; i++)
        {
            childStatus = sequenceNodes[i].Tick();

            if (childStatus == state.running)
            {
                runningNode = sequenceNodes[i];
                lastRunningNode = i;
                return state.running;
            }
            else if (childStatus == state.failed)
            {
                return state.failed;
            }
        }

        lastRunningNode = -1;

        return state.succes;
    }
}

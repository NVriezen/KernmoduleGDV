using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallelNode : CompositeNode
{
    public List<BaseNode> parallelNodes = new List<BaseNode>();

    public override state Tick()
    {
        state finalState = state.failed;
        foreach (BaseNode baseNode in parallelNodes)
        {
            state childStatus = baseNode.Tick();

            if (childStatus == state.running)
            {
                runningNode = baseNode;
                finalState = childStatus;
            } else if (childStatus == state.succes)
            {
                if (finalState != state.running)
                {
                    finalState = childStatus;
                }
            }
        }

        return finalState;
    }
}

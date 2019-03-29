using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : BaseNode
{
    [HideInInspector]
    public BaseNode runningNode;

    [HideInInspector]
    public state childStatus;

    [HideInInspector]
    public int lastRunningNode = -1;
}

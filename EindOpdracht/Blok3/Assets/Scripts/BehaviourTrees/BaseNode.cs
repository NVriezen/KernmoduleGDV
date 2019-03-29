using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : MonoBehaviour
{
    public enum state {
        failed = 0,
        running = 1,
        succes = 2

    };

    public abstract state Tick();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    [SerializeField] private int playerNumber;
    public int PlayerNumber
    {
        get
        {
            return playerNumber;
        }
    }
}

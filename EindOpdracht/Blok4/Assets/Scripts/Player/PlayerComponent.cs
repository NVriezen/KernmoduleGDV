using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerComponent : MonoBehaviour
{
    private int playerID;
    public int PlayerID
    {
        get
        {
            return playerID;
        }
    }
    private string playerName;
    public string PlayerName
    {
        get
        {
            return playerName;
        }
    }

    private int maximumSteps = 2;
    private List<PowerUp> obtainedPowerups = new List<PowerUp>();
    private int stepsTaken = 0;

    public bool SpawnPlayer(int playerID, string playerName)
    {
        this.playerID = playerID;
        this.playerName = playerName;

        return true;
    }

}

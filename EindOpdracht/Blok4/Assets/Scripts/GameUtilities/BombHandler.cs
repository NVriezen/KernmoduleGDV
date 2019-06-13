using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHandler : MonoBehaviour
{
    private Dictionary<uint, BombBase> currentActiveBombs = new Dictionary<uint, BombBase>();
    private uint currentIDNumber = 0;

    public uint SpawnBomb(BombBase bomb)
    {
        currentIDNumber += 1;
        if (currentActiveBombs.ContainsKey(currentIDNumber))
        {
            return SpawnBomb(bomb);
        }
        currentActiveBombs.Add(currentIDNumber, bomb);
        return currentIDNumber;
    }
    
    public void UpdateTurnsPassed()
    {
        uint[] bombIDList = new uint[currentActiveBombs.Count];
        currentActiveBombs.Keys.CopyTo(bombIDList, 0);
        foreach (uint bombID in bombIDList)
        {
            if (currentActiveBombs[bombID].UpdateGetStatus())
            {
                ExplodeBomb(bombID);
            }
        }
    }

    private void ExplodeBomb(uint bombID)
    {
        BombBase bomb = currentActiveBombs[bombID];
        currentActiveBombs.Remove(bombID);
        GameStarter.instance.BombExploded(bombID, bomb.HittedObjects());
    }
}

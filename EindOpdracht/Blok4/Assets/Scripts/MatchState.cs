using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchState
{
    private uint matchID;
    private TurnHandler turnHandler;
    private BombHandler bombHandler;
    private Dictionary<uint, PlayerMatchInfo> playersMatchInfo;

    private uint standardAmountOfLives = 3;

    public void Init(List<uint> playersList)
    {
        turnHandler = new TurnHandler();
        turnHandler.currentActivePlayer = playersList[0];
        bombHandler = new BombHandler();
        playersMatchInfo = new Dictionary<uint, PlayerMatchInfo>();

        foreach (uint player in playersList)
        {
            PlayerMatchInfo playersInfo = new PlayerMatchInfo()
            {
                playerID = player,
                lives = standardAmountOfLives
            };
            playersMatchInfo.Add(player, playersInfo);
        }
    }

    public uint GetTotalTurns()
    {
        return turnHandler.totalTurnsPassed;
    }

    public uint SpawnBomb(BombBase bomb)
    {
        return bombHandler.SpawnBomb(bomb);
    }

    public void SetPlayerReady(uint playerID)
    {
        playersMatchInfo[playerID].gameReady = true;
    }

    public List<uint> GetReadyPlayers()
    {
        List<uint> readyPlayers = new List<uint>();
        foreach (uint player in playersMatchInfo.Keys)
        {
            if (playersMatchInfo[player].gameReady)
            {
                readyPlayers.Add(player);
            }
        }
        return readyPlayers;
    }

    public List<uint> GetPlayerLives()
    {
        List<uint> result = new List<uint>();
        foreach (uint player in playersMatchInfo.Keys)
        {
            result.Add(playersMatchInfo[player].lives);
        }
        return result;
    }

    public uint GetPlayerSteps(uint playerID)
    {
        return playersMatchInfo[playerID].maximumSteps;
    }

    public uint GetCurrentPlayerTurn(bool updateTurn)
    {
        if (updateTurn)
        {
            turnHandler.UpdatePlayerTurn(new List<uint>(playersMatchInfo.Keys));
            bombHandler.UpdateTurnsPassed();
        }
        return turnHandler.currentActivePlayer;
    }

    public void SkipPlayer(uint player)
    {
        turnHandler.playersToSkip.Add(player);
    }

    public void PlayerHit(uint playerID)
    {
        if (playersMatchInfo[playerID].lives < 1)
        {
            return;
        }
        playersMatchInfo[playerID].lives -= 1;
    }

    public uint GameOver()
    {
        uint playerWon = 0;
        foreach (uint player in playersMatchInfo.Keys)
        {
            if (playersMatchInfo[player].lives != 0)
            {
                playerWon = player;
                break;
            }
        }
        return playerWon;
    }
}

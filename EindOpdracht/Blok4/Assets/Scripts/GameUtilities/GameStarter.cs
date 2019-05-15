using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject[] playerObjects;
    private List<uint> playersList;

    //bomb objectpool??

    public void SetupGame(Server server, uint matchID)
    {
        Debug.Log("Retrieving match info");
        playersList = PlayersManager.activeMatches[matchID];
        SpawnPosition[] spawnPoints = FindObjectsOfType<SpawnPosition>();
        for(int i = 0; i < playersList.Count; i++)
        {
            for (int j = 0; j < spawnPoints.Length; j++)
            {
                if (spawnPoints[j].PlayerNumber == i)
                {
                    Debug.Log("Spawning player");
                    SpawnPlayer(server, playersList[i], spawnPoints[j]);
                    break;
                }
            }
        }
    }

    public void SpawnPlayer(Server server, uint spawnplayerID, SpawnPosition spawnPosition)
    {
        Instantiate(playerPrefab, spawnPosition.transform.position, spawnPosition.transform.rotation);
        Debug.Log("Player has been spawned");
        //Set some UI stuff

        PlayerRespawnStruct dataStruct = new PlayerRespawnStruct()
        {
            playerID = spawnplayerID,
            positionX = spawnPosition.transform.position.x,
            positionY = spawnPosition.transform.position.y,
            positionZ = spawnPosition.transform.position.z

        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_RESPAWN, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.StandbyPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }
}

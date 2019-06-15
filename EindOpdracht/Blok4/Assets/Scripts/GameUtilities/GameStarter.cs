using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    public static GameStarter instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LayerMask unwalkableLayer;
    private GameObject[] playerObjects;
    private List<uint> playersList;

    private MatchState match;
    private Server server;

    private List<GameObject> walkableFieldObjects = new List<GameObject>(); //replace with object pool
    private SpawnPosition[] spawnPoints;
    private AStar aStar;
    private float moveSpeed = 10;

    public void SetupGame(Server server, uint matchID)
    {
        instance = this;
        this.server = server;
        Debug.Log("Retrieving match info");
        playersList = PlayersManager.activeMatches[matchID];
        playerObjects = new GameObject[playersList.Count];
        aStar = GetComponentInChildren<AStar>();

        match = new MatchState();
        match.Init(playersList);

        //check if scene loaded with all players
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.REQUEST_LEVEL_LOADED))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }

        spawnPoints = FindObjectsOfType<SpawnPosition>();
        foreach (uint player in playersList)
        {
            for (int j = 0; j < spawnPoints.Length; j++)
            {
                if (spawnPoints[j].PlayerNumber == playersList.IndexOf(player))
                {
                    Debug.Log("Spawning player");
                    //SpawnPlayer(server, playersList[i], spawnPoints[j]);
                    playerObjects[playersList.IndexOf(player)] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    //playerObjects[playersList.IndexOf(player)] = Instantiate(playerPrefab, spawnPoints[j].transform.position, spawnPoints[j].transform.rotation);
                    playerObjects[playersList.IndexOf(player)].transform.position = spawnPoints[j].transform.position;
                    playerObjects[playersList.IndexOf(player)].layer = 12;
                    break;
                }
            }
        }
    }

    public void SpawnAllPlayers(NetworkConnection requester)
    {
        for (int i = 0; i < playersList.Count; i++)
        {
            for (int j = 0; j < spawnPoints.Length; j++)
            {
                if (spawnPoints[j].PlayerNumber == i)
                {
                    Debug.Log("Spawning player");
                    SpawnPlayer(server, playersList[i], spawnPoints[j], requester);
                    break;
                }
            }
        }

        //foreach (uint player in playersList)
        //{
        //    SpawnPlayer(server, player, playerObjects[playersList.IndexOf(player)].transform.position);
        //}
    }

    public void SendUIState()
    {
        //send Lives
        //send total turns
        //send player names in player order
        UIStateUpdateStruct UIStruct = new UIStateUpdateStruct()
        {
            totalTurns = match.GetTotalTurns(),
            amountOfPlayers = (uint)playersList.Count,
            playerLives = match.GetPlayerLives()
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.UI_STATE_UPDATE, UIStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }

        Debug.LogWarning("UI info send");
        //PlayerNamesStruct namesStruct = new PlayerNamesStruct()
        //{
        //    playerNames = new List<string>()
        //    {
        //        { "Player1" },
        //        { "Player2" }
        //    }
        //};
        //using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_NAMES, namesStruct))
        //{
        //    foreach (uint player in playersList)
        //    {
        //        PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
        //    }
        //}

        
    }

    public void CheckGameReady(NetworkConnection connection = default)
    {
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.REQUEST_GAME_READY))
        {
            if (connection != default)
            {
                connection.Send(server.m_Driver, writer);
                return;
            }
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }

    public void SetPlayerReady(uint playerID)
    {
        if (playerID == 0)
        {
            Debug.LogError("Received Game Ready from Non-Connected-Player");
            return;
        }
        match.SetPlayerReady(playerID);

        if (match.GetReadyPlayers().Count == playersList.Count)
        {
            SendCurrentPlayerTurn(false);
        }
    }

    public void SendCurrentPlayerTurn(bool updateTurn = true)
    {
        TurnUpdateStruct dataStruct = new TurnUpdateStruct()
        {
            totalTurns = match.GetTotalTurns(),
            playerID = match.GetCurrentPlayerTurn(updateTurn)
        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.TURN_UPDATE, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }

        SendWalkableField();
    }

    public void SpawnPlayer(Server server, uint spawnplayerID, SpawnPosition spawnPosition, NetworkConnection requester)
    {
        PlayerRespawnStruct dataStruct = new PlayerRespawnStruct()
        {
            playerID = spawnplayerID,
            positionX = spawnPosition.transform.position.x,
            positionY = spawnPosition.transform.position.y,
            positionZ = spawnPosition.transform.position.z

        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_RESPAWN, dataStruct))
        {
            requester.Send(server.m_Driver, writer);
        }
    }

    public void SendWalkableField()
    {
        uint playerID = match.GetCurrentPlayerTurn(false);
        int playerIndex = playersList.IndexOf(playerID);
        Vector3 playerPosition = playerObjects[playerIndex].transform.position;
        uint steps = match.GetPlayerSteps(playerID);

        List<Vector3> walkablePositionsX = new List<Vector3>();
        List<Vector3> walkablePositionsY = new List<Vector3>();

        int y = 0;
        int x = 0;
        for (x = (int)-steps; x < steps; x++)
        {
            if (x == 0)
            {
                continue;
            }
            Vector3 checkingPosition = playerPosition + new Vector3(x, playerPosition.y, y);
            if (!IsTileUnwalkable(checkingPosition))
            {
                walkablePositionsX.Add(checkingPosition);
                continue;
            }
            if (x < 0)
            {
                walkablePositionsX.Clear();
                continue;
            }
            break;
        }

        x = 0;
        for (y = (int)-steps; y < steps; y++)
        {
            if (y == 0)
            {
                continue;
            }
            Vector3 checkingPosition = playerPosition + new Vector3(x, playerPosition.y, y);
            if (!IsTileUnwalkable(checkingPosition))
            {
                walkablePositionsY.Add(checkingPosition);
                continue;
            }
            if (y < 0)
            {
                walkablePositionsY.Clear();
                continue;
            }
            break;
        }

        List<Vector3> resultingPositions = walkablePositionsX;
        foreach (Vector3 position in walkablePositionsY)
        {
            resultingPositions.Add(position);
        }
        resultingPositions.Add(playerPosition);

        foreach (GameObject walkObject in walkableFieldObjects)
        {
            Destroy(walkObject);
        }
        walkableFieldObjects.Clear();

        foreach (Vector3 position in resultingPositions)
        {
            GameObject fieldObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            walkableFieldObjects.Add(fieldObject);
            fieldObject.transform.position = position;
            fieldObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 128, 128);
            fieldObject.layer = 11;
        }

        WalkableFieldUpdateStruct dataStruct = new WalkableFieldUpdateStruct()
        {
            centerPosition = playerPosition,
            walkablePositions = resultingPositions,
            listLength = (uint)resultingPositions.Count,
        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.WALKABLE_FIELD_UPDATE, dataStruct))
        {
            PlayersManager.connectedPlayers[playerID].Send(server.m_Driver, writer);
        }
    }

    public bool IsTileUnwalkable(Vector3 tilePosition)
    {
        if (Physics.CheckBox(tilePosition, new Vector3(0.4f, 0.4f, 0.4f), Quaternion.identity, unwalkableLayer))
        {
            return true;
        }
        return false;
    }

    public void PlayerMove(uint playerID, Vector3 newPosition)
    {
        if (match.GetCurrentPlayerTurn(false) != playerID)
        {
            return;
        }
        //check if position is walkable position
        if (!Physics.CheckBox(newPosition, new Vector3(0.1f, 0.1f, 0.1f), Quaternion.identity, 1 << 11))
        {
            return;
        }
        StartCoroutine(LookForPath(playerID, playerObjects[playersList.IndexOf(playerID)].transform.position, newPosition));
    }

    public void SpawnBomb(uint playerID)
    {
        if (match.GetCurrentPlayerTurn(false) != playerID)
        {
            return;
        }
        GameObject newBomb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newBomb.transform.position = playerObjects[playersList.IndexOf(playerID)].transform.position;
        NormalBomb bombComponent = newBomb.AddComponent<NormalBomb>();
        uint placedBombID = match.SpawnBomb(bombComponent);

        BombSpawnStruct dataStruct = new BombSpawnStruct()
        {
            bombID = placedBombID,
            bombPosition = newBomb.transform.position,
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.BOMB_SPAWN, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }

    //private void SendBombSpawn(Vector3 bombPosition)
    //{
    //    BombSpawnStruct dataStruct = new BombSpawnStruct()
    //    {
    //        bombPosition = bombPosition,
    //    };

    //    using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.BOMB_SPAWN, dataStruct))
    //    {
    //        foreach (uint player in playersList)
    //        {
    //            PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
    //        }
    //    }
    //}

    public void BombExploded(uint bombID, List<Vector3>[] hitInfo)
    {
        CheckForPlayerHit(hitInfo[1]);
        CheckForCrateHit(hitInfo[1]);
        BombExplodeStruct dataStruct = new BombExplodeStruct()
        {
            bombID = bombID,
            amountOfData = (uint)hitInfo[0].Count,
            flamePositions = hitInfo[0],
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.BOMB_EXPLODE, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }

    public void CheckForPlayerHit(List<Vector3> hitPositions)
    {
        int i = 0;
        Debug.Log("amount of hitPosition - " + hitPositions.Count);
        foreach (Vector3 hitPosition in hitPositions) {
            foreach (uint player in playersList)
            {
                float distance = Vector3.Distance(hitPosition, playerObjects[playersList.IndexOf(player)].transform.position);
                if (distance >= 0.3f)
                {
                    continue;
                }

                match.SkipPlayer(player);
                //send message to display skipping of player
                Debug.Log("lives update is calles " + ++i + " specific position is - " + hitPosition);
                UpdateLives(player);
                if (match.GetPlayerLives()[playersList.IndexOf(player)] == 0)
                {
                    GameOver();
                    return;
                }
            }
        }
    }

    public void CheckForCrateHit(List<Vector3> hitPositions)
    {
        List<Vector3> destroyableCrates = new List<Vector3>();
        foreach (Vector3 hitPosition in hitPositions)
        {
            if (CrateManager.instance.DestroyCrate(hitPosition))
            {
                aStar.UpdateTile(hitPosition);
                destroyableCrates.Add(hitPosition + new Vector3(0,0.5f,0));
            }
        }

        if (destroyableCrates.Count == 0)
        {
            return;
        }

        CrateDestroyStruct dataStruct = new CrateDestroyStruct()
        {
            amountOfData = (uint)destroyableCrates.Count,
            cratesToDestroy = destroyableCrates,
        };
        Debug.Log(dataStruct.cratesToDestroy.Count + " First element " + dataStruct.cratesToDestroy[0]);
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.CRATE_DESTROY, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }

    public void GameOver()
    {
        uint playerWon = match.GameOver();

        GameOverStruct dataStruct = new GameOverStruct() { playerWon = playerWon };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.GAME_OVER, dataStruct))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
        StartCoroutine(DestroyMatch());
    }

    private IEnumerator LookForPath(uint playerID, Vector3 startPosition, Vector3 endPosition)
    {
        List<Vector3> resultPath = aStar.FindPath(startPosition, endPosition);
        if (resultPath == null)
        {
            yield break;
        }

        PlayerPositionUpdateStruct dataStruct = new PlayerPositionUpdateStruct()
        {
            playerID = playerID,
            walkablePath = resultPath,
            amountOfData = (uint)resultPath.Count,
        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_POSITION_UPDATE, dataStruct))
        {
            foreach (uint player in playersList) {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }

        StartCoroutine(MovePlayerOnPath(playerObjects[playersList.IndexOf(playerID)].transform, resultPath));

        yield return null;
    }

    private IEnumerator MovePlayerOnPath(Transform objectToMove, List<Vector3> pathToFollow)
    {
        Debug.Log("All info: " + pathToFollow.Count + " - " + pathToFollow[0] + " - " + objectToMove.name);
        for (int i = 0; i < pathToFollow.Count; i++)
        {
            Debug.Log("Inside for loop");
            while (objectToMove.position != pathToFollow[i])
            {
                Debug.Log("Inside while loop");
                float step = moveSpeed * Time.deltaTime;
                objectToMove.position = Vector3.MoveTowards(objectToMove.position, pathToFollow[i], step);
                yield return null;
            }
            yield return null;
        }
    }

    public void EndTurn(uint playerID)
    {
        if (match.GetCurrentPlayerTurn(false) != playerID)
        {
            return;
        }
        SendCurrentPlayerTurn(true);
        SendUIState();
    }

    public void UpdateLives(uint playerID)
    {
        match.PlayerHit(playerID);
        SendUIState();
    }

    private IEnumerator DestroyMatch()
    {
        yield return new WaitForSeconds(8);
        match = null;
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.RETURN_TO_MENU))
        {
            foreach (uint player in playersList)
            {
                PlayersManager.connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}

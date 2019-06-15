using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum ConnectionEvent
{
    PING = 0,
    PONG = 1,
    PLAYER_CONNECT,             // username (client -> server)
    PLAYER_DISCONNECT,          // player id (client -> server)
    REQUEST_OPPONENT,           // player id (client -> server)
    ASSIGN_OPPONENT,            // bool (server -> client)
    START_GAME,                 // playingfield id, players in game, [player id, character id] (server -> client)
    REQUEST_LEVEL_LOADED,
    DONE_LOADING_LEVEL,
    REQUEST_GAME_READY,
    GAME_READY,
    UI_STATE_UPDATE,
    PLAYER_NAMES,
    PLAYER_POSITION_UPDATE,     // player id, position, rotation (server -> client)
    BOMB_SPAWN,                 // position (server -> client)
    BOMB_POSITION_UPDATE,       // bomb id, position (server -> client)
    BOMB_STATE_UPDATE,          // bomb id, bomb state (server -> client) Maybe add bomb blast radius
    CRATE_DESTROY,              // List<position> (server -> client)
    ITEM_SPAWN,                 // item id, item type, position (server -> client)
    ITEM_UPDATE,                // item id, item type, player id (server -> client)
    LIVES_UPDATE,               // playerCount, [player id, lives] (server - client)
    TURN_UPDATE,                // turnCount, player id (server -> client)
    PLAYER_MOVE,                // player id, position (client -> server)
    PLAYER_PLACE_BOMB,          // player id, position (client -> server)
    PLAYER_END_TURN,            // player id (client -> server)
    PLAYER_RESPAWN,             // player id, position (server -> client)
    WALL_SPAWN,                 // position (server -> client)
    WALKABLE_FIELD_UPDATE,
    BOMB_EXPLODE,
    GAME_OVER,
    RETURN_TO_MENU,
}

public static class MessageCenter
{
    public delegate void ReadEventFunction(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection);
    public delegate DataStreamWriter WriteEventFunction(IMessageStruct messageStruct);

    public static Dictionary<ConnectionEvent, ReadEventFunction> ReadDictionary => new Dictionary<ConnectionEvent, ReadEventFunction>()
    {
        { ConnectionEvent.PING, ReadPing },
        { ConnectionEvent.PONG, ReadPong },
        { ConnectionEvent.PLAYER_CONNECT, ReadPlayerConnect },
        { ConnectionEvent.REQUEST_OPPONENT, ReadRequestOpponent },
        { ConnectionEvent.ASSIGN_OPPONENT, ReadAssignOpponent },
        { ConnectionEvent.START_GAME, ReadStartGame},
        { ConnectionEvent.REQUEST_LEVEL_LOADED, ReadRequestLevelLoaded },
        { ConnectionEvent.PLAYER_RESPAWN, ReadPlayerRespawn },
        { ConnectionEvent.DONE_LOADING_LEVEL, ReadDoneLoadingLevel },
        { ConnectionEvent.UI_STATE_UPDATE, ReadUIStateUpdate },
        { ConnectionEvent.PLAYER_POSITION_UPDATE, ReadPlayerPositionUpdate },
        { ConnectionEvent.REQUEST_GAME_READY, ReadRequestGameReady },
        { ConnectionEvent.GAME_READY, ReadGameReady },
        { ConnectionEvent.TURN_UPDATE, ReadTurnUpdate },
        { ConnectionEvent.PLAYER_PLACE_BOMB, ReadPlayerPlaceBomb },
        { ConnectionEvent.PLAYER_MOVE, ReadPlayerMove },
        { ConnectionEvent.PLAYER_END_TURN, ReadPlayerEndTurn },
        { ConnectionEvent.WALKABLE_FIELD_UPDATE, ReadWalkableFieldUpdate },
        { ConnectionEvent.BOMB_EXPLODE, ReadBombExplode },
        { ConnectionEvent.BOMB_SPAWN, ReadBombSpawn },
        { ConnectionEvent.GAME_OVER, ReadGameOver},
        { ConnectionEvent.RETURN_TO_MENU, ReadReturnToMenu },
        { ConnectionEvent.CRATE_DESTROY, ReadCrateDestroy },
    };

    //public static Dictionary<ConnectionEvent, WriteEventFunction> WriteDictionary => new Dictionary<ConnectionEvent, WriteEventFunction>()
    //{
    //    { ConnectionEvent.PING, WritePing },
    //    { ConnectionEvent.PONG, WritePong },
    //    { ConnectionEvent.PLAYER_CONNECT, WritePlayerConnect },
    //    { ConnectionEvent.REQUEST_OPPONENT, WriteRequestOpponent },
    //    { ConnectionEvent.ASSIGN_OPPONENT, WriteAssignOpponent },
    //    { ConnectionEvent.START_GAME, WriteStartGame },
    //    { ConnectionEvent.PLAYER_RESPAWN, WritePlayerRespawn},
    //    { ConnectionEvent.PLAYER_PLACE_BOMB, WritePlayerPlaceBomb },
    //};

    //public static Dictionary<ConnectionEvent, IMessageStruct> StructDictionary => new Dictionary<ConnectionEvent, IMessageStruct>()
    //{
    //    { ConnectionEvent.PING, null },
    //    { ConnectionEvent.PONG, null },
    //    { ConnectionEvent.PLAYER_CONNECT, new PlayerConnectStruct() },
    //    { ConnectionEvent.REQUEST_OPPONENT, new RequestOpponentStruct() },
    //    { ConnectionEvent.ASSIGN_OPPONENT, new AssignOpponentStruct() },
    //    { ConnectionEvent.START_GAME, new StartGameStruct() },
    //    { ConnectionEvent.PLAYER_RESPAWN, new PlayerRespawnStruct() },
    //    { ConnectionEvent.PLAYER_PLACE_BOMB, new PlayerPlaceBombStruct() },
    //};

    //public static DataStreamWriter WriteEvent(ConnectionEvent connectionEvent, IMessageStruct messageStruct = default)
    //{
    //    return WriteDictionary[connectionEvent](messageStruct);
    //}

    public static DataStreamWriter WriteEvent(ConnectionEvent connectionEvent, IMessageStruct messageStruct = default)
    {
        DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp);
        writer.Write((uint)connectionEvent);

        if (messageStruct == default)
        {
            return writer;
        }

        Type dataType = messageStruct.GetType();
        int dataCount = dataType.GetFields(BindingFlags.Instance | BindingFlags.Public).Length;
        writer.Capacity = 4 * (dataCount + 2);

        FieldInfo[] fields = dataType.GetFields();
        foreach (FieldInfo field in fields)
        {
            Debug.Log("Type of: " + field.FieldType + "actual thing we want: " + typeof(List<uint>));
            if (field.FieldType == typeof(List<uint>))
            {
                List<uint> listToWrite = (List<uint>)field.GetValue(messageStruct);
                writer.Capacity += 4 * (listToWrite.Count - 1);
                foreach (uint listValue in listToWrite)
                {
                    Debug.Log("Writing: " + listValue);
                    writer.Write(listValue);
                }
                continue;
            }
            if (field.FieldType == typeof(List<Vector3>))
            {
                List<Vector3> listToWrite = (List<Vector3>)field.GetValue(messageStruct);
                writer.Capacity += 4 * (listToWrite.Count * 3);
                foreach (Vector3 listValue in listToWrite)
                {
                    Debug.Log("Writing: " + listValue);
                    writer.Write(listValue.x);
                    writer.Write(listValue.y);
                    writer.Write(listValue.z);
                }
                continue;
            }
            Debug.Log("Writing: " + field.GetValue(messageStruct).GetType() + " - " + field.Name + " : " + dataType.Name);
            if (field.FieldType == typeof(uint))
            {
                writer.Write((uint)field.GetValue(messageStruct));
                continue;
            }
            if (field.FieldType == typeof(float))
            {
                writer.Write((float)field.GetValue(messageStruct));
                continue;
            }
            if (field.FieldType == typeof(Vector3))
            {
                writer.Capacity += 8;
                Vector3 vectorToWrite = (Vector3)field.GetValue(messageStruct);
                writer.Write((float)vectorToWrite.x);
                writer.Write((float)vectorToWrite.y);
                writer.Write((float)vectorToWrite.z);
                continue;
            }
        }
        return writer;
    }

    public static void ReadEvent(ConnectionEvent connectionEvent, object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        ConnectionEvent receivedEvent = (ConnectionEvent)stream.ReadUInt(ref context);
        ReadDictionary[receivedEvent](caller, stream, ref context, connection);

        //IMessageStruct eventStruct = StructDictionary[receivedEvent];
        //Type dataType = eventStruct.GetType();
        //int dataCount = dataType.GetFields(BindingFlags.Instance | BindingFlags.Public).Length;

        //FieldInfo[] fields = dataType.GetFields();
        //foreach (FieldInfo field in fields)
        //{
        //    Debug.Log("Type of: " + field.FieldType + "actual thing we want: " + typeof(List<uint>));
        //    if (field.FieldType == typeof(List<uint>))
        //    {
        //        List<uint> receivedList = new List<uint>();
        //        for (int i = 0; i < (uint)dataType.GetField("amountOfPlayers").GetValue(eventStruct); i++)
        //        {
        //            receivedList.Add(stream.ReadUInt(ref context));
        //        }
        //        field.SetValue(eventStruct, receivedList);
        //        continue;
        //    }
        //    field.SetValue(eventStruct, stream.ReadBytesAsArray(ref context, ));
        //}

        //return eventStruct;
    }

    #region READERS

    private static void ReadPing(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;
        Debug.Log("Ping received");
        DataStreamWriter writer = WriteEvent(ConnectionEvent.PONG);
        connection.Send(client.m_Driver , writer);
        writer.Dispose();
    }

    private static void ReadPong(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Server server = caller as Server;
        Debug.Log("Pong Received");
        DataStreamWriter writer = WriteEvent(ConnectionEvent.PING);
        connection.Send(server.m_Driver, writer);
        writer.Dispose();
    }

    private static void ReadPlayerConnect(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Server server = caller as Server;

        uint playerID = (uint)stream.ReadUInt(ref context);
        if (PlayersManager.connectedPlayers.ContainsKey(playerID))
        {
            return;
        }
        PlayersManager.connectedPlayers.Add(playerID, connection);

        Debug.Log("Player has been added to the online player list");
    }

    private static void ReadRequestOpponent(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Server server = caller as Server;

        //uint matchID = 0;
        uint playerID = (uint)stream.ReadUInt(ref context);
        //KeyValuePair<uint, NetworkConnection> playerInfo = new KeyValuePair<uint, NetworkConnection>(playerID, connection);


        //PlayersManager.connectedPlayers.Remove(playerID);
        PlayersManager.activelySearchingPlayers.Enqueue(playerID);
        //matchID = PlayersManager.CreateMatch(playerInfo);

        server.StartCoroutine(PlayersManager.CreateMatch(playerID, server));
    }

    private static void ReadAssignOpponent(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;

        uint matchID = (uint)stream.ReadUInt(ref context);

        if (matchID == 0)
        {
            Debug.Log("No match created");
            EventManager.TriggerEvent("NO_OPPONENT_FOUND");
            return;
        }

        Debug.Log("Match created with id: " + matchID);
        EventManager.TriggerEvent("OPPONENT_FOUND");
    }

    private static void ReadStartGame(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;

        StartGameStruct dataContainer = new StartGameStruct()
        {
            playingFieldID = stream.ReadUInt(ref context),
            amountOfPlayers = stream.ReadUInt(ref context),
            playerInfoCharacter = new List<uint>(),
            playerInfoID = new List<uint>()
        };

        for (int i = 0; i < dataContainer.amountOfPlayers; i++)
        {
            dataContainer.playerInfoID.Add(stream.ReadUInt(ref context));
        }

        for (int i = 0; i < dataContainer.amountOfPlayers; i++)
        {
            dataContainer.playerInfoCharacter.Add(stream.ReadUInt(ref context));
        }

        //load scene
        client.StartCoroutine(client.StartGame(dataContainer));
        //UnityEngine.SceneManagement.SceneManager.LoadScene("OpenField");
        //place characters on field
        //set correct names
    }

    private static void ReadRequestLevelLoaded(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;

        client.StartCoroutine(client.CheckForSceneLoaded());
    }

    private static void ReadPlayerRespawn(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;

        PlayerRespawnStruct receivedData = new PlayerRespawnStruct()
        {
            playerID = stream.ReadUInt(ref context),
            positionX = stream.ReadFloat(ref context),
            positionY = stream.ReadFloat(ref context),
            positionZ = stream.ReadFloat(ref context),
        };

        //spawn player object
        ClientMatchManager.instance.SpawnPlayerObject(receivedData);
    }

    private static void ReadDoneLoadingLevel(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        GameStarter.instance.SpawnAllPlayers(connection);
        GameStarter.instance.SendUIState();
        GameStarter.instance.CheckGameReady(connection);
    }

    private static void ReadUIStateUpdate(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        UIStateUpdateStruct dataContainer = new UIStateUpdateStruct()
        {
            totalTurns = stream.ReadUInt(ref context),
            amountOfPlayers = stream.ReadUInt(ref context),
            playerLives = new List<uint>()
        };

        Debug.Log("UI DATA IS: " + dataContainer.totalTurns + dataContainer.amountOfPlayers);

        for (int i = 0; i < dataContainer.amountOfPlayers; i++)
        {
            dataContainer.playerLives.Add(stream.ReadUInt(ref context));
            Debug.Log("Player Lives is: " + dataContainer.playerLives[i]);
        }

        ClientMatchManager.instance.UpdateUI(dataContainer);
    }

    private static void ReadPlayerPositionUpdate(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        //read playerid
        uint playerID = stream.ReadUInt(ref context);
        //read amount of data in list
        uint amountOfData = stream.ReadUInt(ref context);
        //read vector3 list
        List<Vector3> resultingPositions = new List<Vector3>();
        for (int i = 0; i < amountOfData; i++)
        {
            float x = stream.ReadFloat(ref context);
            float y = stream.ReadFloat(ref context);
            float z = stream.ReadFloat(ref context);
            resultingPositions.Add(new Vector3(x, y, z));
        }
        //call A* to walk around 
        Debug.Log("Starting following path");
        ClientMatchManager.instance.MovePlayerOnPath(playerID, resultingPositions);
    }

    private static void ReadRequestGameReady(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Client client = caller as Client;

        do
        {
            Debug.Log("Checking for game ready");
        } while (!ClientMatchManager.instance.GetGameReady());

        using (DataStreamWriter writer = WriteEvent(ConnectionEvent.GAME_READY))
        {
            connection.Send(client.m_Driver, writer);
        }
    }

    private static void ReadGameReady(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint playerID = 0;
        foreach (uint player in PlayersManager.connectedPlayers.Keys)
        {
            if (PlayersManager.connectedPlayers[player].Equals(connection))
            {
                playerID = player;
                break;
            }
        }

        GameStarter.instance.SetPlayerReady(playerID);
    }

    private static void ReadTurnUpdate(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        TurnUpdateStruct dataContainer = new TurnUpdateStruct()
        {
            totalTurns = stream.ReadUInt(ref context),
            playerID = stream.ReadUInt(ref context)
        };
        ClientMatchManager.instance.UpdateTurns(dataContainer);
    }

    private static void ReadPlayerPlaceBomb(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint playerID = stream.ReadUInt(ref context);

        GameStarter.instance.SpawnBomb(playerID);
    }

    private static void ReadPlayerMove(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint playerID = stream.ReadUInt(ref context);
        float x = stream.ReadFloat(ref context);
        float y = stream.ReadFloat(ref context);
        float z = stream.ReadFloat(ref context);
        GameStarter.instance.PlayerMove(playerID, new Vector3(x, y, z));
    }

    private static void ReadPlayerEndTurn(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        GameStarter.instance.EndTurn(stream.ReadUInt(ref context));
    }

    private static void ReadWalkableFieldUpdate(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        float x = stream.ReadFloat(ref context);
        float y = stream.ReadFloat(ref context);
        float z = stream.ReadFloat(ref context);
        Vector3 centerPosition = new Vector3(x, y, z);
        List<Vector3> walkablePosition = new List<Vector3>();
        uint amountOfData = stream.ReadUInt(ref context);
        for (int i = 0; i < amountOfData; i++)
        {
            float vx = stream.ReadFloat(ref context);
            float vy = stream.ReadFloat(ref context);
            float vz = stream.ReadFloat(ref context);
            walkablePosition.Add(new Vector3(vx, vy, vz));
        }
        ClientMatchManager.instance.ActivateWalkableField(centerPosition, walkablePosition);
    }

    private static void ReadBombExplode(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint bombID = stream.ReadUInt(ref context);
        uint amountOfData = stream.ReadUInt(ref context);
        List<Vector3> receivedPositions = new List<Vector3>();
        for (int i = 0; i < amountOfData; i++)
        {
            float x = stream.ReadFloat(ref context);
            float y = stream.ReadFloat(ref context);
            float z = stream.ReadFloat(ref context);
            receivedPositions.Add(new Vector3(x, y, z));
        }
        BombExplodeStruct dataContainer = new BombExplodeStruct()
        {
            bombID = bombID,
            flamePositions = receivedPositions,
        };
        ClientMatchManager.instance.SpawnFlames(dataContainer);
    }

    private static void ReadBombSpawn(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint bombID = stream.ReadUInt(ref context);
        float x = stream.ReadFloat(ref context);
        float y = stream.ReadFloat(ref context);
        float z = stream.ReadFloat(ref context);

        ClientMatchManager.instance.SpawnBomb(bombID, new Vector3(x, y, z));
    }

    private static void ReadGameOver(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        GameOverStruct dataContainer = new GameOverStruct()
        {
            playerWon = stream.ReadUInt(ref context),
        };
        ClientMatchManager.instance.GameOver(dataContainer);
    }

    private static void ReadReturnToMenu(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        ClientMatchManager.instance.DestroyMatch();
    }

    private static void ReadCrateDestroy(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        uint amountOfData = stream.ReadUInt(ref context);
        List<Vector3> cratePositions = new List<Vector3>();
        for (int i = 0; i < amountOfData; i++)
        {
            float x = stream.ReadFloat(ref context);
            float y = stream.ReadFloat(ref context);
            float z = stream.ReadFloat(ref context);
            cratePositions.Add(new Vector3(x, y, z));
        }
        CrateDestroyStruct dataContainer = new CrateDestroyStruct()
        {
            amountOfData = amountOfData,
            cratesToDestroy = cratePositions,
        };

        ClientMatchManager.instance.DestroyCrates(dataContainer);
    }

    #endregion

    #region WRITERS

    private static DataStreamWriter WritePing(IMessageStruct data = null)
    {
        uint connectedEvent = (uint)ConnectionEvent.PING;
        DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp);
        writer.Write(connectedEvent);
        Debug.Log("Ping send");
        return writer;
    }

    private static DataStreamWriter WritePong(IMessageStruct data = null)
    {
        uint connectedEvent = (uint)ConnectionEvent.PONG;
        DataStreamWriter writer = new DataStreamWriter(4, Allocator.Temp);
        writer.Write(connectedEvent);
        Debug.Log("Pong send");
        return writer;
    }

    private static DataStreamWriter WritePlayerConnect(IMessageStruct dataToSend)
    {
        DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp);
        writer.Write((uint)ConnectionEvent.PLAYER_CONNECT);
        writer.Write((uint)dataToSend.GetType().GetField("playerID").GetValue(dataToSend));
        return writer;
    }

    private static DataStreamWriter WriteRequestOpponent(IMessageStruct dataToSend)
    {
        DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp);
        writer.Write((uint)ConnectionEvent.REQUEST_OPPONENT);
        writer.Write((uint)dataToSend.GetType().GetField("playerID").GetValue(dataToSend));
        return writer;
    }

    private static DataStreamWriter WriteAssignOpponent(IMessageStruct dataToSend)
    {
        DataStreamWriter writer = new DataStreamWriter(8, Allocator.Temp);
        writer.Write((uint)ConnectionEvent.ASSIGN_OPPONENT);
        writer.Write((uint)dataToSend.GetType().GetField("matchID").GetValue(dataToSend));
        return writer;
    }

    public static DataStreamWriter WriteStartGame(IMessageStruct dataToSend)
    {
        int dataCount = dataToSend.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Length;
        DataStreamWriter writer = new DataStreamWriter(4 * (dataCount + 1), Allocator.Temp);
        writer.Write((uint)ConnectionEvent.START_GAME);

        FieldInfo[] fields = typeof(StartGameStruct).GetFields();
        foreach(FieldInfo field in dataToSend.GetType().GetFields())
        {
            Debug.Log("Type of: " + field.FieldType + "actual thing we want: " + typeof(List<uint>));
            if (field.FieldType == typeof(List<uint>))
            {
                writer.Capacity += 4 * (((List<uint>)field.GetValue(dataToSend)).Count - 1);
                foreach (uint listValue in (List<uint>)field.GetValue(dataToSend))
                {
                    Debug.Log("Writing: " + listValue);
                    writer.Write(listValue);
                }
                continue;
            }
            Debug.Log("Writing: " + (uint)field.GetValue(dataToSend));
            writer.Write((uint)field.GetValue(dataToSend));
        }
        return writer;
    }

    public static DataStreamWriter WritePlayerRespawn(IMessageStruct dataToSend)
    {
        Type data = dataToSend.GetType();
        DataStreamWriter writer = new DataStreamWriter(20, Allocator.Temp);
        writer.Write((uint)ConnectionEvent.PLAYER_RESPAWN);
        writer.Write((uint)data.GetField("playerID").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionX").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionY").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionZ").GetValue(dataToSend));

        return writer;
    }

    public static DataStreamWriter WritePlayerPlaceBomb(IMessageStruct dataToSend)
    {
        Type data = dataToSend.GetType();
        DataStreamWriter writer = new DataStreamWriter(20, Allocator.Temp);
        writer.Write((uint)ConnectionEvent.PLAYER_PLACE_BOMB);
        writer.Write((uint)data.GetField("playerID").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionX").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionY").GetValue(dataToSend));
        writer.Write((float)data.GetField("positionZ").GetValue(dataToSend));

        return writer;
    }
        
    #endregion
}

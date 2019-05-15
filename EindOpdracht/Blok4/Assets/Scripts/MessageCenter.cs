using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;
using Unity.Collections;
using System;
using System.Reflection;

public enum ConnectionEvent
{
    PING = 0,
    PONG = 1,
    PLAYER_CONNECT,             // username (client -> server)
    PLAYER_DISCONNECT,          // player id (client -> server)
    REQUEST_OPPONENT,           // player id (client -> server)
    ASSIGN_OPPONENT,            // bool (server -> client)
    START_GAME,                 // playingfield id, players in game, [player id, character id] (server -> client)
    PLAYER_POSTIION_UPDATE,     // player id, position, rotation (server -> client)
    BOMB_SPAWN,                 // bomb id, bomb type, position (server -> client)
    BOMB_POSITION_UPDATE,       // bomb id, position (server -> client)
    BOMB_STATE_UPDATE,          // bomb id, bomb state (server -> client) Maybe add bomb blast radius
    BOX_UPDATE,                 // box id, box state (server -> client)
    ITEM_SPAWN,                 // item id, item type, position (server -> client)
    ITEM_UPDATE,                // item id, item type, player id (server -> client)
    LIVES_UPDATE,               // playerCount, [player id, lives] (server - client)
    TURN_UPDATE,                // turnCount, player id (server -> client)
    PLAYER_MOVE,                // player id, position (client -> server)
    PLAYER_PLACE_BOMB,          // player id, position (client -> server)
    PLAYER_END_TURN,            // player id (client -> server)
    PLAYER_RESPAWN,             // player id, position (server -> client)
    WALL_SPAWN,                 // position (server -> client)
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
        { ConnectionEvent.PLAYER_RESPAWN, ReadPlayerRespawn },
    };

    public static Dictionary<ConnectionEvent, WriteEventFunction> WriteDictionary => new Dictionary<ConnectionEvent, WriteEventFunction>()
    {
        { ConnectionEvent.PING, WritePing },
        { ConnectionEvent.PONG, WritePong },
        { ConnectionEvent.PLAYER_CONNECT, WritePlayerConnect },
        { ConnectionEvent.REQUEST_OPPONENT, WriteRequestOpponent },
        { ConnectionEvent.ASSIGN_OPPONENT, WriteAssignOpponent },
        { ConnectionEvent.START_GAME, WriteStartGame },
        { ConnectionEvent.PLAYER_RESPAWN, WritePlayerRespawn},
    };

    public static DataStreamWriter WriteEvent(ConnectionEvent connectionEvent, IMessageStruct messageStruct = default)
    {
        return WriteDictionary[connectionEvent](messageStruct);
    }

    #region READERS

    private static void ReadPing(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Debug.Log("Ping received");
        WriteEvent(ConnectionEvent.PONG);
    }

    private static void ReadPong(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Debug.Log("Pong Received");
    }

    private static void ReadPlayerConnect(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Server server = caller as Server;

        uint playerID = (uint)stream.ReadUInt(ref context);
        PlayersManager.StandbyPlayers.Add(playerID, connection);

        Debug.Log("Player has been added to the online player list");
    }

    private static void ReadRequestOpponent(object caller, DataStreamReader stream, ref DataStreamReader.Context context, NetworkConnection connection)
    {
        Server server = caller as Server;

        //uint matchID = 0;
        uint playerID = (uint)stream.ReadUInt(ref context);
        KeyValuePair<uint, NetworkConnection> playerInfo = new KeyValuePair<uint, NetworkConnection>(playerID, connection);


        PlayersManager.StandbyPlayers.Remove(playerID);
        PlayersManager.ActivelySearchingPlayers.Enqueue(playerInfo);
        //matchID = PlayersManager.CreateMatch(playerInfo);

        server.StartCoroutine(PlayersManager.CreateMatch(playerInfo, server, connection));
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("OpenField");
        //place characters on field
        //set correct names
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
        client.SpawnPlayerObject(receivedData);
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
        

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class PlayersManager
{
    //public static Dictionary<int, int> playersList = new Dictionary<int, int>(); // connection id, playerid
    public static Queue<KeyValuePair<uint, NetworkConnection>> ActivelySearchingPlayers = new Queue<KeyValuePair<uint, NetworkConnection>>(); //playerID, connectionID
    public static Dictionary<uint, NetworkConnection> StandbyPlayers = new Dictionary<uint, NetworkConnection>(); //playerID, connectionID

    public static Dictionary<uint, List<uint>> activeMatches = new Dictionary<uint, List<uint>>(); //matchID, List of players
    private static float waitTime = 15;
    private static uint currentMatchID = 0;

    public static IEnumerator CreateMatch(KeyValuePair<uint, NetworkConnection> requestingPlayer, Server server, NetworkConnection connection)
    {
        bool opponentFound = false;
        KeyValuePair<uint, NetworkConnection> opponentInfo = new KeyValuePair<uint, NetworkConnection>();
        float startTime = Time.time;
        float waitedTime = Time.time - startTime;
        while ((ActivelySearchingPlayers.Count <= 1 && waitedTime < waitTime))
        {
            waitedTime = Time.time - startTime;
            yield return null;
        }

        if (waitedTime < waitTime)
        {
            do
            {
                opponentInfo = ActivelySearchingPlayers.Dequeue();
                yield return null;
            } while (opponentInfo.Key == requestingPlayer.Key && StandbyPlayers.ContainsKey(opponentInfo.Key));

            Debug.Log("requesting: " + requestingPlayer.Key + " - " + requestingPlayer.Value);
            Debug.Log("opponent: " + opponentInfo.Key + " - " + opponentInfo.Value);
            StandbyPlayers.Add(requestingPlayer.Key, requestingPlayer.Value);
            StandbyPlayers.Add(opponentInfo.Key, opponentInfo.Value);

            opponentFound = true;
        }

        uint newMatchID = 0;
        List<uint> players = new List<uint>();
        if (opponentFound)
        {
            newMatchID = currentMatchID = currentMatchID + 1;
            activeMatches.Add(newMatchID, new List<uint>() { requestingPlayer.Key, opponentInfo.Key });
            players = activeMatches[newMatchID];

            server.StartCoroutine(server.StartMatch(newMatchID));
        }

        AssignOpponentStruct dataStruct = new AssignOpponentStruct()
        {
            matchID = newMatchID
        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.ASSIGN_OPPONENT, dataStruct))
        {
            if (players.Count == 0)
            {
                connection.Send(server.m_Driver, writer);
                yield break;
            }

            foreach (uint player in players)
            {
                StandbyPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }
}

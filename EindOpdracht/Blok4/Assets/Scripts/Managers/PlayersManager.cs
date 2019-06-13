using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Networking.Transport;

public static class PlayersManager
{
    //public static Dictionary<int, int> playersList = new Dictionary<int, int>(); // connection id, playerid
    public static Queue<uint> activelySearchingPlayers = new Queue<uint>(); //playerID, connectionID
    public static Dictionary<uint, NetworkConnection> connectedPlayers = new Dictionary<uint, NetworkConnection>(); //playerID, connectionID

    public static Dictionary<uint, List<uint>> activeMatches = new Dictionary<uint, List<uint>>(); //matchID, List of players
    private static float waitTime = 15;
    private static uint currentMatchID = 0;

    public static IEnumerator CreateMatch(uint requestingPlayerID, Server server)
    {
        bool opponentFound = false;
        KeyValuePair<uint, NetworkConnection> requestingPlayer = new KeyValuePair<uint, NetworkConnection>(requestingPlayerID, connectedPlayers[requestingPlayerID]);
        KeyValuePair<uint, NetworkConnection> opponentInfo = new KeyValuePair<uint, NetworkConnection>();
        float startTime = Time.time;
        float waitedTime = Time.time - startTime;
        while ((activelySearchingPlayers.Count <= 1 && waitedTime < waitTime))
        {
            waitedTime = Time.time - startTime;
            yield return null;
        }

        if (waitedTime < waitTime)
        {
            do
            {
                uint opponentID = activelySearchingPlayers.Dequeue();
                opponentInfo = new KeyValuePair<uint, NetworkConnection>(opponentID, connectedPlayers[opponentID]);
                yield return null;
            } while (opponentInfo.Key == requestingPlayer.Key && connectedPlayers.ContainsKey(opponentInfo.Key));

            Debug.Log("requesting: " + requestingPlayer.Key + " - " + requestingPlayer.Value);
            Debug.Log("opponent: " + opponentInfo.Key + " - " + opponentInfo.Value);
            //connectedPlayers.Add(requestingPlayer.Key, requestingPlayer.Value);
            //StandbyPlayers.Add(opponentInfo.Key, opponentInfo.Value);

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
                requestingPlayer.Value.Send(server.m_Driver, writer);
                yield break;
            }

            foreach (uint player in players)
            {
                connectedPlayers[player].Send(server.m_Driver, writer);
            }
        }
    }
}

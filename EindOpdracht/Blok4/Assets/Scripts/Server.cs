using UnityEngine;
using System.Net;
using UnityEngine.SceneManagement;

using Unity.Networking.Transport;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class Server : MonoBehaviour
{
    private static Server instance;
    public UdpCNetworkDriver m_Driver;
    private NativeList<NetworkConnection> m_Connections;

    [SerializeField] private int networkPort = 9000;
    [SerializeField] private int connectionCapacity = 16;
    [SerializeField] private int dataCapacity = 4;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        m_Driver = new UdpCNetworkDriver(new NetworkConfigParameter{disconnectTimeoutMS = 60000});
        if (m_Driver.Bind(new IPEndPoint(IPAddress.Any, networkPort)) != 0)
        {
            Debug.Log("Failed to bind to port " + networkPort);
        }
        else
        {
            m_Driver.Listen();
        }

        m_Connections = new NativeList<NetworkConnection>(connectionCapacity, Allocator.Persistent);
    }

    private void OnApplicationQuit()
    {
        OnDestroy();
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
        m_Connections.Dispose();
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        // Clean up connections
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                --i;
            }
        }

        // Accept new connections
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default(NetworkConnection))
        {
            m_Connections.Add(c);
            using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PING))
            {
                c.Send(m_Driver, writer);
            }
            Debug.Log("Accepted a connection");
        }

        DataStreamReader stream;
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                continue;
            }

            NetworkEvent.Type command;
            while ((command = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (command == NetworkEvent.Type.Data)
                {
                    HandleData(stream, m_Connections[i]);
                }
                else if (command == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from server");
                    m_Connections[i] = default(NetworkConnection);
                }
            }
        }
    }

    private void HandleData(DataStreamReader stream, NetworkConnection connection)
    {
        DataStreamReader.Context readerCtx = default(DataStreamReader.Context);
        ConnectionEvent receivedEvent = (ConnectionEvent)stream.ReadUInt(ref readerCtx);

        Debug.Log("Received event is: " + receivedEvent.ToString());

        MessageCenter.ReadDictionary[receivedEvent](this, stream, ref readerCtx, connection);
    }

    public IEnumerator StartMatch(uint matchID)
    {
        yield return new WaitForSeconds(5);
        
        List<uint> playersInMatch;
        if (!PlayersManager.activeMatches.TryGetValue(matchID, out playersInMatch))
        {
            //send error
            Debug.Log("Match does not exist!");
            yield break;
        }

        Debug.Log(playersInMatch[0] + " - " + playersInMatch[1]);

        StartGameStruct dataStruct = new StartGameStruct()
        {
            playingFieldID = 1,
            amountOfPlayers = (uint)playersInMatch.Count,
            playerInfoID = new List<uint>
            {
                playersInMatch[0],
                playersInMatch[1]
            },
            playerInfoCharacter = new List<uint>
            {
                1,
                1
            }
        };

        //List<uint> data = new List<uint>();
        ////data.Add(connectionEvent);
        //data.Add(1); //playingFieldID
        //data.Add((uint)playersInMatch.Count);

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.START_GAME, dataStruct))
        {
            foreach (uint player in playersInMatch)
            {
                PlayersManager.connectedPlayers[player].Send(m_Driver, writer);
                Debug.Log("Send message to player " + player);
            }
        }

        Debug.Log("Loading scene");
        AsyncOperation asyncLoading = SceneManager.LoadSceneAsync("OpenField");
        while (!asyncLoading.isDone)
        {
            yield return null;
        }

        Debug.Log("Setting up game");
        FindObjectOfType<GameStarter>().SetupGame(this, matchID);
    }
}

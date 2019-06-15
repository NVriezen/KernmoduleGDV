using UnityEngine;
using System.Net;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Utilities;
using System.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;

public class Client : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    //public NetworkPipeline reliablePipeline;
    public NetworkConnection m_Connection;
    public bool connected;

    [SerializeField] private int networkPort = 9000;
    [SerializeField] private int dataCapacity = 4;
    [SerializeField] private bool localPlayer = true;
    private IPEndPoint endPoint;

    public PlayerInfo playerInfo;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        if (localPlayer)
        {
            m_Driver = new UdpCNetworkDriver(new NetworkConfigParameter { });///*new ReliableUtility.Parameters { WindowSize = 32 },*/ new NetworkConfigParameter { disconnectTimeoutMS = 60000 });
            //reliablePipeline = m_Driver.CreatePipeline(typeof(UnreliableSequencedPipelineStage), typeof(ReliableSequencedPipelineStage));
            m_Connection = default(NetworkConnection);

            endPoint = new IPEndPoint(IPAddress.Loopback, networkPort);
            //NetworkEndPoint endPoint = NetworkEndPoint.Parse("192.168.2.190", 9000);
            m_Connection = m_Driver.Connect(endPoint);
        }
    }

    public void AssignInfo(PlayerInfo playerInfo)
    {
        this.playerInfo = playerInfo;
    }

    public void OnApplicationQuit()
    {
        OnDestroy();
    }

    public void OnDestroy()
    {
        if (localPlayer)
        {
            m_Driver.Dispose();
        }
    }

    private void Update()
    {
        if (!localPlayer)
        {
            return;
        }

        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            if (!connected)
            {
                Debug.Log("Something went wrong during connecting");
            }

            //if (endPoint.IsValid)
            //{
            //    m_Connection = m_Driver.Connect(endPoint);
            //    Debug.Log("Tried to reconnect");
            //}

            return;
        }

        Debug.Log("Working connection");
        DataStreamReader stream;
        NetworkEvent.Type command;

        while ((command = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (command == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server");

                PlayerConnectStruct dataStruct = new PlayerConnectStruct()
                {
                    playerID = (uint)playerInfo.userID
                };

                using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_CONNECT, dataStruct))
                {
                    m_Connection.Send(m_Driver, writer);
                }

                connected = true;
            }
            else if (command == NetworkEvent.Type.Data)
            {
                HandleData(stream, m_Connection);
            }
            else if (command == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server");
                m_Connection = default(NetworkConnection);
                connected = false;
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

    public bool RequestOpponent()
    {
        if (!connected || !m_Connection.IsCreated)
        {
            return false;
        }

        RequestOpponentStruct dataStruct = new RequestOpponentStruct()
        {
            playerID = (uint)playerInfo.userID
        };

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.REQUEST_OPPONENT, dataStruct))
        {
            m_Connection.Send(m_Driver, /*reliablePipeline,*/ writer);
        }
        return true;
    }

    public IEnumerator CheckForSceneLoaded()
    {
        do
        {
            Debug.Log("Checking for scene loading done");
            yield return null;
        } while (GameObject.FindObjectOfType<ClientMatchManager>() == null);

        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.DONE_LOADING_LEVEL))
        {
            m_Connection.Send(m_Driver, /*reliablePipeline,*/ writer);
        }
    }

    public IEnumerator StartGame(StartGameStruct startGameStruct)
    {
        AsyncOperation loadingScene = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("OpenField");
        while (!loadingScene.isDone)
        {
            yield return null;
        }

        ClientMatchManager.instance.Init(startGameStruct);

        yield return null;
    }
}

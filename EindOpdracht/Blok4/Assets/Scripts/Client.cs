using UnityEngine;
using System.Net;

using Unity.Networking.Transport;
using Unity.Collections;

using UdpCNetworkDriver = Unity.Networking.Transport.BasicNetworkDriver<Unity.Networking.Transport.IPv4UDPSocket>;
using System.Collections;

public class Client : MonoBehaviour
{
    public UdpCNetworkDriver m_Driver;
    public NetworkConnection m_Connection;
    public bool connected;

    [SerializeField] private int networkPort = 9000;
    [SerializeField] private int dataCapacity = 4;
    [SerializeField] private bool localPlayer = true;

    //[SerializeField] private int playerID;
    //private string username;
    //private string sessionID;

    public PlayerInfo playerInfo;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        if (localPlayer)
        {
            m_Driver = new UdpCNetworkDriver(new NetworkConfigParameter { disconnectTimeoutMS = 60000 });
            m_Connection = default(NetworkConnection);

            IPEndPoint endpoint = new IPEndPoint(IPAddress.Loopback, networkPort);
            m_Connection = m_Driver.Connect(endpoint);
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

            return;
        }

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
            m_Connection.Send(m_Driver, writer);
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
            m_Connection.Send(m_Driver, writer);
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

using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class ClientMatchManager : MonoBehaviour
{
    public static ClientMatchManager instance;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private GameObject flamePrefab;
    private List<uint> playersList;
    private Client client;
    private GameObject[] playerObjects;

    [SerializeField] private Text totalTurnText;
    [SerializeField] private GameObject playersLivesParentObject;
    [SerializeField] private GameObject playersNamesParentObject;
    private Text[] playersLivesText;
    private Text[] playersNamesText;
    private Image[] playersIconImages;

    [SerializeField] private Transform playerTurnIndicator;
    [SerializeField] private Camera fieldCamera;
    [SerializeField] private GameObject activeDuringTurnObject;
    [SerializeField] private Text notificationText;

    [SerializeField] private float moveSpeed = 2f;

    private List<GameObject> walkableFieldObjects = new List<GameObject>();
    private Dictionary<uint, GameObject> bombObjects = new Dictionary<uint, GameObject>();

    public void Start()
    {
        instance = this;
        client = FindObjectOfType<Client>();
        notificationText.transform.parent.gameObject.SetActive(false);
    }

    public void Init(StartGameStruct matchData)
    {
        playersList = matchData.playerInfoID;
        playersLivesText = playersLivesParentObject.GetComponentsInChildren<Text>();
        playersNamesText = playersNamesParentObject.GetComponentsInChildren<Text>();
        playersIconImages = playersLivesParentObject.GetComponentsInChildren<Image>();
        playerObjects = new GameObject[playersList.Count];

        for (int i = playersList.Count; i < playersLivesText.Length - playersList.Count; i++)
        {
            playersLivesText[i].gameObject.SetActive(false);
            playersNamesText[i].gameObject.SetActive(false);
        }
    }

    public bool GetGameReady()
    {
        //check if all player objects have been spawned
        if (totalTurnText.text == "")
        {
            return false;
        }
        return true;
    }

    public void SpawnPlayerObject(PlayerRespawnStruct spawnData)
    {
        playerObjects[playersList.IndexOf(spawnData.playerID)] = Instantiate(playerPrefab, new Vector3(spawnData.positionX, spawnData.positionY, spawnData.positionZ), Quaternion.identity);
    }

    public void UpdateUI(UIStateUpdateStruct UIData)
    {
        totalTurnText.text = "Turn " + UIData.totalTurns.ToString();
        for(int i = 0; i < playersList.Count; i++)
        {
            uint lifePoints = UIData.playerLives[i];
            playersLivesText[i].text = lifePoints.ToString();
            Debug.Log("new life value set");
        }

        Debug.Log("UI Update Done");
    }

    public void UpdateTurns(TurnUpdateStruct turnsData)
    {
        totalTurnText.text = "Turn " + turnsData.totalTurns.ToString();
        ActivatePlayerTurn(turnsData.playerID);
    }

    private void ActivatePlayerTurn(uint playerID)
    {
        Debug.LogWarning("It is player" + playerID + "'s turn!");
        ClearWalkableField();
        int playerIndex = playersList.IndexOf(playerID);
        playerTurnIndicator.position = fieldCamera.ScreenToWorldPoint(playersIconImages[playerIndex].rectTransform.position);
        activeDuringTurnObject.SetActive(false);
        if (client.playerInfo.userID == playerID)
        {
            activeDuringTurnObject.SetActive(true);
        }
    }

    private void ClearWalkableField()
    {
        foreach (GameObject gameObject in walkableFieldObjects)
        {
            Destroy(gameObject);
        }
        walkableFieldObjects.Clear();
    }

    public void ActivateWalkableField(Vector3 centerPosition, List<Vector3> walkablePositions)
    {
        foreach (Vector3 position in walkablePositions)
        {
            GameObject fieldObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            walkableFieldObjects.Add(fieldObject);
            fieldObject.transform.position = position + new Vector3(0,-0.4f,0);
            fieldObject.GetComponent<MeshRenderer>().material.color = new Color32(0, 255, 128, 128);
        }
    }

    public void MovePlayerOnPath(uint playerID, List<Vector3> pathToFollow)
    {
        Debug.Log("Starting coroutine");
        StartCoroutine(FollowPath(playerObjects[playersList.IndexOf(playerID)].transform, pathToFollow));
    }

    private IEnumerator FollowPath(Transform objectToMove, List<Vector3> pathToFollow)
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

    public void EndTurn()
    {
        PlayerEndTurnStruct dataToSend = new PlayerEndTurnStruct()
        {
            playerID = (uint)client.playerInfo.userID
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_END_TURN, dataToSend))
        {
            client.m_Connection.Send(client.m_Driver, writer);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnMouseDown();
        }
    }

    private void OnMouseDown()
    {
        Ray positionRay = fieldCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Vector3 newPositionToSend = Vector3.zero;
        if (Physics.Raycast(positionRay, out hit, 5000, 1 << 10)){
            newPositionToSend = hit.point;
        }

        PlayerMoveStruct dataToSend = new PlayerMoveStruct()
        {
            playerID = (uint)client.playerInfo.userID,
            newPosition = newPositionToSend,
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_MOVE, dataToSend))
        {
            client.m_Connection.Send(client.m_Driver, writer);
        }
    }

    public void PlaceBomb()
    {
        PlayerPlaceBombStruct playerPlaceBombStruct = new PlayerPlaceBombStruct()
        {
            playerID = (uint)client.playerInfo.userID
        };
        using (DataStreamWriter writer = MessageCenter.WriteEvent(ConnectionEvent.PLAYER_PLACE_BOMB, playerPlaceBombStruct))
        {
            client.m_Connection.Send(client.m_Driver, writer);
        }
    }

    public void SpawnBomb(uint bombID, Vector3 bombPosition)
    {
        bombObjects.Add(bombID, Instantiate(bombPrefab, bombPosition, Quaternion.identity)); 
        bombObjects[bombID].layer = 11;
    }

    public void SpawnFlames(BombExplodeStruct receivedData)
    {
        Destroy(bombObjects[receivedData.bombID]);
        bombObjects.Remove(receivedData.bombID);
        foreach (Vector3 flamePosition in receivedData.flamePositions) {
            Instantiate(flamePrefab, flamePosition, Quaternion.identity).layer = 11;
        }
    }

    public void DestroyCrates(CrateDestroyStruct data)
    {
        for (int i = 0; i < data.amountOfData; i++)
        {
            CrateManager.instance.DestroyCrate(data.cratesToDestroy[i]);
        }
    }

    public void GameOver(GameOverStruct dataContainer)
    {
        ClearWalkableField();
        notificationText.transform.parent.gameObject.SetActive(true);
        
        if (dataContainer.playerWon == 0)
        {
            notificationText.text = "It is a tie!";
        } else
        {
            Debug.Log("amount of text " + playersNamesText.Length);
            Debug.Log(" -- " + playersNamesText[0].text);
            notificationText.text = playersNamesText[playersList.IndexOf(dataContainer.playerWon)].text + " has won!";
        }
    }

    public void DestroyMatch()
    {
        //Send them to scores instead of Lobby
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginLobby");
    }
}

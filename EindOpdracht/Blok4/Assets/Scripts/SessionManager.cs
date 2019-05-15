using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;
    private string roomName;
    private int roomID;
    private List<PlayerComponent> playersInstances;
    private List<PlayerDetails> playersInRoom;
    private string currentPlayingField;
    private int currentTurn = 0;
    private int currentActivePlayer = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    public void Setup(string roomName, int roomID, string selectedPlayingField, List<PlayerDetails> playersList)
    {
        this.roomName = roomName;
        this.roomID = roomID;
        currentPlayingField = selectedPlayingField;
        playersInRoom = playersList;

        StartGame();
    }

    public void StartGame()
    {
        //setup game
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentPlayingField);

        List<SpawnPosition> playerSpawnPositions = new List<SpawnPosition>();
        foreach (SpawnPosition spawnPosition in FindObjectsOfType<SpawnPosition>())
        {
            playerSpawnPositions.Add(spawnPosition);
        }
        playerSpawnPositions.Sort((x, y) => x.PlayerNumber < y.PlayerNumber ? -1 : 1);

        foreach (PlayerDetails player in playersInRoom)
        {
            Instantiate(Resources.Load<GameObject>("Characters/" + player.Model), playerSpawnPositions[player.ID - 1].transform.position, playerSpawnPositions[player.ID - 1].transform.rotation);
        }

        //spawn crates in correct place and populate them

        //Update UI
    }
}

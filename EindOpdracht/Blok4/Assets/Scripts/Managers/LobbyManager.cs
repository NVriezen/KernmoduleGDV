using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject loginManagerContainer;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private Text informationText;

    private const string postURL = PHPSender.baseURL + "GameCallers/SetGame.php";

    private Client localPlayer;

    private void Awake()
    {
        lobbyCanvas.SetActive(false);
        EventManager.StartListening("NO_OPPONENT_FOUND", NoOpponentFound);
        EventManager.StartListening("OPPONENT_FOUND", OpponentFound);
    }


    public void Init(PlayerInfo playerInfo)
    {
        SetLobbyActive(playerInfo);

        informationText.text = "Connecting to game...";

        GameObject client = new GameObject("Client");
        localPlayer = client.AddComponent<Client>();
        localPlayer.AssignInfo(playerInfo);
        PHPSender.SetSession(playerInfo.sessionID);
        if (localPlayer.connected)
        {
            informationText.text = "Connected to server!";
        }
    }

    public void SetLobbyActive(PlayerInfo playerInfo)
    {
        loginManagerContainer.SetActive(false);
        lobbyCanvas.SetActive(true);
    }

    public void SearchOpponent()
    {
        informationText.text = "Searching for opponent...";
        StartCoroutine(SearchForOpponent());
    }

    public void Start()
    {
        StartCoroutine(SetGame());
    }

    private void OnDisable()
    {
        EventManager.StopListening("NO_OPPONENT_FOUND", NoOpponentFound);
        EventManager.StopListening("OPPONENT_FOUND", OpponentFound);
    }

    private void NoOpponentFound()
    {
        informationText.text = "No opponents were found. Please come back later.";
    }

    private void OpponentFound()
    {
        informationText.text = "An opponent has been found! The game begins!";
        EventManager.StopListening("NO_OPPONENT_FOUND", NoOpponentFound);
    }

    private IEnumerator SearchForOpponent()
    {
        while (!localPlayer.RequestOpponent())
        {
            yield return null;
        }
    }

    private IEnumerator SetGame()
    {
        WWWForm gameForm = new WWWForm();

        gameForm.AddField("sessionID", localPlayer.playerInfo.sessionID);
        gameForm.AddField("gamename", Application.productName);

        // Post the URL to the site and create a download object to get the result.
        UnityWebRequest post = UnityWebRequest.Post(postURL, gameForm);
        Debug.Log(post.url);
        yield return post.SendWebRequest(); // Wait until the download is done


        Debug.Log(post.downloadHandler.text);
        if (post.isNetworkError || post.isHttpError || post.downloadHandler.text.Contains("Error"))
        {
            Debug.Log("There was an error: " + post.error);
            Debug.Log(post.downloadHandler.text);
            yield break;
        }
        else
        {
            if (post.downloadHandler.text == 0.ToString())
            {
                Debug.LogError("Error setting game - " + post.downloadHandler.text);
                yield return null;
            }
            Debug.Log("Game set succesfully!");
        }
    }

    public void LoadScene(string name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }
}

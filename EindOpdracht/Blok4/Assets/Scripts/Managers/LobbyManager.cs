using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private GameObject loginManagerContainer;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private Text informationText;
    //private string username;
    private Client localPlayer;

    private void Awake()
    {
        lobbyCanvas.SetActive(false);
        EventManager.StartListening("NO_OPPONENT_FOUND", NoOpponentFound);
        EventManager.StartListening("OPPONENT_FOUND", OpponentFound);
    }


    public void Init(PlayerInfo playerInfo)
    {
        loginManagerContainer.SetActive(false);
        lobbyCanvas.SetActive(true);

        informationText.text = "Connecting to game...";

        GameObject client = new GameObject("Client");
        localPlayer = client.AddComponent<Client>();
        localPlayer.AssignInfo(playerInfo);
        //localPlayer.sessionID = sessionID;
    }

    private void Start()
    {
        informationText.text = "Searching for opponent...";
        StartCoroutine(SearchForOpponent());
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
}

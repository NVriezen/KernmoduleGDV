using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

[System.Serializable]
public class PlayerInfo
{
    public int userID;
    public string username;
    public string sessionID;
}

public class LoginManager : MonoBehaviour
{
    [SerializeField] private InputField usernameField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private LobbyManager lobbyManager;
    private string username;
    private string password;

    private const string postURL = PHPSender.baseURL + "GameCallers/LoginGame.php";

    private void Awake()
    {
        usernameField.onValueChanged.AddListener((value) => OnUsernameEdit(value));
        passwordField.onValueChanged.AddListener((value) => OnPasswordEdit(value));
        lobbyManager.enabled = false;
    }

    private void Start()
    {
        Client client = FindObjectOfType<Client>();
        if (client != null)
        {
            EnableLobby(client.playerInfo);
        }
    }

    public void OnLogin()
    {
        Debug.Log("Logging in...");
        Debug.Log("Username entered is: " + username);

        StartCoroutine(PostLogin());
    }

    public void OnUsernameEdit(string value)
    {
        username = value;
    }

    public void OnPasswordEdit(string value)
    {
        password = value;
    }

    private IEnumerator PostLogin()
    {
        WWWForm loginForm = new WWWForm();

        loginForm.AddField("username", username);
        loginForm.AddField("password", password);

        // Post the URL to the site and create a download object to get the result.
        UnityWebRequest post = UnityWebRequest.Post(postURL, loginForm);
        Debug.Log(post.url);
        yield return post.SendWebRequest(); // Wait until the download is done

        
        
        if (post.isNetworkError || post.isHttpError || post.downloadHandler.text.Contains("Error"))
        {
            Debug.Log("There was an error: " + post.error);
            Debug.Log(post.downloadHandler.text);
            yield break;
        }
        else
        {
            Debug.Log(post.downloadHandler.text);
            PlayerInfo info = JsonUtility.FromJson<PlayerInfo>(post.downloadHandler.text);
            Debug.Log("New SID is: " + info.sessionID);
            Debug.Log("ID: " + info.userID);
            Debug.Log("username: " + info.username);
            EnableLobby(info);
        }
    }

    private void EnableLobby(PlayerInfo info)
    {
        lobbyManager.enabled = true;
        lobbyManager.Init(info);
    }
}

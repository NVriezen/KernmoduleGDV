using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum HighscoreSetting
{
    Day = 1,
    Week = 2,
    Month = 3,
    Year = 4,
    Absolute = 0
}

public class HighscoreContainer
{
    public string[] highscores;
}

public class ScoreUIManager : MonoBehaviour
{
    [SerializeField] private GameObject loginManagerContainer;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private Text scoreText;

    private HighscoreSetting highscoreSetting = HighscoreSetting.Absolute; 

    private const string addScoreURL = PHPSender.baseURL + "GameCallers/AddScore.php";
    private const string getScoreURL = PHPSender.baseURL + "GameCallers/GetScore.php";
    private const string getHighscoreURL = PHPSender.baseURL + "GameCallers/GetHighscores.php";

    void Start()
    {
        scoreText.text = "";//Random.Range(0, 225320).ToString();
        ShowHighscores();
    }

    public void BackToLobby()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginLobby");
    }

    public void SetTimePeriod(Dropdown newTimePeriod)
    {
        highscoreSetting = (HighscoreSetting)newTimePeriod.value;
        
        ShowHighscores();
    }

    public void UploadScore()
    {
        Debug.Log("Sending Score to database");

        WWWForm scoreForm = new WWWForm();

        scoreForm.AddField("sessionID", PHPSender.SessionID);
        scoreForm.AddField("score", int.Parse(scoreText.text));

        StartCoroutine(PHPSender.SendFormAsync(addScoreURL, scoreForm, null));
    }

    public void ShowHighscores()
    {
        Debug.Log("Timeperiod: " + highscoreSetting);
        WWWForm highscoreForm = new WWWForm();

        highscoreForm.AddField("sessionID", PHPSender.SessionID);
        highscoreForm.AddField("highscoreSetting", (int)highscoreSetting);

        StartCoroutine(PHPSender.SendFormAsync(getHighscoreURL, highscoreForm, PopulateHighScoreList));
    }

    public void PopulateHighScoreList(string result)
    {
        scoreText.text = "";
        if (result == "1")
        {
            scoreText.text = "No scores have been set in this time period.";
            return;
        }
        HighscoreContainer highscoreArray = JsonUtility.FromJson<HighscoreContainer>(result);
        foreach (string highscoreEntry in highscoreArray.highscores)
        {
            scoreText.text += highscoreEntry + "\n";
        }
        if (scoreText.text == "")
        {
            scoreText.text = "Can't connect to server.";
        }
    }
}

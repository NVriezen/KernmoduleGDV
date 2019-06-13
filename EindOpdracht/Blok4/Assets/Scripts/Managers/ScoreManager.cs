using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoreManager
{
    private const string addScoreURL = "https://studenthome.hku.nl/~niels.vriezen/database/GameCallers/AddScore.php";
    private const string getScoreURL = "https://studenthome.hku.nl/~niels.vriezen/database/GameCallers/GetScore.php";

    public static int GetAllScores()
    {
        return 0;
    }

    public static int GetCurrentScore()
    {
        return 0;
    }

    //public static bool UploadScore(int newScore)
    //{
    //    Debug.Log("Sending Score to database");

    //    WWWForm scoreForm = new WWWForm();

    //    scoreForm.AddField("sessionID", localPlayer.playerInfo.sessionID);
    //    scoreForm.AddField("score", newScore);

    //    StartCoroutine(PHPSender.SendFormAsync(addScoreURL, scoreForm, null));
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class PHPSender
{
    private static string sessionID;
    public static string SessionID
    {
        get { return sessionID; }
    }

    public static void SetSession(string newSessionID)
    {
        if (sessionID != null)
        {
            return;
        }
        sessionID = newSessionID;
    }

    public delegate void SendingResult(string result);
    public const string baseURL = "https://nvriezen.nl/database/";

    public static IEnumerator SendFormAsync(string urlToSendTo, WWWForm formToSend, SendingResult callback)
    {
        // Post the URL to the site and create a download object to get the result.
        UnityWebRequest post = UnityWebRequest.Post(urlToSendTo, formToSend);
        Debug.Log(post.url);
        yield return post.SendWebRequest(); // Wait until the download is done

        if (post.isNetworkError || post.isHttpError || post.downloadHandler.text.Contains("Error") || post.downloadHandler.text.Contains("Notice"))
        {
            Debug.Log("There was an error: " + post.error);
            Debug.Log(post.downloadHandler.text);
            yield break;
        }
        else
        {
            Debug.Log("Checking for 0: " + post.downloadHandler.text);
            if (post.downloadHandler.text == 0.ToString())
            {
                Debug.LogError("Error " + post.downloadHandler.text);
                yield return false;
            }
           
        }
        if (callback != null)
        {
            callback(post.downloadHandler.text);
        }
        Debug.Log("Sending succesful");
        yield return true;
    }
}

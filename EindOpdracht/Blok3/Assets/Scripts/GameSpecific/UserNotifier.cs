using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UserNotifier : MonoBehaviour
{
    public static UserNotifier instance;
    [SerializeField] private float secondsToShowText = 2f;
    [SerializeField] private Text playerHealth;
    [SerializeField] private Text buddyHealth;
    private Text notifyText;


    private void Awake()
    {
        instance = this;
        notifyText = GetComponentInChildren<Text>();
        notifyText.text = "";
    }

    public void OnEnemyKill(string name)
    {
        StartCoroutine(ShowForSeconds("YOU KILLED " + name));
        if (name == "Boss")
        {
            StopAllCoroutines();
            OnBossKill();
        }
    }

    public void OnHit()
    {
        StartCoroutine(ShowForSeconds("YOU HIT SOMETHING!"));
    }

    public void OnHitReceive()
    {
        StartCoroutine(ShowForSeconds("SOMETHING HIT YOU!"));
    }

    private IEnumerator ShowForSeconds(string text)
    {
        notifyText.text = text;
        yield return new WaitForSeconds(secondsToShowText);
        notifyText.text = "";
    }

    public void OnBossKill()
    {
        StopAllCoroutines();
        notifyText.text = "BOSS KILLED\nYOU WIN!";
    }

    public void OnPlayerKill()
    {
        StopAllCoroutines();
        notifyText.text = "GAME OVER\nYOU LOST!";
    }

    public void UpdatePlayerHealth(float health)
    {
        playerHealth.text = health.ToString();
    }

    public void UpdateBuddyHealth(float health)
    {
        buddyHealth.text = health.ToString();
    }
}

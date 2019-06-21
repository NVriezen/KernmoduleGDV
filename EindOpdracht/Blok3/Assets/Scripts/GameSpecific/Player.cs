using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : IAttacker, IDamagable
{
    [SerializeField] private float health = 100;
    [SerializeField] private float attackingPower = 10;
    [SerializeField] private float healthRecoveryValue = 1;
    [SerializeField] private GameObject swordObject;
    [SerializeField] private float swingSpeed = 0.5f;
    [SerializeField] private float swingToleranceDistance = 0.05f;
    private bool isSwinging = false;

    private void Awake()
    {
        attackPower = attackingPower;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>() != null)
        {
            return;
        }

        if (other.GetComponent<Buddy>() != null)
        {
            return;
        }

        if (other.GetComponent<IDamagable>() != null)
        {
            UserNotifier.instance.OnHit();
            other.GetComponent<IDamagable>().ReceiveDamage(this.gameObject, attackPower);
        }
    }

    public bool ReceiveDamage(Object caller, float attackPower)
    {
        GameObject buddy = caller as GameObject;
        if (buddy.GetComponent<Buddy>() != null)
        {
            return false;
        }

        UserNotifier.instance.OnHitReceive();
        health -= attackPower;
        UserNotifier.instance.UpdatePlayerHealth(health);
        if (health <= 0)
        {
            OnDeath();
            return true;
        }
        return false;
    }

    public void OnDeath()
    {
        Debug.Log("Player has died :(");
        UserNotifier.instance.OnPlayerKill();
        //gameObject.SetActive(false);
    }

    public float GetHealth()
    {
        return health;
    }

    public bool RecoverHealth()
    {
        health += healthRecoveryValue;

        return true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(SwingSword());
        }
    }

    IEnumerator SwingSword()
    {
        if (isSwinging)
        {
            yield break;
        }

        isSwinging = true;
        Debug.Log("Swinging Sword - " + swordObject.transform.position + " -- : -- " + swordObject.transform.forward);
        Vector3 swingForward = swordObject.transform.localPosition;
        swingForward.z += 1;
        Vector3 originalLocalPosition = swordObject.transform.localPosition;
        Debug.Log(swingForward + ":" + originalLocalPosition);
        while (Vector3.Distance(swordObject.transform.localPosition, swingForward) > swingToleranceDistance)
        {
            Debug.Log("Still not there");
            swordObject.transform.localPosition = Vector3.MoveTowards(swordObject.transform.localPosition, swingForward, swingSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("Wait is over");
        while (Vector3.Distance(swordObject.transform.localPosition, originalLocalPosition) > swingToleranceDistance)
        {
            Debug.Log("Moving back");
            swordObject.transform.localPosition = Vector3.MoveTowards(swordObject.transform.localPosition, originalLocalPosition, swingSpeed * Time.deltaTime);
            yield return null;
        }
        isSwinging = false;
    }
}

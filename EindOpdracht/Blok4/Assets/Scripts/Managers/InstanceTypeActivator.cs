using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceTypeActivator : MonoBehaviour
{
    [SerializeField] private GameObject serverMatchManager;
    [SerializeField] private GameObject clientMatchManager;

    public void Awake()
    {
        serverMatchManager.SetActive(false);
        clientMatchManager.SetActive(false);
    }

    public void Start()
    {
        if (FindObjectOfType<Server>() != null)
        {
            serverMatchManager.SetActive(true);
        } else
        {
            clientMatchManager.SetActive(true);
        }
    }
}

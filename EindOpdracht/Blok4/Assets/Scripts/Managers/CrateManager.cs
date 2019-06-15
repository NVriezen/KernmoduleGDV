using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateManager : MonoBehaviour
{
    public static CrateManager instance;
    [SerializeField] private GameObject cratesContainer;
    private List<GameObject> crates = new List<GameObject>();

    private void Awake()
    {
        instance = this;   
    }

    private void Start()
    {
        BoxCollider[] colliders = cratesContainer.GetComponentsInChildren<BoxCollider>();
        foreach (BoxCollider collider in colliders)
        {
            if (collider.gameObject.layer == 12)
            {
                continue;
            }
            crates.Add(collider.gameObject);
        }
    }

    public bool DestroyCrate(Vector3 cratePosition)
    {
        GameObject crateToDestroy = crates.Find(x => x.transform.position.Equals(cratePosition + new Vector3(0,0.5f,0)));
        if (crateToDestroy == null)
        {
            Debug.LogError("No crate on that position. Total crates: " + crates.Count);
            return false;
        }
        Debug.Log(crateToDestroy.name);
        if (!crates.Remove(crateToDestroy))
        {
            Debug.LogError("Crate could not be destroyed! - Position: " + crateToDestroy.transform.position);
        }
        crateToDestroy.SetActive(false);
        //update walkable field
        return true;
    }
}
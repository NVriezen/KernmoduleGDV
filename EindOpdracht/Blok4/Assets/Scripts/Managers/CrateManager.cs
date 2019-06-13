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
            crates.Add(collider.gameObject);
        }
    }

    public bool DestroyCrate(Vector3 cratePosition)
    {
        GameObject crateToDestroy = crates.Find(x => x.transform.position.Equals(cratePosition + new Vector3(0,0.5f,0)));
        if (crateToDestroy == null)
        {
            return false;
        }
        //int index = crates.IndexOf(crateToDestroy);
        //crates.RemoveAt(index);
        if (!crates.Remove(crateToDestroy))
        {
            Debug.LogError("Crate could not be destroyed! - Position: " + crateToDestroy.transform.position);
        }
        Destroy(crateToDestroy);
        return true;
    }
}
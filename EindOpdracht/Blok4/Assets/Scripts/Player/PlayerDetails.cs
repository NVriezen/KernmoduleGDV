using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetails : MonoBehaviour
{
    private int id;
    public int ID
    {
        get
        {
            return id;
        }
    }

    private string name;
    public string Name
    {
        get
        {
            return name;
        }
    }

    private string model;
    public string Model
    {
        get
        {
            return model;
        }
    }
}

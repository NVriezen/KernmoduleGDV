using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Unity.Networking.Transport;
using System.Text;
using System;

public class BinaryTest : MonoBehaviour
{
    Encoding Ascii = Encoding.ASCII;
    Encoding Unicode = Encoding.Unicode;

    // Start is called before the first frame update
    void Start()
    {
        string text = "This is a string";

        Debug.Log("This is the string as a normal string: " + text);

        byte[] stringInBytes = Ascii.GetBytes(text);

        Debug.Log("This is the string converted from bytes back to a string: " + Ascii.GetString(stringInBytes));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Net.NetworkInformation;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        Server.Start(50, 2457);

    }

    private void OnApplicationQuit()
    {
        Debug.Log("Server Disconnected");
        Server.DisconnectAll();
        Server.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfo : MonoBehaviour
{
    [SerializeField]
    public string UserID { get; private set; }
    private string Username;
    private string UserPassword;
    private string level;
    private string coins;


    public void SetCredentials( string username, string userpassword)
    {
        Username = username;
        UserPassword = userpassword;
    }

    public void SetID(string id)
    {
        UserID = id;
        UserID = 1.ToString();
    }
}

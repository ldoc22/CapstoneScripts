using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    public Web web;
    public UserInfo userInfo;
    public Login login;
    public GameObject userProfile;
    public ItemManager items;
  
    public static Main instance;

    public bool loggedIn;

    UserProfile userProfileTemp;
    
    public bool ConnectedAndLoggedIn;


    public int numberOfLoads = 1, currentNumberOfLoads;
    

    public float ItemVersion = 0.0f;

    private void OnEnable()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
        ConnectedAndLoggedIn = false;
        userInfo = GetComponent<UserInfo>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //ItemLoader.UpdateIconNumbers();
        Client.instance.ConnectToServer();
        
    }


  


    public void Connected()
    {
        currentNumberOfLoads = 0;
        loggedIn = true;
        login = GetComponent<Login>();
        web = GetComponent<Web>();
        
        ConnectedAndLoggedIn = false;
        ReadKey();
        CharacterManager.instance.CreateCharacters();
        //CreateSOs();
    }

    public void FinishALoad(bool t)
    {
        currentNumberOfLoads++;
        if(currentNumberOfLoads == numberOfLoads)
        {
            LoadComplete(true);
        }
    }


    public void LoadComplete(bool b)
    {
        ConnectedAndLoggedIn = b;

        
        
        login.E_D_Load(false);
    }
    public void SendLogin()
    {
        ClientSend.RequestGameEntrance();
            
    }
    public void LaunchToGame()
    {
       
        try
        {
            Client.instance.LoadScene();
            //GameObject go = Instantiate(Resources.Load("ClientManager") as GameObject);
            //ClientSend.CharacterLogIn();
            
            
        }
        catch(Exception ex)
        {
            
            Debug.Log("Failed Launch into Game");
            Debug.Log(ex);
        }

    }

    public void SetProfile(UserProfile _userProfile)
    {
        userProfileTemp = _userProfile;
    }

    public bool ErrorTextActive;
    string errorText;
    public void ErrorText(string txt)
    {
        errorText = txt;
        StartCoroutine(HoldTextForTime());
    }
    public  IEnumerator HoldTextForTime()
    {
        ErrorTextActive = true;
        new WaitForSeconds(3);
        ErrorTextActive = false;
        return null;

    }
    private void OnGUI()
    {
        if (ErrorTextActive)
        {
            GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, 100, 50), errorText);
        }
    }

    public int ReadKey()
    {
        string rootPath = Directory.GetCurrentDirectory();
        string fileName = Path.Combine(rootPath, "GameClient_Data/Resources", "Key.txt");
        if (File.Exists(fileName))
        {
            ErrorText("FOUND FILE");
        }
        else
        {
            Application.Quit();
        }
        try
        {
            using (StreamReader sr = File.OpenText(fileName))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    try
                    {
                        return int.Parse(s);
                    }
                    catch
                    {
                        Debug.Log("Could not parse from file");
                    }
                }
            }
        }
        catch
        {
            Debug.Log("Could not find file");
        }
        return -1;
    }

    


}

using System.Net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;
using System.Security.Cryptography;
using System.Text;

public class ServerHandle : MonoBehaviour
{
    public static ServerHandle instance;

    private void OnEnable()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }


    string _basePath = "http://localhost/UnityBackendTutorial/";

    public void RegisterUser(int _from, Packet _packet)
    {
        string _username = _packet.ReadString();
        string _password = _packet.ReadString();
        string salt = SecurityHelper.GenerateSalt(70);
        string pwdHashed = SecurityHelper.HashPassword(_password, salt, 10101, 70);
        Debug.Log(pwdHashed);
        StartCoroutine(RegisterUser(_username, _password, _from));
    }

    public void Login(int _from, Packet _packet)
    {
        string _username = _packet.ReadString();
        string _password = _packet.ReadString();
        StartCoroutine(Login(_username, _password, _from));
    }
   

    public void CharacterRequest(int _from)
    {
        StartCoroutine(GetCharacters(_from,Server.clients[_from].dbID, ServerSend.ReturnCharacterRequest));
    }

    public void RequestCreation(int _from, Packet _packet)
    {
       
        string _list = _packet.ReadString();
        StartCoroutine(CreateCharacter(_from, _list));
    }



    public IEnumerator Login(string username, string password, int _id)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "Login.php", form))
        {

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                //Main.instance.userInfo.SetCredentials(username, password);
                //Main.instance.userInfo.SetID(www.downloadHandler.text);

                if (www.downloadHandler.text.Contains("Wrong Credentials") || www.downloadHandler.text.Contains("Username does not exist"))
                {
                    Debug.Log("Try Again");
                    ServerSend.LoginFailure(_id);
                }
                else
                {

                    ServerSend.LoginSuccessful(_id, int.Parse(www.downloadHandler.text.Trim()));
                    // Main.instance.characterManager.CreateCharacters();
                }

            }
        }
    }

    public IEnumerator RegisterUser(string username, string password, int _from)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "RegisterUser.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                ServerSend.RegisterFailure(_from);
            }
            else
            {   
                if(www.downloadHandler.text.Contains("Username is already taken"))
                {
                    Debug.Log("Name Already Taken");
                }else if (www.downloadHandler.text.Contains("Successfully")){
                    StartCoroutine(Login(username, password, _from));
                    Debug.Log(www.downloadHandler.text);
                }
                else
                {
                    ServerSend.RegisterFailure(_from);
                }
            }
        }
    }


    public IEnumerator GetCharacters(int _from,int id, System.Action<int, string> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", id);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "GetCharacters.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                string jsonArray = www.downloadHandler.text;
                callback(_from, jsonArray);
            }
        }
    }


    public IEnumerator CreateCharacter(int id, string characterDetails)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", Server.clients[id].dbID);

        string[] pieces = characterDetails.Split(',');
        form.AddField("char_name", pieces[0]);
        form.AddField("race", int.Parse(pieces[1]));
        form.AddField("gender", int.Parse(pieces[2]));
        form.AddField("hair", int.Parse(pieces[3]));
        form.AddField("beard", int.Parse(pieces[4]));
        form.AddField("eyebrow", int.Parse(pieces[5]));
        form.AddField("skin", pieces[6]);
        form.AddField("eye", pieces[7]);
        form.AddField("hair_color", pieces[8]);
        form.AddField("mouth_color", pieces[9]);


        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "CheckCharacterName.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (www.downloadHandler.text.Contains("Created"))
                {
                    ServerSend.ReturnCharacterCreation(id, true);
                }
                else
                {
                    ServerSend.ReturnCharacterCreation(id, false);
                }
            }
        }
    }


}
public class SecurityHelper
{
    public static string GenerateSalt(int nSalt)
    {
        var saltBytes = new byte[nSalt];

        using (var provider = new RNGCryptoServiceProvider())
        {
            provider.GetNonZeroBytes(saltBytes);
        }

        return Convert.ToBase64String(saltBytes);
    }

    public static string HashPassword(string password, string salt, int nIterations, int nHash)
    {
        var saltBytes = Convert.FromBase64String(salt);

        using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltBytes, nIterations))
        {
            return Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(nHash));
        }
    }
}

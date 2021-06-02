using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Web : MonoBehaviour
{
    // Start is called before the first frame update


    string _basePath = "http://localhost/UnityBackendTutorial/";
    void Start()
    {
        //StartCoroutine(GetDate());
        //StartCoroutine(GetUsers());
        //StartCoroutine(Login("logantest", "logantest1234"));
        //StartCoroutine(RegisterUser("logantestReg", "logantest12345"));
       
    }
 

   IEnumerator GetDate()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_basePath+"GetDate.php"))
        {
            yield return www.SendWebRequest();
            if(www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                byte[] results = www.downloadHandler.data;
            }
        }
    }

    IEnumerator GetUsers()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(_basePath + "GetUsers.php"))
        {
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);

                byte[] results = www.downloadHandler.data;
            }
        }
    }


    


   

    public IEnumerator GetCharacters(string id, System.Action<string> callback)
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
                callback(jsonArray);
            }
        }
    }


    public IEnumerator CreateCharacter(string _id, string _name, System.Action<string> callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("char_name", _name);
        form.AddField("userID", _id);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "CheckCharacterName.php", form))
        {

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonArray = www.downloadHandler.text;
                callback(jsonArray);
            }
        }
    }
}

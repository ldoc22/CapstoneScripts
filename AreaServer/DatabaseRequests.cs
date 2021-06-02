using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using System;

public class DatabaseRequests : MonoBehaviour
{
    public static DatabaseRequests instance;
    private void OnEnable()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }

    string _basePath = "http://localhost/UnityBackendTutorial/";

    public IEnumerator GetCharacter_Username(string id, string charID, System.Action<string> callback)
    {
        WWWForm form = new WWWForm();
        Debug.Log("userID: " + id + " , charID: " + charID);
        //form.AddField("userID", id);
        form.AddField("char_id", charID);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "Server_GetCharacter.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                JSONArray jsonArray = JSON.Parse(www.downloadHandler.text) as JSONArray;
                // string _username = jsonArray.AsObject["char_name"];

                callback(www.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetCharacterCharacteristics(int id, Action<string> callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("charID", id);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "GetCharacterCharacteristics.php", form))
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



    public IEnumerator CreateCharacter(string id, string characterDetails)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", id);

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
                    ServerSend.ReturnDatabaseRequest_CharacterCreation(id, true);
                }
                else
                {
                    ServerSend.ReturnDatabaseRequest_CharacterCreation(id, false);
                }
            }
        }
    }


    public IEnumerator GetCharacters(string id, System.Action<string, string> callback)
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
                callback(id, jsonArray);
            }
        }
    }

    public IEnumerator GetItems(int id, System.Action<int, string> callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("charID", id);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "GetItemsIDs.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text);
                string jsonArray = www.downloadHandler.text;
                callback(id, jsonArray);
            }
        }

    }
    public IEnumerator GetItem(string id, Action<string> callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("itemID", id);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "GetItems.php", form))
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

    public IEnumerator ReturnItemFromJson(int id, string jsonarray)
    {
        JSONArray jsonArray = JSON.Parse(jsonarray) as JSONArray;
        if (jsonarray.Length != 0 && !jsonarray.Contains("0 results"))
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {

                bool isDone = false; //are we done downloading
                string itemId = jsonArray[i].AsObject["itemid"];
                JSONObject itemInfoJson = new JSONObject();
                Action<string> getItemInfoCallback = (itemInfo) =>
                {
                    isDone = true;
                    JSONArray tmpArray = JSON.Parse(itemInfo) as JSONArray;
                    itemInfoJson = tmpArray[0].AsObject;
                };

                StartCoroutine(GetItem(itemId, getItemInfoCallback));

                yield return new WaitUntil(() => isDone == true);

                ServerSend.ReturnDatabaseRequest_Items(id, itemInfoJson.ToString(), (i == jsonArray.Count - 1));

            }
        }

    }

    public IEnumerator AddItem(int _from, int charID, int itemNumber)
    {
        WWWForm form = new WWWForm();
        Debug.Log(charID + " picked up item " + itemNumber);
        form.AddField("charID", charID);
        form.AddField("itemID", itemNumber);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "AddItem.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Action<string> itemcallback = (info) =>
                {
                    ServerSend.ReturnDatabaseRequest_Items(_from, info, true);
                };

                Debug.Log(www.downloadHandler.text);
                if (www.downloadHandler.text.Contains("success"))
                {
                    StartCoroutine(GetItem(itemNumber.ToString(), itemcallback));
                }
            }
        }
    }


    public IEnumerator CreateItemForDatabase(string name, string description, int price, int rarity, int levelReq, bool isWeapon, int armor, int attackDamage, int magicDamage, int iconID, Action<bool> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("Itemname", name);
        form.AddField("description", description);
        form.AddField("price", price);
        form.AddField("rarity", rarity);
        if (isWeapon)
        {
            form.AddField("isWeapon", 1);
            form.AddField("armor", 0);
        }
        else
        {
            form.AddField("isWeapon", 0);
            form.AddField("armor", armor);
        }


        form.AddField("attackDamage", attackDamage);
        form.AddField("magicDamage", magicDamage);
        form.AddField("iconID", iconID);
        form.AddField("levelRequirement", levelReq);

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "CreateItem.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                callback(true);
            }
        }
    }

    public void StartPingLog(int numberOfUsers, float average)
    {
        StartCoroutine(PingLog(numberOfUsers, average));
    }
    public IEnumerator PingLog(int numberOfUsers, float average)
    {
        WWWForm form = new WWWForm();
        form.AddField("numberOfUsers", numberOfUsers);
        form.AddField("average", average.ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(_basePath + "AddPingLog.php", form))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                
            }
        }
    }
}

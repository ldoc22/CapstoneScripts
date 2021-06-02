using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using SimpleJSON;

public class CharacterManager : MonoBehaviour
{
    Action<string> _createCharacterCallback;

    public static CharacterManager instance;

    void OnEnable()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);

        /*
        _createCharacterCallback = (jsonArray) => {
            StartCoroutine(CreateCharactersRoutine(jsonArray, Main.instance.LoadComplete));
        };
        */
        

        
    }

    public void CreateCharacters()
    {  
        ClientSend.RequestCharacters();

    }

    public void RequestReturned(string json)
    {
        StartCoroutine(CreateCharactersRoutine(json, Main.instance.LoadComplete));
    }

    IEnumerator CreateCharactersRoutine(string _jsonArray, Action<bool> callback)
    {

        
        //print("Childsren" + transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
           
            Destroy(transform.GetChild(i).gameObject);
        }

        JSONArray jsonArray = JSON.Parse(_jsonArray) as JSONArray;
        if (jsonArray.Count > 0 && jsonArray != null)
        {
            for (int i = 0; i < jsonArray.Count; i++)
            {
                GameObject item = Instantiate(Resources.Load<GameObject>("CharacterSelectPrefab") as GameObject);
                item.transform.SetParent(this.transform);
                CharacterSelectPrefab_UI ui = item.AddComponent<CharacterSelectPrefab_UI>();
                string _name = jsonArray[i].AsObject["char_name"];
                string _level = jsonArray[i].AsObject["char_level"];
                string _id = jsonArray[i].AsObject["id"];
                ui.SetText(_name, _level);
                print(_name);
                item.GetComponent<Button>().onClick.AddListener(() =>
                {
                    string name = _name;
                    string level = _level;
                    string id = _id;
                    Account.instance.SetCharID(int.Parse(_id));
                    Account.instance.character.SetProfile(name, level, id);
                });
            }

            yield return null;
            callback(true);
        }
    }

    public void CreateCharacter_Indivdual(string _name, string _level, string _id)
    {
        GameObject item = Instantiate(Resources.Load<GameObject>("CharacterSelectPrefab") as GameObject);
        item.transform.SetParent(this.transform);
        CharacterSelectPrefab_UI ui = item.AddComponent<CharacterSelectPrefab_UI>(); 
        ui.SetText(_name, _level);
        item.GetComponent<Button>().onClick.AddListener(() =>
        {
            string name = _name;
            string level = _level;
            string id = _id;
            Account.instance.character.SetProfile(name, level, id);
        });
    }

    public void Reset()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }


}

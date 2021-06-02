using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public DatabaseRequests db;
    public GameObject playerPrefab;
    public GameObject projectilePrefab;

    public static Dictionary<int, NPC> NPCs = new Dictionary<int, NPC>();
    public bool spoofing;
    public float time;

    public bool isOnline;
    public bool attempting;
    public int recieved;
    public int sent;
    public float testTime;
    public void Awake()
    {

        if(instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Debug.Log("Instance of Network Manager already Exists!");

            Destroy(this);
        }

       NPC[] npcTemp = FindObjectsOfType<NPC>();
        for (int i = 0; i < npcTemp.Length; i++)
        {
            NPCs.Add(i, npcTemp[i]);
        }
        time = 0;
        isOnline = false;
        attempting = false;
        
        
    }

    public void Start()
    {


        isOnline = true;
        StartAreaServer();

        

    }

    public void StartAreaServer()
    {
        coroutine = null;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Server.Start(50, 2456);
        db = GetComponent<DatabaseRequests>();
        if (db == null)
        {
            db = gameObject.AddComponent<DatabaseRequests>();
        }
    }

    private void Update()
    {
        if (isOnline)
        {
            time += Time.deltaTime;
            if (time >= 2)
            {
                for (int i = 1; i <= Server.toPing.Count; i++)
                {
                    ServerSend.Ping(i);
                    
                }
                time = 0;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(Server.FindHowManyActive());
            }
        }
        else
        {

            
            {

            }
            if (!attempting)
            {
                time = 0;
                attempting = true;
                AreaServer.instance.Connect();

            }else if (time > 10)
            {
                attempting = false;
            }
            time += Time.deltaTime;
        }
        if(testTime > 1f)
        {
            db.StartPingLog(Server.clients.Count, sent);
            sent = 0;
            testTime = 0;
        }
        testTime += Time.deltaTime;
       
        
        
    }

    public void AddPingLog()
    {

        db.StartPingLog(PingTracker.trackers.Count, PingTracker.GetAverage());
    }



    Coroutine coroutine;
    private void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width * .9f, Screen.height * .1f, 80, 40), "Fill Database"))
        {
            if(coroutine == null)
            {
                coroutine = StartCoroutine(DatabaseFill());
            }
        }     
    }

    public IEnumerator DatabaseFill()
    {
        bool isDone = false;
        Action<bool> _databasecallback = (itemInfo) =>
        {
            isDone = true;
        };


        for (int i = 1; i <= 4; i++)
        {
            for (int l = 1; l <= 50; l += 3)
            {

                for (int j = 1; j <= 106; j++)
                {
                    isDone = false;
                    StartCoroutine(db.CreateItemForDatabase("", "", (l * i) * 50, i,l, false, (i * l) * 8, (i * l) * 2, (i * l) * 3, j, _databasecallback));
                    yield return new WaitUntil(() => isDone == true);
                }
                for (int j = 1006; j <= 1252; j++)
                {
                    StartCoroutine(db.CreateItemForDatabase("", "", (l * i) * 50, i,l, true, 0, (i * l) * 2, (i * l) * 3, j, _databasecallback));
                    yield return new WaitUntil(() => isDone == true);
                }
            }
        }
        Debug.Log("Completed");
        yield return null;
    }

    

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {

        GameObject obj = Instantiate(playerPrefab, new Vector3(0, 6, 0), Quaternion.identity);
        if(obj.GetComponent<Player>() != null)
            return obj.GetComponent<Player>();
        else
        {
            Debug.Log("player null");
            return null;
        }
    }

    public void LoadCharacterVariables(int _id)
    {
        Server.clients[_id].player = InstantiatePlayer();
        Server.clients[_id].player.Initialize(_id, Server.clients[_id].dbID, Server.clients[_id].charID, "Test: " +_id);
        GetPlayerCharacteristics(Server.clients[_id].player);
    }

    public void GetPlayerCharacteristics(Player _player)
    {
        bool isDone = false;

        Action<string> NameCallback = (name) =>
        {
            _player.username = name;
            Server.clients[_player.id].SendIntogame(_player);
        };

        Action<string> CharacteristicsCallback = (charStats) =>
        {
            _player.Characteristics = charStats;

            StartCoroutine(db.GetCharacter_Username(_player.dbID.ToString(), _player.characterID.ToString(), NameCallback));
        };

        Action<int, string> Itemcallback = (charID, json) => {
            //Debug.Log("THIS IS THE JSON FOR " +charID +": " + json );
            if (json.Length != 0 && !json.Contains("0 results"))
            {


                JSONArray jsonArray = JSON.Parse(json) as JSONArray;
                for (int i = 0; i < jsonArray.Count; i++)
                {
                    string itemID = jsonArray[i].AsObject["itemid"];
                    bool _equipped = (int.Parse(jsonArray[i].AsObject["equipped"]) == 1);
                    
                    if (!_equipped)
                    {
                        _player.AddItemToInventory(int.Parse(itemID));
                    }
                    else
                    {
                        _player.EquipItem(int.Parse(itemID), 0);
                    }


                }
            }
            StartCoroutine(db.GetCharacterCharacteristics(_player.characterID, CharacteristicsCallback));
        };

        

       StartCoroutine(db.GetItems(_player.characterID, Itemcallback));
      
       
    }

    public Projectile InstantiateProjectile(Transform _shootOrigin)
    {
        return Instantiate(projectilePrefab, _shootOrigin.position + _shootOrigin.forward * 0.7f, 
            Quaternion.identity).GetComponent<Projectile>();
    }

    public void PickedUpItem(int _userID, int _charID, int _itemID)
    {
        StartCoroutine(db.AddItem(_userID, _charID, _itemID));
    }

    public void DBRequests(int _id, int _type, Packet _packet)
    {
        switch (_type)
        {
            case (int) DataBaseRequests.Characters:
                Debug.Log("Request for characters:" + _id);
                //StartCoroutine(db.GetCharacters(_from.ToString(), ServerSend.ReturnDatabaseRequest_Characters));
                break;
            case (int) DataBaseRequests.Items:
                StartCoroutine(db.GetItems(Server.clients[_id].charID, ReturnItemID_Callback));
                break;
            case (int)DataBaseRequests.CharacterCreation:
                //StartCoroutine(db.CreateCharacter(_id.ToString(), _packet.ReadString()));
                break;
            case (int)DataBaseRequests.ItemVerification:
                
                break;


        }
        
    }

    public void ReturnItemID_Callback(int _id, string _json)
    {
        
        StartCoroutine(db.ReturnItemFromJson(_id, _json));
    }

    
}

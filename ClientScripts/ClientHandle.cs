using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;

public class ClientHandle : MonoBehaviour
{

   
   

    public static void Welcome(Packet _package)
    {
        string _msg = _package.ReadString();
        int _id = _package.ReadInt();

        Debug.Log($"Message from Server: {_msg}");
        Debug.Log("Server ID set is and mine is now:" + _id);
        Client.instance.myId = _id;
        Account.instance.Initialize(_id, 0, 0);
        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        

        //for full version
       ClientSend.WelcomeReceived(Account.instance.dbID, Account.instance.charID);


        ClientSend.CharacterLogIn();
        Inventory.instance.CreateItems();
        
    }

    
    public static void PingHandle(Packet _packet)
    {
        //Debug.Log("Length: " + _packet.Length());
        int _packetID = _packet.ReadInt();
        if (GameManager.instance.lastPacketID >= _packetID)
        {
            Debug.Log("Packet Loss");
        }
        GameManager.instance.lastPacketID = _packetID;
        
        ClientSend.PingReturn();
    }

    public static void LoadScene(Packet _packet)
    {
        try
        {
            Main.instance.LaunchToGame();
        }
        catch(Exception ex)
        {
            
            Debug.Log("Failed Launch into Game");
            Debug.Log(ex);
        }
    }


    #region Player Handles
    public static void SpawnPlayer(Packet _packet)
    {
        
        int _id = _packet.ReadInt();
        string _username = "empty";
        try
        {
            _username = _packet.ReadString();
        }
        catch
        {

        }
        Debug.Log("Spawning: " + _id);
        Vector3 _postion = _packet.ReadVector3();

        Quaternion _rotation = _packet.ReadQuaternion();
        string _characteristcs = _packet.ReadString();
        int[] equipment = new int[_packet.ReadInt()];
        for (int i = 0; i < equipment.Length; i++)
        {
            equipment[i] = _packet.ReadInt();
        }


        GameManager.instance.SpawnPlayer(_id,_username , _postion, _rotation, _characteristcs, equipment);
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        if (GameManager.players.ContainsKey(_id))
        {
            if (GameManager.players[_id].ReadyToPlay)
                GameManager.players[_id].SetPosition(_position);
            else
                Debug.Log("Player Position update: ID->" + _id);
        }
    }

    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _rotation = _packet.ReadFloat();
        if (GameManager.players.ContainsKey(_id))
        {
            Quaternion _rot = GameManager.players[_id].transform.rotation;
            GameManager.players[_id].transform.rotation = new Quaternion(_rot.x, _rotation, _rot.z, _rot.w);
        }
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Destroy(GameManager.players[_id].gameObject);
        GameManager.players.Remove(_id);
    }

    public static void StartIntroduction(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _time = _packet.ReadFloat();
        if (GameManager.players.ContainsKey(_id))
            GameManager.players[_id].role.StartIntroduction(_time);
    }


    public static void PlayerHealth(Packet _packet)
    {
        int _id = _packet.ReadInt();
        float _health = _packet.ReadInt();
        Debug.Log("Health: " + _health);
        GameManager.players[_id].SetHealth(_health);
    }

    public static void PlayerRespawned(Packet _packet)
    {
        int _id = _packet.ReadInt();
        GameManager.players[_id].Respawn();
    }

    #endregion Player Handles


    #region Items

    public static void CreateItemSpawner(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        Vector3 _spawnerPosition = _packet.ReadVector3();
        bool _hasItem = _packet.ReadBool();

       GameManager.instance.CreateItemSpawner(_spawnerId, _spawnerPosition, _hasItem);
    }

    public static void ItemSpawned(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();

        GameManager.itemSpawners[_spawnerId].ItemSpawned();
    }

    public static void ItemPickedUp(Packet _packet)
    {
        int _spawnerId = _packet.ReadInt();
        

        GameManager.itemSpawners[_spawnerId].ItemPickedUp();
        
    }

    #endregion Items

    #region Projectile

    public static void SpawnProjectile(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        int _thrownByPlayer = _packet.ReadInt();

        GameManager.instance.SpawnProjectile(_projectileId, _position, _thrownByPlayer);
        GameManager.players[_thrownByPlayer].itemCount--;
    }

    public static void ProjectilePosition(Packet _packet)
    {
        int _projectileId = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();

        GameManager.projectiles[_projectileId].transform.position = _position;
    }

    public static void ProjectileExploded(Packet _packet)
    {
        int _projectileId = 0;
        Debug.Log("Client Handle Explosion");
        
        
       _projectileId = _packet.ReadInt();
        //int _damage = _packet.ReadInt();

        //Vector3 _position = _packet.ReadVector3();

        GameManager.projectiles[_projectileId].End();
    }

    #endregion Projectile


    #region NPC

    public static void NPCHandle(Packet _packet)
    {
        Debug.Log("NPC HAndle");
        int _type = _packet.ReadInt();
        int id = _packet.ReadInt();
        switch (_type)
        {
            case (int)NPCUpdate.Spawn:
                
                Vector3 pos = _packet.ReadVector3();
                int model = _packet.ReadInt();
                bool friendly = _packet.ReadBool();
                GameObject go = Instantiate(Resources.Load<GameObject>("NPC") as GameObject,pos, Quaternion.identity);
                go.GetComponent<Interactable>().SetID(id);
                if(go != null)
                {
                    GameManager.NPCs.Add(id, go.GetComponent<NPC>());
                }
                break;
            case (int)NPCUpdate.Destroy:
                GameManager.instance.DestroyNPC(id);
                break;
            case (int)NPCUpdate.SetDesintation:
                Vector3 _pos = _packet.ReadVector3();
                GameManager.instance.SetNPCDestination(id,_pos);
                break;
            case (int)NPCUpdate.Health:
                GameManager.NPCs[id].UpdateHealth(_packet.ReadInt());
                    
                break;
            case (int)NPCUpdate.Animation:
                int _state = _packet.ReadInt();
                Debug.Log("Incoming state: " + _state);
                GameManager.NPCs[id].SetAnimation(_state);
                break;
        }
    }

    public static void NPCPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        if (GameManager.NPCs.ContainsKey(_id))
        {
            GameManager.NPCs[_id].gameObject.transform.position = _packet.ReadVector3();
            GameManager.NPCs[_id].gameObject.transform.localEulerAngles = new Vector3(GameManager.NPCs[_id].gameObject.transform.rotation.eulerAngles.x, _packet.ReadFloat(), GameManager.NPCs[_id].gameObject.transform.rotation.eulerAngles.z);
        }
    }


    #endregion NPC


    #region Chat
    public static void ReceieveWorldChat(Packet _packet)
    {

        Debug.Log("message recieved");
        int _id = _packet.ReadInt();
        int _channel = _packet.ReadInt();
        string _msg = _packet.ReadString();
        UIManager.instance.ReceiveWorldChat(_id,_channel,_msg);
    }


    #endregion Chat


    #region Ability
    public static void RecieveAbility(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _ability = _packet.ReadInt();
        
       GameManager.players[_id].role.LoadAbility(_ability);
    }

    public static void SetTarget(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _targetID = _packet.ReadInt();
        bool isNPC = _packet.ReadBool();
        if(_targetID == -1)
        {
            GameManager.players[_id].Target = null;
            UIManager.instance.DisableTargetPanel();
            return;
        }
        if (isNPC) {
            GameManager.players[_id].Target = GameManager.NPCs[_targetID];
            if(Client.instance.myId == _id)
            {
                UIManager.instance.SetTargetPanel(GameManager.NPCs[_targetID]);
            }
        }
        else
        {
            GameManager.players[_id].Target = GameManager.players[_targetID];
            if (Client.instance.myId == _id)
            {
                UIManager.instance.SetTargetPanel(GameManager.players[_targetID]);
            }
        }
    }

    #endregion Ability

    #region Animation
    public static void UpdatePlayerAnimation(Packet _packet)
    {
        int _from = _packet.ReadInt();
        //GameManager.players[_from].role.UpdateAnimator(_packet.ReadFloat(), _packet.ReadFloat(), _packet.ReadBool());
    }

    #endregion Animation



    #region Database

    public static void DBReturn(Packet _packet)
    {
        int _type = _packet.ReadInt();
        string json;
        switch (_type) 
        {
            case (int)DataBaseRequests.Characters:
                json = _packet.ReadString();
                
                CharacterManager.instance.RequestReturned(json);
                break;
            case (int)DataBaseRequests.Items:
                bool complete = _packet.ReadBool();
                if (complete) Debug.Log("Finished");
                json = _packet.ReadString();
                //Debug.Log(json);
                Inventory.instance.RequestReturned(json);
                break;
            case (int)DataBaseRequests.CharacterCreation:
                bool success = _packet.ReadBool();
                if (success)
                {
                    Main.instance.login.ReturnToCharacterSelection();
                }
                else
                {
                    Main.instance.ErrorText("Creation Failed");
                }
                break;
            case (int)DataBaseRequests.ItemVerification:
                if (_packet.ReadBool())
                {
                    Main.instance.FinishALoad(true);
                }
                else
                {
                    Main.instance.ItemVersion = _packet.ReadFloat();
                    
                    //HERE NEED TO LOAD NEW ONES

                    Main.instance.FinishALoad(true);
                }
                break;


        
        }

        
        
    }

    #endregion Database

    #region Client Login
    public static void HandleLogin(Packet _packet)
    {
        if (_packet.ReadBool())
        {
            Main.instance.login.LoginSuccessful();
        }
        else
        {
            Main.instance.login.LoginFailed();
        }
    }

    public static void HandleCharacters(Packet _packet)
    {
        string json = _packet.ReadString();

        CharacterManager.instance.RequestReturned(json);
    }
    public static void HandleCharacterCreation(Packet _packet)
    {
        if (_packet.ReadBool())
        {
            Main.instance.login.CreationCallback(true);
        }
        else
        {
            Main.instance.login.CreationCallback(false);
        }
    }

    public static void LaunchGranted(Packet _packet)
    {

        Debug.Log("launch Granted");
        int dbID = _packet.ReadInt();
        int charID = _packet.ReadInt();
        
        Account.instance.Initialize(Client.instance.myId,dbID, charID);
       // Account.instance.Initialize(_packet.ReadInt(), _packet.ReadInt());

        Main.instance.LaunchToGame(); 
        
    }


    #endregion Client Login

}



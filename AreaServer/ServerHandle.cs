using System.Collections;
using System.Collections.Generic;
using System.Net;
using System;
using System.Net.Sockets;
using UnityEngine;

public class ServerHandle 
{
    
    public static void WelcomeRecieved(int _fromClient, Packet _packet)
    {
        //Server.clients[_fromClient].id = _fromClient;
        Server.clients[_fromClient].dbID = _packet.ReadInt();
        Server.clients[_fromClient].charID = _packet.ReadInt();
        Debug.Log(Server.clients[_fromClient].dbID + " , " + Server.clients[_fromClient].charID.ToString());

        Console.Write($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");

        Server.toPing.Add(_fromClient);
        //Server.clients[_fromClient].SendIntogame("Test");
       
    }

    public static void PingRecieve(int _fromClient, Packet _packet)
    {
        PingTracker.EndTime(_fromClient, Time.time * 1000);
        NetworkManager.instance.AddPingLog();
    }

    public static void SetTarget(int _from, Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _targetID = _packet.ReadInt();
       
        bool isNPC = _packet.ReadBool();
        Debug.Log("isNPC: " + isNPC);
        if (_targetID == -1)
        {
            Server.clients[_from].player.Target = null;
        }
        else if (isNPC)
        {
            Server.clients[_id].player.Target = NPC.NPCs[_targetID];
            Debug.Log(NPC.NPCs[_targetID].gameObject.name);
            //ServerSend.SendTargetSet(_from, _targetID, isNPC);
        }
        else
        {
            Server.clients[_from].player.Target = Server.clients[_targetID].player;
            Debug.Log(Server.clients[_targetID].player.gameObject.name);

        }
        ServerSend.SendTargetSet(_id, _targetID, isNPC);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        Debug.Log("Recieved input from: "+_fromClient);
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
       Quaternion _Yrotation = _packet.ReadQuaternion();

        Server.clients[_fromClient].player.transform.rotation = _Yrotation;
        Server.clients[_fromClient].player.SetInput(_inputs, 0);//_rotation);
    }



    public static void CharacterSpawn(int _fromClient, Packet _packet)
    {
        int charID = _packet.ReadInt();
        Debug.Log("SPAWN CHARACTER: ID -> " + charID);
       // Server.clients[_fromClient].player.characterID = charID;
        Server.clients[_fromClient].charLoggedIn = true;
       // NetworkManager.instance.GetPlayerCharacteristics(Server.clients[_fromClient].player);
        Debug.Log(_fromClient + " Spawned into game");
        NetworkManager.instance.LoadCharacterVariables(_fromClient);
        //Server.clients[_fromClient].SendIntogame(_fromClient.ToString());
        
        
      
    }

    


    public static void PlayerShoot(int _fromClient, Packet _packet)
    {
        Vector3 _shootDirection = _packet.ReadVector3();

        if(Server.clients[_fromClient].player == null || _shootDirection == null)
        {
            Debug.Log("Client is null -> Next open spot: " + Server.NextOpenSpot());
            return;
        }
        Server.clients[_fromClient].player.Shoot(_shootDirection);
    }

    public static void PlayerThrowItem(int _fromClient, Packet _packet)
    {
        Vector3 _throwDirection = _packet.ReadVector3();

        Server.clients[_fromClient].player.ThrowItem(_throwDirection);
    }

    public static void SendWorldChatUDP(int _fromClient, Packet _packet)
    {
        
        int _id = _packet.ReadInt();
        int _channel = _packet.ReadInt();
        string _msg = _packet.ReadString();
        ChatLogWriter.LogChat(_msg, _id);
        if (_channel == (int)ChatChannel.PM)
        {
            ServerSend.SendPMChat(_fromClient, _id, _msg);
        }
        else
        {
            ServerSend.WorldChat(_id,_channel, _msg);
        }
    }

    public static void AbilityUse(int _fromClient, Packet _packet)
    {
        
        int _ability = _packet.ReadInt();   
        Server.clients[_fromClient].player.UseAbility(_ability);
        
    }


    public static void DatabaseRequest(int _fromClient, Packet _packet)
    {
        int _id = _packet.ReadInt();
        int dbRequest = _packet.ReadInt();
        NetworkManager.instance.DBRequests(_fromClient, dbRequest, _packet);
    }

    public static void CharacterAnimationUpdate(int _fromClient, Packet _packet)
    {
        //ServerSend.UpdateCharacterAnimations(_fromClient, _packet);
    }

    public static void Equip(int _fromClient, Packet _packet)
    {
        int _itemID = _packet.ReadInt();
        int _itemReplaced = _packet.ReadInt();
        Server.clients[_fromClient].player.EquipItem(_itemID, _itemReplaced);
    }



    ///World Server Handles
    ///

    public static void ConnectionMessage(Packet _packet)
    {
        Debug.Log(_packet.ReadString());
        NetworkManager.instance.StartAreaServer();
        NetworkManager.instance.isOnline = true;
    }




    /////
}

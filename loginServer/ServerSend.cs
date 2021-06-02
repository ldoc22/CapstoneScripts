using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerSend
{
    #region TCP Sends
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }



    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }
        }
    }
    #endregion TCP Sends


    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.Connected))
        {
            Debug.Log("Sent Welcome Message");
            _packet.Write(_toClient);
            _packet.Write(_msg);
            

            SendTCPData(_toClient, _packet);
        }
    }

    public static void PlayerDisconnected(int _toClient)
    {
        using (Packet _packet = new Packet((int)ServerPackets.Disconnected))
        {
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);


        }
    }

    public static void LoginSuccessful(int id, int _responseID)
    {
        Debug.Log("Suceessful: Old - " + id + "  New: " + _responseID);
        Server.clients[id].dbID = _responseID;
        using (Packet _packet = new Packet((int)ServerPackets.LoginResponse))
        {
            _packet.Write(true);
            
            SendTCPData(id, _packet);
       
        }
    }

    public static void LoginFailure(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.LoginResponse))
        {
            _packet.Write(false);
            SendTCPData(_id, _packet);

        }
    }

    public static void RegisterFailure(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.LoginResponse))
        {
            _packet.Write(false);
            SendTCPData(_id, _packet);

        }
    }

    public static void ReturnCharacterRequest(int _to, string json)
    {
        using (Packet _packet = new Packet((int)ServerPackets.Characters))
        {
            _packet.Write(json);
            SendTCPData(_to, _packet);

        }
    }

    public static void CreateCharacterResponse(int _to, bool _success)
    {
        using (Packet _packet = new Packet((int)ServerPackets.CreateCharacter))
        {
            _packet.Write(_success);
            SendTCPData(_to, _packet);

        }
    }

    public static void LaunchToGame(int _to, int _charID)
    {
        Server.clients[_to].characterID = _charID;

        using (Packet _packet = new Packet((int)ServerPackets.Launch))
        {
           
            _packet.Write(Server.clients[_to].dbID);
            _packet.Write(Server.clients[_to].characterID);
            SendTCPData(_to, _packet);
        }
        ServerSend.PlayerDisconnected(_to);
    }

    public static void ReturnCharacterCreation(int _id, bool _success)
    {
        using(Packet _packet = new Packet((int)ServerPackets.Launch))
        {
            _packet.Write(_success);
            SendTCPData(_id, _packet);
        }
    }
}

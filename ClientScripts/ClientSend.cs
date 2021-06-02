using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
   private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    public static void WelcomeReceived(int _id, int _charID)
    {

        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(_id);
            _packet.Write(_charID);
            
            SendTCPData(_packet);
        }
    }

    public static void PingReturn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.ping))
        {
            SendUDPData(_packet);
        }
    }


    public static void PlayerMovement(bool [] _inputs)
    {
        
        using (Packet _packet = new Packet((int)ClientPackets.playerMovement))
        {
            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }
            // _packet.Write(GameManager.players[Client.instance.myId].transform.rotation.y);
            Debug.Log(Client.instance.myId + ":trying to send");
            try
            {
                _packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            }catch
            {
                Debug.Log("GameManager.players[Client.instance.myId] is null");
            }


            //SendTCPData(_packet);
            SendUDPData(_packet);
          
        }
    }

    public static void PlayerShoot(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerShoot))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void PlayerThrowItem(Vector3 _facing)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playerThrowItem))
        {
            _packet.Write(_facing);

            SendTCPData(_packet);
        }
    }

    public static void Equip(int _itemID, int _removingID)
    {

        using (Packet _packet = new Packet((int)ClientPackets.Equip))
        {
            _packet.Write(_itemID);
            _packet.Write(_removingID);
            SendTCPData(_packet);
        }
    }



    public static void SendWorldChat(int _channel, string _msg)
    {
        using (Packet _packet = new Packet((int)ClientPackets.worldChat))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_channel);
            _packet.Write(_msg);

            SendUDPData(_packet);
        }
    }

    public static void SendSetTarget(int _targetID, bool isNPC)
    {
       
        using (Packet _packet = new Packet((int)ClientPackets.SetTarget))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(_targetID);
            _packet.Write(isNPC);
            SendTCPData(_packet);
        }
    }

    public static void SendAbilityToServer(int _ability)
    {
        if (GameManager.players[Client.instance.myId].Target == null)
        {
            ShowOutput.instance.QueueOutputMessage("No Target");
        }

        
        using (Packet _packet = new Packet((int)ClientPackets.AbilityCast))
        {
            //_packet.Write(Client.instance.myId);
            _packet.Write(_ability);
            //_packet.Write(_targetID);
            //_packet.Write(isNPC);
            SendTCPData(_packet);
      
        }
    }

    public static void RequestCharacters()
    {
        using (Packet _packet = new Packet((int)ClientLoginPackets.Characters))
        {
            Debug.Log("Requesting ");
            SendTCPData(_packet);

        }
    }

    public static void CharacterLogIn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.CharacterLoggedIn))
        {
            Debug.Log("Sent : " + Account.instance.charID);
            _packet.Write(Account.instance.charID); 
            //_packet.Write();

            SendTCPData(_packet);

        }
    }

    public static void RequestItems()
    {
        using (Packet _packet = new Packet((int)ClientPackets.dbRequest))
        {
            _packet.Write(0);//int.Parse(Account.instance.character.dbID));
            _packet.Write((int)DataBaseRequests.Items);
            SendTCPData(_packet);

        }
    }

    

   

    public static void UpdateAnimation(float speedX, float speedY, bool jumping)
    {
        using (Packet _packet = new Packet((int)ClientPackets.characterAnimation))
        {
            _packet.Write(speedX);
            _packet.Write(speedY);
            _packet.Write(jumping);
            
            SendTCPData(_packet);

        }
    }


    #region Login Sends



    public static void Login(string _username, string _password)
    {
        using (Packet _packet = new Packet((int)ClientLoginPackets.Login))
        {
            _packet.Write(_username);
            _packet.Write(_password);
            SendTCPData(_packet);

        }
    }

    public static void Register(string _username, string _password, string _confirmPass)
    {
        using (Packet _packet = new Packet((int)ClientLoginPackets.RegisterUser))
        {
            _packet.Write(_username);
            _packet.Write(_password);
            _packet.Write(_confirmPass);
            SendTCPData(_packet);

        }
    }

    public static void RequestGameEntrance()
    {
        using (Packet _packet = new Packet((int)ClientLoginPackets.RequestToLaunch))
        {
            _packet.Write(Account.instance.charID);
            SendTCPData(_packet);

        }
    }

    public static void CreateCharacter(string list)
    {
        using (Packet _packet = new Packet((int)ClientLoginPackets.RequestCreation))
        {
            _packet.Write(list);
            SendTCPData(_packet);

        }
    }


    #endregion Login Sends



}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ServerSend
{
    #region TCP Sends
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        Debug.Log("Sent to Client: " + _toClient);
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
        PacketTracker.instance.UpTotal(1);
    }



    private static void SendTCPDataToAll(Packet _packet, bool _requiresLoggedIn)
    {
        if (_requiresLoggedIn)
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (Server.clients[i].charLoggedIn)
                {
                    Server.clients[i].tcp.SendData(_packet);
                    PacketTracker.instance.UpTotal(1);
                }
            }
        }
        else
        {
            _packet.WriteLength();
            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                Server.clients[i].tcp.SendData(_packet);
                PacketTracker.instance.UpTotal(1);
            }
        }
    }

    

    private static void SendTCPDataToAllInArea(Packet _packet, bool _requiresLoggedIn, int _playerID)
    {
       
        _packet.WriteLength();
        for (int i = 0; i <= Server.clients[_playerID].player.players.Count; i++)
        {
            Server.clients[Server.clients[_playerID].player.players[i]].tcp.SendData(_packet);
            PacketTracker.instance.UpTotal(1);
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
                PacketTracker.instance.UpTotal(1);
            }
        }
    }
    #endregion TCP Sends

    #region UDP Sends
    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        if (!Server.clients[_toClient].charLoggedIn) return;
        Server.clients[_toClient].udp.SendData(_packet);
        PacketTracker.instance.UpTotal(1);
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (Server.clients[i].charLoggedIn)
            {
                Server.clients[i].udp.SendData(_packet);
                Debug.Log("Sent to Client: " + Server.clients[i].id);
                PacketTracker.instance.UpTotal(1);
            }
        }
    }

    private static void SendUDPDataToAllInArea(Packet _packet, int _playerID)
    {
        _packet.WriteLength();
        for (int i = 0; i <= Server.clients[_playerID].player.players.Count; i++)
        {
            Server.clients[Server.clients[_playerID].player.players[i]].udp.SendData(_packet);
            PacketTracker.instance.UpTotal(1);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                if (Server.clients[i].charLoggedIn)
                {
                    Server.clients[i].udp.SendData(_packet);
                    PacketTracker.instance.UpTotal(1);
                }
            }
        }
    }
    #endregion UDP Sends


    public static void Welcome(int _toClient, string _msg)
    {

        PacketTracker.instance.Increment((int)ServerPackets.welcome);

        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            Debug.Log("Sent Weclome Message to user: " + Server.clients[_toClient].id);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void Ping(int _client)
    {
        PingTracker.NewPing(_client);
        using (Packet _packet = new Packet((int)ServerPackets.ping))
        {
            PingTracker.StartTime(_client, Time.time * 1000);
            _packet.Write(Server.clients[_client].packetID++);
            SendUDPData(_client, _packet);
        }
        
    }

    public static void LoadScene(int _to, int _scene)
    {
        using (Packet _packet = new Packet((int)ServerPackets.LoadScene))
        {
            _packet.Write(_scene);
            SendTCPData(_to ,_packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        
        PacketTracker.instance.Increment((int)ServerPackets.spawnPlayer);
        
        Debug.Log("Spawning Player: " + _player.id);
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.Characteristics);
            int size = _player.GetEquipmentSize();
            _packet.Write(size);
            for (int i = 0; i < size; i++)
            {
                if(_player.Equipment[i] != 0)
                {
                    _packet.Write(_player.Equipment[i]);
                }
            }

            SendTCPData(_toClient, _packet);
            //SendTCPDataToAllInArea(_packet, true, _toClient);
        }
    }

    public static void PlayerPostion(Player _player)
    {

        PacketTracker.instance.Increment((int)ServerPackets.playerPosition);
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            SendUDPDataToAll(_packet);
        }
    }
    public static void N_PlayerPostion(Player _player)
    {

        PacketTracker.instance.Increment((int)ServerPackets.playerPosition);
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);

            //SendUDPDataToAll(_packet);
            foreach (int i in _player.players)
            {
                SendUDPData(i, _packet);
            }
        }
    }

    public static void PlayerRotation(Player _player)
    {

        PacketTracker.instance.Increment((int)ServerPackets.playerRotation);

        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation.y);

            SendUDPDataToAll(_packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {

        PacketTracker.instance.Increment((int)ServerPackets.playerDisconnected);

        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet, false);


        }
    }

    public static void PlayerHealth(Player _player)
    {
        PacketTracker.instance.Increment((int)ServerPackets.playerHealth);
        using (Packet _packet = new Packet((int)ServerPackets.playerHealth))
        {
            _packet.Write(_player.id);
            
            _packet.Write(_player.health);

            SendTCPDataToAll(_packet, true);
        }
    }

    public static void PlayerRespawned(Player _player)
    {
        PacketTracker.instance.Increment((int)ServerPackets.playerDeath);

        using (Packet _packet = new Packet((int)ServerPackets.playerDeath))
        {
            _packet.Write(_player.id);

            SendTCPDataToAll(_packet, true);
        }
    }


    public static void CreateItemSpawner(int _toClient, int _spawnerId, Vector3 _spawnerPosition, bool _hasItem)
    {

        PacketTracker.instance.Increment((int)ServerPackets.createItemSpawner);

        using (Packet _packet = new Packet((int)ServerPackets.createItemSpawner))
        {
            _packet.Write(_spawnerId);
            _packet.Write(_spawnerPosition);
            _packet.Write(_hasItem);

            SendTCPData(_toClient, _packet);

        }
    }

    public static void ItemSpawned(int _spawnerId)
    {

        PacketTracker.instance.Increment((int)ServerPackets.itemSpawned);

        using (Packet _packet = new Packet((int)ServerPackets.itemSpawned))
        {
            _packet.Write(_spawnerId);

            SendTCPDataToAll(_packet, true);
        }
    }

    public static void SpawnNPC(int _sendTo, NPC _npc)
    {
        PacketTracker.instance.Increment((int)ServerPackets.updateNPC);

        using (Packet _packet = new Packet((int)ServerPackets.updateNPC))
        {
            _packet.Write((int)NPCUpdate.Spawn);
            _packet.Write(_npc.AI_ID);
            _packet.Write(_npc.gameObject.transform.position);
            _packet.Write(_npc.AI_Visuals);
            _packet.Write(false);
            SendTCPData(_sendTo ,_packet);
                
        }
    }

    public static void RemoveNPC(int _sendTo, int _npcID)
    {
        PacketTracker.instance.Increment((int)ServerPackets.updateNPC);

        using (Packet _packet = new Packet((int)ServerPackets.updateNPC))
        {
            _packet.Write((int)NPCUpdate.Destroy);
            _packet.Write(_npcID);
            SendTCPData(_sendTo, _packet);
        }
    }
    public static void SetNPCDestination(int _id , Vector3 _pos)
    {
        PacketTracker.instance.Increment((int)ServerPackets.updateNPC);

        using (Packet _packet = new Packet((int)ServerPackets.updateNPC))
        {
            _packet.Write((int)NPCUpdate.SetDesintation);
            _packet.Write(_id);
            _packet.Write(_pos);
            SendTCPDataToAll(_packet, true);

        }
    }

    public static void SetNPCAnimation(int _id, int _state)
    {

       
        using (Packet _packet = new Packet((int)ServerPackets.updateNPC))
        {
            Debug.Log("Sending animation");
            _packet.Write((int)NPCUpdate.Animation);
            _packet.Write(_id);
            _packet.Write(_state);
            SendTCPDataToAll(_packet, true);
           
        }
    }

    public static void NPCPosition(int _id, Vector3 _pos, float _yRot)
    {
        PacketTracker.instance.Increment((int)ServerPackets.NPCposition);

        using (Packet _packet = new Packet((int)ServerPackets.NPCposition))
        {
            _packet.Write(_id);
            _packet.Write(_pos);
            _packet.Write(_yRot);

            SendUDPDataToAll(_packet);

        }
    }

    public static void N_NPCPosition(int _id, Vector3 _pos, float _yRot)
    {
        PacketTracker.instance.Increment((int)ServerPackets.NPCposition);

        using (Packet _packet = new Packet((int)ServerPackets.NPCposition))
        {
            _packet.Write(_id);
            _packet.Write(_pos);
            _packet.Write(_yRot);
            foreach (int i in NPC.NPCs[_id].players)
            {
                SendUDPData(i, _packet);
            }

        }
    }


    public static void UpdateNPCHealth(int _npcID, int _health)
    {
        PacketTracker.instance.Increment((int)ServerPackets.NPCposition);

        using (Packet _packet = new Packet((int)ServerPackets.updateNPC))
        {
            _packet.Write((int)NPCUpdate.Health);
            _packet.Write(_npcID);
            _packet.Write(_health);

            SendUDPDataToAll(_packet);

        }
    }

    public static void ItemPickedUp(int _spawnerId, int _byPlayer)
    {
        PacketTracker.instance.Increment((int)ServerPackets.itemPickedUp);

        NetworkManager.instance.PickedUpItem(_byPlayer, Server.clients[_byPlayer].player.characterID, ItemSpawner.spawners[_spawnerId].itemNumber);

        using (Packet _packet = new Packet((int)ServerPackets.itemPickedUp))
        {
            _packet.Write(_spawnerId);
            
            SendTCPDataToAll(_packet, true);
        }
    }

    public static void SpawnProjectile(Projectile _projectile, int _thrownBy)
    {
        PacketTracker.instance.Increment((int)ServerPackets.spawnProjectile);
        using (Packet _packet = new Packet((int)ServerPackets.spawnProjectile))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);
            _packet.Write(_thrownBy);

            SendTCPDataToAll(_packet, true);
        }
    }

    public static void ProjectilePosition(Projectile _projectile)
    {
        PacketTracker.instance.Increment((int)ServerPackets.projectilePosition);

        using (Packet _packet = new Packet((int)ServerPackets.projectilePosition))
        {
            _packet.Write(_projectile.id);
            _packet.Write(_projectile.transform.position);

            SendTCPDataToAll(_packet, true);
        }
    }

    public static void ProjectileExploded(Projectile _projectile, int _damage)
    {

        PacketTracker.instance.Increment((int)ServerPackets.projectileExplosion);

        using (Packet _packet = new Packet((int)ServerPackets.projectileExplosion))
        {
            
            _packet.Write(_projectile.id);
            int _dmg = DamageCalculator.GetDamage(_projectile.thrownByPlayer, _projectile.target.GetComponent<NPC>().AI_ID, 10, true);
            _packet.Write(_dmg);
            NPC.NPCs[_projectile.target.GetComponent<NPC>().AI_ID].TakeDamage(_dmg);

            Debug.Log("Explode");
            SendTCPDataToAll(_packet, true);
        }
    }




    public static void WorldChat(int _fromClient, int _channel ,string _msg)
    {

        PacketTracker.instance.Increment((int)ServerPackets.worldChatRecieve);

        Debug.Log($"Message to all users from {_fromClient} : {_msg}");
        using (Packet _packet = new Packet((int)ServerPackets.worldChatRecieve))
        {
            _packet.Write(_fromClient);
            _packet.Write(_channel);
            _packet.Write(_msg);

            SendUDPDataToAll( _packet);

        }

    }

    public static void SendPMChat(int _fromClient, int _to, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.worldChatRecieve))
        {
           
            _packet.Write(_fromClient);
            _packet.Write((int)ChatChannel.PM);
            _packet.Write(_msg);
            SendTCPData(_fromClient, _packet);
            SendTCPData(_to, _packet);
            

        }

       

    }

    public static void AbilityUse(int _fromClient, int _ability)
    {
        PacketTracker.instance.Increment((int)ServerPackets.AbilityUse);

        Debug.Log($"Casting ability from {_fromClient}  ability:{_ability}");
        using (Packet _packet = new Packet((int)ServerPackets.AbilityUse))
        {
            _packet.Write(_fromClient);
            _packet.Write(_ability);
            
            SendUDPDataToAll(_packet);
        }
    }

    public static void StartIntroduction(int _playerID, float _time)
    {
        using (Packet _packet = new Packet((int)ServerPackets.StartIntroduction))
        {
            _packet.Write(_playerID);
            _packet.Write(_time);
            SendTCPDataToAll(_packet, true);
        }
    }
    

    public static void SendTargetSet(int _from, int _targetID, bool isNPC)
    {
        PacketTracker.instance.Increment((int)ServerPackets.SetTarget);

        
        using (Packet _packet = new Packet((int)ServerPackets.SetTarget))
        {
            _packet.Write(_from);
            _packet.Write(_targetID);
            _packet.Write(isNPC);
            SendTCPDataToAll(_packet, true);
        }
    }

    public static void ReturnDatabaseRequest_Characters(string _toClient, string json)
    {
        PacketTracker.instance.Increment((int)ServerPackets.dbReturn);
        int id = 0;
        try
        {
            id = int.Parse(_toClient);
        }catch
        {
            Debug.Log("Couldnt parse toclient");
        }
        
        using (Packet _packet = new Packet((int)ServerPackets.dbReturn))
        {
            _packet.Write((int)DataBaseRequests.Characters);
            _packet.Write(json);

            SendTCPData(id, _packet);
        }
    }

    public static void ReturnDatabaseRequest_Items(int _toClient, string json, bool complete)
    {
        PacketTracker.instance.Increment((int)ServerPackets.dbReturn);

        
        Debug.Log("Sent: " + json);

        if (json.Contains("0 results")) return;
        using (Packet _packet = new Packet((int)ServerPackets.dbReturn))
        {
            _packet.Write((int)DataBaseRequests.Items);
            _packet.Write(complete);
            _packet.Write(json);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ReturnDatabaseRequest_CharacterCreation(string _toClient, bool successful)
    {
        PacketTracker.instance.Increment((int)ServerPackets.dbReturn);
        using (Packet _packet = new Packet((int)ServerPackets.dbReturn))
        {
            _packet.Write((int)DataBaseRequests.CharacterCreation);
            _packet.Write(successful);

            Debug.Log(_toClient);
            SendTCPData(int.Parse(_toClient), _packet);
        }
    }

    public static void UpdateCharacterAnimations(int _from, Packet packet)
    {
        PacketTracker.instance.Increment((int)ServerPackets.UpdateAnimation);
        using (Packet _packet = new Packet((int)ServerPackets.UpdateAnimation))
        {
            _packet.Write(_from);
            _packet.Write(_packet.ReadFloat());
            _packet.Write(_packet.ReadFloat());
            _packet.Write(_packet.ReadBool());
  
            SendUDPDataToAll(_from, _packet);
        }
    }

    public static void ReturnDatabaseRequest_ItemVerificationn(string _toClient, bool successful)
    {
        PacketTracker.instance.Increment((int)ServerPackets.dbReturn);
        using (Packet _packet = new Packet((int)ServerPackets.dbReturn))
        {
            _packet.Write((int)DataBaseRequests.ItemVerification);
            _packet.Write(successful);
            if (!successful) _packet.Write(FileLocations.ItemVersions);
            
            SendTCPData(int.Parse(_toClient), _packet);
            
        }
    }






}
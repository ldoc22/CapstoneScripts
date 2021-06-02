using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public static Dictionary<int, PlayerManager> players = new Dictionary<int, PlayerManager>();
    public static Dictionary<int, ItemSpawner> itemSpawners = new Dictionary<int, ItemSpawner>();
    public static Dictionary<int, ProjectileManager> projectiles = new Dictionary<int, ProjectileManager>();
    public static Dictionary<int, NPC> NPCs = new Dictionary<int, NPC>();
    

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;
    public GameObject itemSpawnerPrefab;
    public GameObject projectilePrefab;
    public int lastPacketID = 0;

    public bool LoginSpoofed;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance of Gamemanager alreadyt exists");
            Destroy(this);
        }



    }

    public PlayerManager GetLocalPlayer()
    {
        return players[Client.instance.myId];
    }

    public void NewNPC(NPC _npc, int _id, bool _friendly)
    {
        _npc.SetID(_id);
        NPCs.Add(_id, _npc);
    }

    public void SetNPCDestination(int _id, Vector3 _pos)
    {
        NPCs[_id].SetDestination(_pos);
    }

    public void SetNPCPosition(int _id, float _percent)
    {
        NPCs[_id].SetLerpPercent(_percent);
    }


    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation, string _characteristics, int [] _equipment)
    {
        GameObject _player;
        Debug.Log("Spawning Player: "+ _id + " , " + Account.instance.ID);
        if(_id == Account.instance.ID)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
            UIManager.instance.SetLoading(false);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }


        _player.GetComponent<PlayerManager>().Initialize(_id, _username, _characteristics, _equipment);
        players.Add(_id, _player.GetComponent<PlayerManager>());

        
    }

    public void CreateItemSpawner(int _spawnerId, Vector3 _position, bool _hasItem)
    {
        GameObject _spawner = Instantiate(itemSpawnerPrefab, _position, itemSpawnerPrefab.transform.rotation);
        _spawner.GetComponent<ItemSpawner>().Initialize(_spawnerId, _hasItem);
        itemSpawners.Add(_spawnerId, _spawner.GetComponent<ItemSpawner>());
    }

    public void SpawnProjectile(int _id, Vector3 _position, int _playerID)
    {
        
        GameObject _projectile = Instantiate(players[_playerID].role.currentAbility.Projectile, _position, Quaternion.identity);
        Debug.Log("Projectile ID set to " + _id);
        _projectile.GetComponent<ProjectileManager>().Initialize(_id);
        projectiles.Add(_id, _projectile.GetComponent<ProjectileManager>());
    }

    public void SpawnNPC(int _id, bool friendly, int type)
    {
        
    }

    public void DestroyNPC(int _id)
    {
        GameObject go = NPCs[_id].gameObject;
        NPCs.Remove(_id);
        Destroy(go);
    }
}

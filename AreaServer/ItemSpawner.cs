using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public static Dictionary<int, ItemSpawner> spawners = new Dictionary<int, ItemSpawner>();
    private static int nextSpawnerId = 1;

    public int spawnerId;
    public bool hasItem;

    public int itemNumber;
    private Collider coll;


    private void Start()
    {
        coll = GetComponent<Collider>();
        hasItem = false;
        spawnerId = nextSpawnerId;
        nextSpawnerId++;
        spawners.Add(spawnerId, this);
        StartCoroutine(SpawnItem());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasItem && other.CompareTag("Player"))
        {
            Player _player = other.GetComponent<Player>();
            if(_player.AttemptPickupItem())
            {
                coll.enabled = false;
                ItemPickedUp(_player.id);
                _player.PickupItem(_player.id,itemNumber);
            }
        }
    }

    private IEnumerator SpawnItem()
    {
        yield return new WaitForSeconds(10f);

        hasItem = true;
        coll.enabled = true;
        ServerSend.ItemSpawned(spawnerId);
    }

    private void ItemPickedUp(int _byPlayer)
    {
        hasItem = false;
        ServerSend.ItemPickedUp(spawnerId, _byPlayer);
        StartCoroutine(SpawnItem());
    }
}

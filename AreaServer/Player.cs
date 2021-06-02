using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Interactable
{

    public int id;
    public string username;

    public CharacterController controller;

    public int dbID;
    public int characterID;
    public int level;
    public Transform shootOrigin;
    public float throwForce = 1000f;
    public bool isBusy;

    public float gravity = -9.81f;


    private float moveSpeed = 5f;
    public float jumpSpeed = 5f;
    private bool[] inputs;
    private float yVelocity = 0;


    public int itemAmount = 0;
    public int maxItemAmount = 3;

    public int[] Inventory;
    public int[] Equipment;
    public string Characteristics;
    public Interactable Target;

    private Vector3 moveDirection = Vector3.zero;

    public List<int> players;
    public List<int> npcs;

    public Collider LoadArea;
    
    public void OnSpawn()
    {
        players = new List<int>();
        npcs = new List<int>();
        players.Add(id);
    }

    private void Start()
    {
        gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
        moveSpeed *= Time.fixedDeltaTime;
        jumpSpeed *= Time.fixedDeltaTime; 

    }

    public void Initialize(int _id, int _dbID, int _charID, string _username)
    {
        id = _id;
        dbID = _dbID;
        characterID = _charID;
        username = "User " + _id + "/" + _dbID + "/" + _charID;
        isBusy = false;
        health = maxHealth;
        inputs = new bool[5];
        Inventory = new int[16];
        Equipment = new int[8];
        
        OnSpawn();

    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("NPC"))
        {
            NPC npc = col.gameObject.GetComponent<NPC>();
            //ServerSend.SpawnNPC(id, npc);
            npc.players.Add(id);
        }else if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            //ServerSend.SpawnPlayer(id, player);
            players.Add(player.id);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.CompareTag("NPC"))
        {
            NPC npc = col.gameObject.GetComponent<NPC>();
            //ServerSend.RemoveNPC(id, npc.AI_ID);
            npc.players.Remove(id);
        }
        else if (col.gameObject.CompareTag("Player"))
        {
            Player player = col.gameObject.GetComponent<Player>();
            //ServerSend.PlayerRespawned(player);
            players.Remove(player.id);
        }
    }

   

    public void FixedUpdate()
    {


        if (health <= 0)
        {
            return;
        }
        Vector2 _inputDirection = Vector2.zero;
        if (inputs[0])
        {
            _inputDirection.y += 1;
        }
        if (inputs[1])
        {
            _inputDirection.y -= 1;
        }
        if (inputs[2])
        {
            _inputDirection.x -= 1;
        }
        if (inputs[3])
        {
            _inputDirection.x += 1;
        }

        Move(inputs);

    }

    private void Move(Vector2 _inputDirection)
    {



        Vector3 _moveDirection = new Vector3(_inputDirection.x, 0, _inputDirection.y);
        _moveDirection *= moveSpeed;

        if (controller.isGrounded)
        {
            yVelocity = 0f;
            if (inputs[4])
            {
                yVelocity = jumpSpeed;
            }
        }
        yVelocity += gravity;

        _moveDirection.y = yVelocity;

        
        controller.Move(_moveDirection);



        ServerSend.PlayerPostion(this);
        //ServerSend.N_PlayerPostion(this);
        ServerSend.PlayerRotation(this);
        

    }

    private void Move(bool [] _inputs)
    {
        Vector3 _dir = Vector3.zero;
        if (inputs[0])
        {
            _dir = transform.forward;
        }
        else if (inputs[1])
        {
            _dir = transform.forward * -1;
        }

        controller.Move(_dir * .2f);
        ServerSend.PlayerPostion(this);
        //ServerSend.N_PlayerPostion(this);
        ServerSend.PlayerRotation(this);
    }


    public void SetInput(bool[] _inputs, float _rotation)
    {
        inputs = _inputs;
        //transform.rotation = new Quaternion(transform.rotation.x, _rotation, transform.rotation.z, transform.rotation.w);
    }
    public void Shoot(Vector3 _viewDirection)
    {
        if (health <= 0)
        {
            return;
        }
        if (Physics.Raycast(shootOrigin.position, _viewDirection, out RaycastHit _hit, 25f))
        {
            if (_hit.collider.CompareTag("Player"))
            {
                _hit.collider.GetComponent<Player>().TakeDamage(50);
            }
        }
    }

    public void ThrowItem(Vector3 _viewDirection)
    {
        if (health <= 0)
        {
            return;
        }
        if (itemAmount > 0)
        {
            itemAmount--;
            NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(_viewDirection, throwForce, id);
        }
    }

    public override void TakeDamage(int _damage)
    {
        float startHealth = health;
        if (health <= 0)
        {
            return;
        }
        health -= _damage;
        if (health <= 0)
        {
            health = 0;
            controller.enabled = false;
            transform.position = new Vector3(0f, 25f, 0f);
            ServerSend.PlayerPostion(this);
            //ServerSend.N_PlayerPostion(this);
            StartCoroutine(Respawn());
        }
        Debug.Log("Player " + id + " Health Changed from " + startHealth + "to " + health);
        ServerSend.PlayerHealth(this);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(5f);

        health = maxHealth;
        controller.enabled = true;
        ServerSend.PlayerRespawned(this);
    }

    public bool AttemptPickupItem()
    {
        if (itemAmount >= maxItemAmount)
        {
            return false;
        }
        itemAmount++;
        return true;
    }

    public void PickupItem(int id, int itemID)
    {
        AddItemToInventory(id);
        //StartCoroutine(NetworkManager.instance.db.AddItem(id, characterID, itemID));
    }
    public void EquipItem(int _toEquip, int _ToRemove)
    {   if(_ToRemove == 0)
        {
            for (int i = 0; i < Equipment.Length; i++)
            {
                if(Equipment[i] == 0)
                {
                    Equipment[i] = _toEquip;
                    RemoveFromInventory(_toEquip);
                    return;
                }
            }
        }
        for (int i = 0; i < Equipment.Length; i++)
        {
            if(Equipment[i] == _ToRemove)
            {
                PickupItem(id, _ToRemove);
                Equipment[i] = _toEquip;
                break;
            }
        }
    }

    public int GetEquipmentSize()
    {
        int count = 0;
        for (int i = 0; i < Equipment.Length; i++)
        {
            if(Equipment[i] != 0)
            {
                count++;
            }
        }
        return count;
    }
    public void AddItemToInventory(int _id)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == 0)
            {
                Inventory[i] = _id;
                Action<string> GetItemCallback = (json) =>
                {
                    ServerSend.ReturnDatabaseRequest_Items(_id, json, true);
                };

                StartCoroutine(DatabaseRequests.instance.GetItem(_id.ToString(), GetItemCallback));
                return;
            }
        }
    }

    public void RemoveFromInventory(int _toRemove)
    {
        for (int i = 0; i < Inventory.Length; i++)
        {
            if (Inventory[i] == _toRemove)
            {
                Inventory[i] = 0;
                return;
            }
        }
    }

    public void UseAbility(int _ability)
    {
       // NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(id);

        

        Ability ability = Resources.Load<Ability>("Abilities/" + _ability) as Ability;
        if (!isBusy)
        {
            Action<bool> completeIntroduction = (completed) =>
            {
                if (completed)
                {
                    ServerSend.AbilityUse(id, _ability);
                   if(ability.isRanged)
                        NetworkManager.instance.InstantiateProjectile(shootOrigin).Initialize(id);
                    else
                    {
                        Target.TakeDamage(ability.damage);
                    }

                    
                    
                }
            };

            StartCoroutine(WaitToCast(ability.IntroductionTime, completeIntroduction));
        }
        
    }

    IEnumerator WaitToCast(float _introTime, Action<bool> _callback)
    {
        yield return new WaitForSeconds(_introTime);
        _callback(true);
    }



    IEnumerator WaitForTime(float _time)
    {
        isBusy = true;
        yield return new WaitForSeconds(_time);
        isBusy = false;
    }

    public bool isInRange(Transform _target, float _dist)
    {
        if (Vector3.Distance(transform.position, _target.position) <= _dist)
        {
            if(Vector3.Dot(transform.TransformDirection(Vector3.forward), _target.position - transform.position) > 0)
            {
                return true;
            }
            else
            {
                Debug.Log("Not facing target");
            }
            
        }
        Debug.Log("Not in range");
        return false;
    }





}

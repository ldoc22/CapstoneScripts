using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Role))]
public class PlayerManager : Interactable
{
    //public int id;
    public string username; 
    public int itemCount;
    public MeshRenderer model;
    public Role role;
    public PlayerController controller;
    public bool isLocal;
    public Interactable Target;
    public bool ReadyToPlay;

    private void Awake()
    {
        ReadyToPlay = false;
    }

    public void Initialize(int _id, string _username, string characteristics, int [] _equipment)
    {
        
        SetID(_id);
        username = _username;
        Health = MaxHealth;
        GetComponent<PlayerCharacter>().LoadCharacteristics(characteristics);
        GetComponent<PlayerCharacter>().LoadEquipment(_equipment);
        role = gameObject.GetComponent<Role>();
        role.Init();
        controller = GetComponent<PlayerController>();
        Debug.Log("Characteristcis: " + characteristics);
       
        
        if(controller == null)
        {
            isLocal = false;
        }
        else
        {
            isLocal = true;
            UIManager.instance.SetCharacterPanelStats(MaxHealth, Health, MaxMana, Mana);
        }
        ReadyToPlay = true;
        //Inventory.instance.CreateItems();
        
    }

    

    public void SetHealth(float _health)
    {
        Health = _health;

        if(Health <= 0f)
        {
            Die();
        }
        UIManager.instance.CharacterHealthPanel.HealthChanged(MaxHealth, Health);
    }

    public void Die()
    {
        model.enabled = false;
    }

    public void Respawn()
    {
        model.enabled = true;
        SetHealth(MaxHealth);
    }
}



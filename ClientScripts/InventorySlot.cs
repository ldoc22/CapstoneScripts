using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGCharacters;

class InventorySlot
{
    public bool inUse;
    public Item item;
    public int inventoryID;


    ///// Item Info
    public int id;
    string name;
    string description;
    int price;
    public string GetInfo()
    {
        return name + "\n" + description + "\n Price: " + price.ToString();
    }
   public InventorySlot()
    {
        id = 0;
    }

    public void Init()
    {
        inUse = false;
        item = null;
        id = 0;
    }

    public void Use()
    {
        if (id == 0 ) return;
        //PlayerCharacter.instance.EquipItem(id);
        Init();
   
    }

    public void SetItem(int _id, string _name, string _description, int _price)
    {
        inUse = true;
        id = _id;
        name = _name;
        description = _description;
        price = _price;
    }

    public void SetItem(ItemSO item)
    {
        inUse = true;
        id = item.dbID;
        name = item.ItemName;
        description = item.description;
        price = item.price;
    }

    

    public int GetItemID()
    {
        if (!inUse) return 0;
        return id;
    }

}



using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Item 
{

    public readonly int ID; //unique id for each item in our inventory
    public readonly int ItemID;// unique id for item in catelog;
    public readonly string ItemName; // Item name;
    public readonly string ItemDescripton;
    public readonly int price;

    public UI_Item(int _slotNumber, int _itemID, string _name, string _description, int _price)
    {
        ID = _slotNumber;
        ItemID = _itemID;
        ItemName = _name;
        ItemDescripton = _description;
        price = _price;
    }
 
    
}

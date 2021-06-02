using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public int id { get; private set; }
    public string UnitName;
    public float Health, MaxHealth;
    public float Mana, MaxMana;
    public bool isNPC;
    
    Vector2 directionChange;
    public void SetID(int _id)
    {
        id = _id;
    }

    public void SetPosition(Vector3 _position)
    {
         //Debug.Log((_position - transform.position).normalized);
        transform.position = _position;
    }
}

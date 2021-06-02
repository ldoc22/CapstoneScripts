using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable :MonoBehaviour
{
    public int health;
    public int maxHealth;

    public int armor;
    public int DamageAttribute;
    protected bool isAlive;

    public virtual void TakeDamage(int _dmg)
    {
        health -= _dmg;
        if(health <= 0)
        {
            isAlive = false;
        }
    }

}

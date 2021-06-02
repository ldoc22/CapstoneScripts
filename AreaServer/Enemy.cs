using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NPC
{

    public int damage;

    Player target;


    

    public void Die()
    {

    }

    private void Start()
    {
        health = maxHealth;
        startMarker = transform;
        //SetLocation(markers[GetCurrentTarget()]);

    }
    private void FixedUpdate()
    {
        if (isAlive)
        {
            GameStep();
        }
    }


}

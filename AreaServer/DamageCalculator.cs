using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator 
{
    public static int GetDamage(int _attackerID, int _targetID, int _abilityDamage, bool _attackerIsAPlayer)
    {
        if (_attackerIsAPlayer)
        {
            return Server.clients[_attackerID].player.DamageAttribute + _abilityDamage - NPC.NPCs[_targetID].armor;
        }
        else
        {
            return Mathf.Abs(Server.clients[_attackerID].player.armor - _abilityDamage + NPC.NPCs[_targetID].DamageAttribute);
        }

    }
}

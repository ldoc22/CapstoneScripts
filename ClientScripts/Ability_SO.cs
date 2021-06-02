using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Ability")]
public class Ability_SO : ScriptableObject
{
    public RoleType role;
    public string AbilityName;
    public AnimationClip animation;
    public GameObject Projectile;
    public float IntroductionTime;
    public int LevelRequirement;
    public float range;
    public bool isRanged;
}

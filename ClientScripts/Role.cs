using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RoleType 
{
    Champion,
    Mage
};


public class Role : MonoBehaviour
{
    enum Abilities
    {
        AutoAttack = 1,
        Taunt
    };

    public Animator anim;
    public bool isBusy;
    public Transform ProjectileSpawnLocation;
    
    bool isArmed;
    public RoleType roleType;
    
    
    enum Animations{
        idle, walk, run, autoattack
    };


    public Ability_SO currentAbility { get; private set; }

    public void LoadAbility(int _abilityID)
    {
        if (!isArmed)
        {
            anim.SetBool("isArmed", true);
        }

        Ability_SO ability = Resources.Load<Ability_SO>("ScriptableObjects/Abilities/" + _abilityID); 
        currentAbility = ability;

        anim.Play(currentAbility.animation.name);

        Action<bool> completeIntroduction = (completed) =>
        {
            if (completed)
            {
                //currentAbility = ability;
                PlayAnimation(ability.animation);
                
            }
        };
        if (ability != null)
        {
           // UIManager.instance.StartIntroduction(ability.IntroductionTime, completeIntroduction); 
        }
    }


    public void AbilityEventTrigger()
    {

        GameObject obj = currentAbility.Projectile;
        ProjectileManager projectile = obj.GetComponent<ProjectileManager>();
        

        if (currentAbility.isRanged)
        {
            //StartCoroutine(projectile.LerpProjectile(GameManager.players[Client.instance.myId].Target.transform, 3f));
        }
    }

    public void StartIntroduction(float _time)
    {
        StartCoroutine(Introduction(_time));
    }



    public void Init()
    {
        InitializeAbilityCast();
        try
        {
            if (roleType == RoleType.Champion)
            {
                anim = GameObject.Find("Armature_0(Clone)").GetComponent<Animator>();
                
            }
            else if (roleType == RoleType.Mage)
            {
                GameObject.Find("Armature_0(Clone)").SetActive(false);
                GameObject.Find("Armature_3(Clone)").SetActive(true);
                anim = GameObject.Find("Armature_3(Clone)").GetComponent<Animator>();
            }
            //anim = GetComponentInChildren<Animator>();
            if (anim == null) Debug.Log("Anim is null");
            isBusy = false;
        }
        catch 
        {
            Debug.LogWarning("Anim is null in role class when trying to get amature");
        }
    }


    private delegate void AbilityHandler(int idx);
    private static Dictionary<int, AbilityHandler> abilityHandlers;

    public void CastAbility(int i, int indx)
    {
       // abilityHandlers[i](indx);
    }

    public void InitializeAbilityCast()
    {
        abilityHandlers = new Dictionary<int, AbilityHandler>()
        {
            {(int)Abilities.AutoAttack, AutoAttack},
            


        };
    }

    public void AutoAttack(int idx)
    {
        //PlayAnimation(1, true);
        anim.SetBool("Attacking", true);
        float l = 1.5f;//anim.GetCurrentAnimatorStateInfo(0).length;
        print("AutoAttack");
        //StartCoroutine(wait(l));
    }


   


   


    public void PlayAnimation(AnimationClip _clip)
    {

        Debug.Log(_clip.name);
        //StartCoroutine(WaitDuringAnimation(_clip.length));
        anim.Play(_clip.name);
    }

    private IEnumerator Introduction(float _time)
    {
        isBusy = true;
       yield return new WaitForSeconds(_time);
        isBusy = false;
        
    }






    #region Animations

    public void SetHorizontalRun(float _h)
    {
        anim.SetFloat("Horizontal", _h);
    }

    public void ArmOrDisarm(bool b)
    {
        anim.SetBool("Armed", b);
    }

    public void Interrupt()
    {
        if (anim.GetBool("Attacking"))
        {
            anim.Play("Interrupt");
        }
    }




    #endregion Animations


    

    

    

   


}

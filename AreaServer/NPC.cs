using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum NPCMode
{
    
    still,
    roam,
    search,
    chase
}

public class NPC : Interactable
{

    public static Dictionary<int, NPC> NPCs = new Dictionary<int, NPC>();
    private static int nextEnemyId = 1;
    string NPC_Name;
    public int AI_ID;
    public int AI_Visuals;
    public NPCMode npcMode;

    public Transform startMarker;
    public Transform endMarker;

    private float startTime;
    public float speed = 2.0F;
    private float journeyLength;
    

    [Header("NPC Attributes")]
    public int walkRadius;
    public float AttackRange;
    public float SightRange;
    public bool isAttacking;
    public float StopChaseRange;
    public Queue<Transform> Targets;

    NavMeshAgent nav;
    SphereCollider AreaOfDetection;


    public List<int> players;
    private void OnEnable()
    {
        AI_ID = nextEnemyId;
        nextEnemyId++;
        NPCs.Add(AI_ID, this);
        nav = this.GetComponent<NavMeshAgent>();
        Targets = new Queue<Transform>();
        //RandomLocation();
        health = maxHealth;
        AreaOfDetection = GetComponent<SphereCollider>();
        AreaOfDetection.radius = SightRange;
        AreaOfDetection.isTrigger = true;
        this.isAlive = true;
        this.nav.isStopped = true;
        players = new List<int>();
        

    }
    
    private void OnTriggerEnter(Collider col)
    {
        /*
        if(col.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            npcMode = NPCMode.chase;
            Targets.Enqueue(col.transform);
            if (Targets.Count == 1) nav.destination = Targets.Peek().position;
            Debug.Log("Collided with Player");
            nav.isStopped = false;
        }
        */
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            

        }
    }

    public void StateChanged()
    {
        switch ((int)npcMode)
        {
            case (int)NPCMode.still:
                ServerSend.SetNPCAnimation(AI_ID, 0);
                break;
            case (int)NPCMode.roam:
                ServerSend.SetNPCAnimation(AI_ID, 1);
                break;
            case (int)NPCMode.chase:
                ServerSend.SetNPCAnimation(AI_ID, 1);
                break;
        }
    }

    public void GameStep()
    {
       
        switch ((int)npcMode)
        {
            case (int)NPCMode.still:
                
                nav.isStopped = true;
                break;
            case (int)NPCMode.roam:
                ServerSend.NPCPosition(this.AI_ID, this.transform.position, transform.rotation.eulerAngles.y);
                //ServerSend.N_NPCPosition(this.AI_ID, this.transform.position, transform.rotation.eulerAngles.y);
                Debug.Log(nav.remainingDistance);
                if (nav.remainingDistance == 0)
                {
                    Debug.Log("Stopped");
                    RandomLocation();
                    //SetLocation(NextMarker());
                }
                break;
            case (int)NPCMode.search:

                break;
            case (int)NPCMode.chase:
                nav.isStopped = false;
                ServerSend.NPCPosition(this.AI_ID, this.transform.position, transform.rotation.eulerAngles.y);
                //ServerSend.N_NPCPosition(this.AI_ID, this.transform.position, transform.rotation.eulerAngles.y);
                if (Targets.Count > 0)
                {
                    if (isAttacking)
                    {
                        return;
                    }
                    if(nav.remainingDistance > AttackRange)
                    {
                        ServerSend.SetNPCAnimation(AI_ID, 1);
                    }
                    if (!isAttacking && nav.remainingDistance < AttackRange)
                    {
                        isAttacking = true;
                        nav.isStopped = true;
                        Attack();
                    }
                    else if(nav.remainingDistance > StopChaseRange)
                    {
                        Targets.Dequeue();
                        if (Targets.Count == 0)
                        {
                            npcMode = NPCMode.roam;
                        }
                        
                    }
                    else
                    {
                       
                    }
                    if(Targets.Count > 0) 
                        nav.destination = Targets.Peek().position;
                    else
                        npcMode = NPCMode.roam;
                }
                else
                {
                    npcMode = NPCMode.still;
                }
                
                break;
        }
    }

    public void RandomLocation()
    {
        Vector3 randomDirection = Random.insideUnitSphere * walkRadius;
        randomDirection += transform.position;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, walkRadius, 1);
        Vector3 finalPosition = hit.position;
        nav.destination = finalPosition;
        ServerSend.SetNPCDestination(AI_ID, finalPosition);
    }

    public override void TakeDamage(int _dmg)
    {
        health -= _dmg;
        if(health <= 0)
        {
            health = 0;
            //die
        }
        ServerSend.UpdateNPCHealth(this.AI_ID, health);
    }

    private void Attack()
    {
        //Play Animation
        Debug.Log("Attack");
        ServerSend.SetNPCAnimation(AI_ID, 2);
        StartCoroutine(Wait(1.5f));
    }

    private void DealDamage(Player _player)
    {
        _player.TakeDamage(10);
        Debug.Log("Dealt 10 damage");
    }

    IEnumerator Wait(float _time)
    {
        
        if (Targets.Count > 0) {
           
            DealDamage(Targets.Peek().gameObject.GetComponent<Player>());
         }
        yield return new WaitForSeconds(_time);
        ServerSend.SetNPCAnimation(AI_ID, 0);
        isAttacking = false;
    }

    private void LoadNPCPreset()
    {
        /////Load Stats 
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Dictionary<int, Projectile> projectiles = new Dictionary<int, Projectile>();
    private static int nextProjectileId = 1;

    public int id;
    public Rigidbody rb;
    public int thrownByPlayer;
    public Vector3 initialForce;
    public float explosionRadius = 1.5f;
    public float explosionDamage = 75f;
    public Transform target;
    private void Start()
    {
       // id = nextProjectileId;
        //nextProjectileId++;
        //projectiles.Add(id, this);

       // ServerSend.SpawnProjectile(this, thrownByPlayer);

        
       // StartCoroutine(ExplodeAfterTime());
       
    }

    private void FixedUpdate()
    {
        ServerSend.ProjectilePosition(this);
    }
    private void OnCollisionEnter(Collision collision)
    {
        //Explode();
    }

   public IEnumerator LerpProjectile(Transform _target, float time)
    {
        float elapsedTime = 0;
        Vector3 startingPos = transform.position;
        Vector3 TargetPos = _target.position;
        while (elapsedTime < time)
        {
            transform.position = Vector3.Lerp(startingPos, TargetPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            TargetPos = _target.position;
            yield return null;
            //Explode(transform.position);
            //destoy
        }

        Explode();
        
    }

    public void Initialize(Vector3 _initialMovementDirection, float _initialForceStrength, int _thrownByPlayer)
    {
        initialForce = _initialMovementDirection * _initialForceStrength;
        thrownByPlayer = _thrownByPlayer;
        rb.AddForce(initialForce);
        StartCoroutine(LerpProjectile(Server.clients[thrownByPlayer].player.Target.transform, 5f));
    }
    public void Initialize(int _thrownByPlayer)
    {
        id = nextProjectileId;
        nextProjectileId++;
        projectiles.Add(id, this);
        thrownByPlayer = _thrownByPlayer;
        target = Server.clients[thrownByPlayer].player.Target.transform;

        ServerSend.SpawnProjectile(this, _thrownByPlayer);
        StartCoroutine(LerpProjectile(target, 5f));
    }

    private void Explode()
    {
        
        ServerSend.ProjectileExploded(this, 10);
        /*
        Collider[] _colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider _collider in _colliders)
        {
            if (_collider.CompareTag("Player"))
            {
                _collider.GetComponent<Player>().TakeDamage(explosionDamage);
            }
        }
        */
        projectiles.Remove(id);
        Destroy(gameObject);
    }

    private IEnumerator ExplodeAfterTime()
    {
        yield return new WaitForSeconds(10f);

        Explode();
    }
    

}

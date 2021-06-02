using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedProjectile : MonoBehaviour
{

    Transform target;
    Vector3 startPos;
    private float distance;
    private float speed;
    private float startTime;



    void Init(Transform _target, float _speed)
    {
        target = _target;
        startTime = Time.time;
        speed = _speed;
        distance =  Vector3.Distance(transform.position, target.position);
        startPos = transform.position;

    }

    private void FixedUpdate()
    {
        float distCovered = (Time.time - startTime) * speed;
        float fracOfJourney = distCovered / distance;

        transform.position = Vector3.Lerp(startPos, target.position, fracOfJourney);
        //ServerSend.
    }


}

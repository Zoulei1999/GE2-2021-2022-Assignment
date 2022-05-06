using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetPursue : SteeringBehaviour
{
    public Boid leader;
    private Vector3 targetPos;
    private Vector3 worldTarget;
    private Vector3 offset;

    private void Start()
    {
        offset = transform.position - leader.transform.position;
        offset = Quaternion.Inverse(leader.transform.rotation) * offset;
    }

    public override Vector3 Calculate()
    {
        worldTarget = leader.transform.TransformPoint(offset);
        float dist = Vector3.Distance(transform.position, worldTarget);
        float time = dist / boid.maxSpeed;

        targetPos = worldTarget + (leader.velocity * time);
        
        return boid.ArriveForce(targetPos);
    }
}

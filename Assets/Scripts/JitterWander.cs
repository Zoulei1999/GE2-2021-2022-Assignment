using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class JitterWander : SteeringBehaviour
{
    public float distance = 20;
    public float radius = 10;
    public float jitter = 100;

    public Vector3 target;
    public Vector3 worldTarget;

    public void OnDrawGizmos()
    {
        if (isActiveAndEnabled && Application.isPlaying)
        {
            Vector3 localCP = Vector3.forward * distance;
            Vector3 worldCP = transform.TransformPoint(localCP);
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(worldCP, radius);
            Gizmos.DrawSphere(worldTarget, 0.1f);
            Gizmos.DrawLine(transform.position, worldTarget);
        }
    }

    public override Vector3 Calculate()
    {
        Vector3 displacement = jitter * Random.insideUnitSphere * Time.deltaTime;
        target += displacement;

        target = Vector3.ClampMagnitude(target, radius);

        Vector3 localTarget = (Vector3.forward * distance) + target;

        worldTarget = transform.TransformPoint(localTarget);

        return worldTarget - transform.position;
    }

    private void Start()
    {
        target = Random.insideUnitSphere * radius;
    }
}

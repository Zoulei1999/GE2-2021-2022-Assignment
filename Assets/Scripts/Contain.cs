using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Contain : SteeringBehaviour
{
    public Transform center;
    public float radius = 50f;

    public void OnDrawGizmos()
    {
        if (Application.isPlaying && isActiveAndEnabled)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(center.position, radius);
        }
    }
    
    public override Vector3 Calculate()
    {
        Vector3 centerOffset = center.transform.position - transform.position;
        float t = centerOffset.magnitude / radius;
        
        if (t < 0.9f)
        {
            return Vector3.zero;
        }
        return centerOffset * t * t;
    }
}

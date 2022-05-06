using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : SteeringBehaviour
{
    public Path path;

    private Vector3 _nextWaypoint;
    public float detectionRange = 3f;

    public override Vector3 Calculate()
    {
        _nextWaypoint = path.NextWaypoint();
        if (Vector3.Distance(transform.position, _nextWaypoint) < detectionRange)
        {
            path.AdvanceToNext();
        }

        if (!path.looped && path.IsLast())
        {
            return boid.ArriveForce(_nextWaypoint, 20);
        }
        else
        {
            return boid.SeekForce(_nextWaypoint);
        }
    }
}

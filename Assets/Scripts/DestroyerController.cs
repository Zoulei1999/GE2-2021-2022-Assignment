using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Entry : State
{
    public override void Enter()
    {
        owner.GetComponent<Arrive>().enabled = true;
    }

    public override void Think()
    {
        if (Vector3.Distance(owner.transform.position,
            owner.GetComponent<Arrive>().targetGameObject.transform.position) < 100)
        {
            owner.GetComponent<Boid>().acceleration = Vector3.zero;
            owner.GetComponent<Boid>().velocity = Vector3.zero;
            owner.GetComponent<Boid>().force = Vector3.zero;
            owner.GetComponent<Arrive>().enabled = false;
        }
    }
}

public class DestroyerController : MonoBehaviour
{
    void Start()
    {
        GetComponent<StateMachine>().ChangeState(new Entry());
    }
}

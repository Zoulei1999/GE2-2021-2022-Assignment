using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

class Patrol : State
{
    public override void Enter()
    {
        owner.GetComponent<Pursue>().enabled = false;
        owner.GetComponent<Contain>().enabled = false;
        owner.GetComponent<FollowPath>().enabled = true;
    }
    
    public override void Think()
    {
        if (Vector3.Distance(owner.GetComponent<Leader>().Star.transform.position,
            owner.transform.position) < 500)
        {
            owner.ChangeState(new LeaderFindTargetState());
        }
    }

    public override void Exit()
    {
        owner.GetComponent<FollowPath>().enabled = false;
    }
}

class LeaderFindTargetState : State
{
    public override void Enter()
    {
        owner.GetComponent<JitterWander>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(owner.GetComponent<Leader>().targetTag);
        owner.GetComponent<Leader>().enemy = enemies[Random.Range(0, enemies.Length)];
    }

    public override void Think()
    {
        if (Vector3.Distance(owner.GetComponent<Leader>().enemy.transform.position,
            owner.transform.position) < 500)
        {
            owner.ChangeState(new LeaderAttackState());
        }
    }

    public override void Exit()
    {
        owner.GetComponent<JitterWander>().enabled = false;
    }
}

class LeaderAttackState : State
{
    AudioSource fireSound;
    
    public override void Enter()
    {
        owner.GetComponent<Pursue>().target = owner.GetComponent<Leader>().enemy.GetComponent<Boid>();
        owner.GetComponent<Pursue>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
    }

    public override void Think()
    {
        Vector3 toEnemy = owner.GetComponent<Leader>().enemy.transform.position - owner.transform.position;
        if (Vector3.Angle(owner.transform.forward, toEnemy) < 45 && toEnemy.magnitude < 300)
        {
            fireSound = owner.GetComponents<AudioSource>()[0];
            GameObject bullet = GameObject.Instantiate(owner.GetComponent<Leader>().bulletPrefab, owner.transform.position + owner.transform.forward * 2, owner.transform.rotation);
            fireSound.volume = 1f;
            fireSound.Play(0);
        }

        if (toEnemy.magnitude < 50)
        {
            owner.ChangeState(new LeaderFleeState());
        }
    }

    public override void Exit()
    {
        owner.GetComponent<Pursue>().enabled = false;
    }
}

class LeaderFleeState : State
{
    public override void Enter()
    {
        owner.GetComponent<Boid>().maxSpeed += 10;
        owner.GetComponent<Boid>().maxForce += 5;
        owner.GetComponent<Flee>().target = owner.GetComponent<Leader>().enemy.transform.position;
        owner.GetComponent<Flee>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
    }

    public override void Think()
    {
        Vector3 toEnemy = owner.GetComponent<Leader>().enemy.transform.position - owner.transform.position;
        if (toEnemy.magnitude > 200)
        {
            owner.ChangeState(new LeaderAttackState());
        }
    }

    public override void Exit()
    {
        owner.GetComponent<Boid>().maxSpeed -= 10;
        owner.GetComponent<Boid>().maxForce -= 5;
        owner.GetComponent<Flee>().enabled = false;
    }
}

class LeaderAlive : State
{
    public override void Think()
    {
        if (owner.GetComponent<Leader>().health <= 0)
        {
            LeaderDead dead = new LeaderDead();
            owner.ChangeState(dead);
            owner.SetGlobalState(dead);
            return;
        }
    }
}

class LeaderDead : State
{
    public override void Enter()
    {
        SteeringBehaviour[] sbs = owner.GetComponent<Boid>().GetComponents<SteeringBehaviour>();
        foreach (SteeringBehaviour sb in sbs)
        {
            sb.enabled = false;
        }

        owner.GetComponent<StateMachine>().enabled = false;
    }
}

public class Leader : MonoBehaviour
{
    public int health = 20;
    public GameObject bulletPrefab;
    public GameObject enemy;
    public Transform Star;
    public String targetTag;

    public AudioSource[] aSources;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            if (GetComponent<Leader>().health > 0)
            {
                health--;
            }

            aSources[1].volume = 1f;
            aSources[1].Play(0);
            Destroy(other.gameObject);
            if (GetComponent<StateMachine>().currentState.GetType() != typeof(Dead))
            {
                GetComponent<StateMachine>().ChangeState(new FleeState());
            }
        }
    }
    
    void Start()
    {
        GetComponent<StateMachine>().ChangeState(new Patrol());
        GetComponent<StateMachine>().SetGlobalState(new LeaderAlive());
    }
}

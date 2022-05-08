using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Random = UnityEngine.Random;

class FindTargetState : State{

    public override void Enter(){

        owner.GetComponent<JitterWander>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
        
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(owner.GetComponent<Starfighter>().targetTag);
        if (enemies == null){

            owner.ChangeState(new ReformationState());
        
        }
        owner.GetComponent<Starfighter>().enemy = enemies[Random.Range(0, enemies.Length)];
        
    }

    public override void Think(){

        if (Vector3.Distance(owner.GetComponent<Starfighter>().enemy.transform.position,owner.transform.position) < 500){
        
            owner.ChangeState(new AttackState());
        
        }
    }

    public override void Exit(){

        owner.GetComponent<JitterWander>().enabled = false;
    
    }
}

class FollowLeaderState : State{

    public override void Enter(){

        owner.GetComponent<Pursue>().enabled = false;
        owner.GetComponent<Contain>().enabled = false;
        owner.GetComponent<OffsetPursue>().leader = owner.GetComponent<Starfighter>().leader;
        owner.GetComponent<OffsetPursue>().enabled = true;
    
    }
    
    public override void Think(){

        if (Vector3.Distance(owner.GetComponent<Starfighter>().star.transform.position,owner.transform.position) < 500){
            
            owner.ChangeState(new FindTargetState());

        }

        if (Vector3.Distance(owner.GetComponent<Starfighter>().leader.transform.position, owner.transform.position) > 400){

            owner.ChangeState(new ReturnToBattleState());
        
        }
    }

    public override void Exit(){

        owner.GetComponent<OffsetPursue>().enabled = false;
    
    }
}

class ReturnToBattleState : State{

    public override void Enter(){
    
        owner.GetComponent<Seek>().targetGameObject = owner.GetComponent<Starfighter>().star.gameObject;
        owner.GetComponent<Seek>().enabled = true;
        owner.GetComponent<Boid>().maxSpeed += 30;
        owner.GetComponent<Boid>().maxForce += 30;
    
    }

    public override void Think(){

        if (Vector3.Distance(owner.GetComponent<Starfighter>().star.transform.position,owner.transform.position) < 200){

            owner.ChangeState(new FindTargetState());
        
        }
    }

    public override void Exit(){

        owner.GetComponent<Seek>().enabled = false;
        owner.GetComponent<Boid>().maxSpeed -= 30;
        owner.GetComponent<Boid>().maxForce -= 30;
    
    }
}

class FleeState : State{

    public override void Enter(){
    
        owner.GetComponent<Boid>().maxSpeed += 10;
        owner.GetComponent<Boid>().maxForce += 5;
        owner.GetComponent<Flee>().target = owner.GetComponent<Starfighter>().enemy.transform.position;
        owner.GetComponent<Flee>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
    
    }

    public override void Think(){

        Vector3 toEnemy = owner.GetComponent<Starfighter>().enemy.transform.position - owner.transform.position;
        if (toEnemy.magnitude > 200){

            owner.ChangeState(new AttackState());
        }
    }

    public override void Exit(){

        owner.GetComponent<Boid>().maxSpeed -= 10;
        owner.GetComponent<Boid>().maxForce -= 5;
        owner.GetComponent<Flee>().enabled = false;
    
    }
}

class AttackState : State{
    AudioSource fireSound;
    public override void Enter(){

        owner.GetComponent<Pursue>().target = owner.GetComponent<Starfighter>().enemy.GetComponent<Boid>();
        owner.GetComponent<Pursue>().enabled = true;
        owner.GetComponent<Contain>().center = GameObject.Find("Death Star Position").transform;
        owner.GetComponent<Contain>().enabled = true;
    
    }

    public override void Think(){

        Vector3 toEnemy = owner.GetComponent<Starfighter>().enemy.transform.position - owner.transform.position;
        if (Vector3.Angle(owner.transform.forward, toEnemy) < 45 && toEnemy.magnitude < 300){
            
            fireSound = owner.GetComponents<AudioSource>()[0];
            GameObject bullet = GameObject.Instantiate(owner.GetComponent<Starfighter>().bulletPrefab, owner.transform.position + owner.transform.forward * 2, owner.transform.rotation);
            fireSound.volume = 1f;
            fireSound.Play(0);

        }

        if (toEnemy.magnitude < 50){

            owner.ChangeState(new FleeState());

        }
    }

    public override void Exit(){

        owner.GetComponent<Pursue>().enabled = false;

    }
}

class FlyFormationState : State{

    public override void Enter(){

        owner.GetComponent<OffsetPursue>().leader = owner.GetComponent<Starfighter>().leader;
        owner.GetComponent<OffsetPursue>().enabled = true;
    
    }
}

class ReformationState : State{

    public override void Enter(){

        owner.GetComponent<Pursue>().target = owner.GetComponent<Starfighter>().leader;
        owner.GetComponent<Pursue>().enabled = true;
    
    }

    public override void Think(){

        if (Vector3.Distance(owner.GetComponent<Starfighter>().leader.transform.position,owner.transform.position) < 50){

            owner.ChangeState(new FlyFormationState());
        
        }
    }

    public override void Exit(){

        owner.GetComponent<Pursue>().enabled = false;
    
    }
}

class Alive : State{

    public override void Think(){

        if (owner.GetComponent<Starfighter>().health <= 0){

            Dead dead = new Dead();
            owner.ChangeState(dead);
            owner.SetGlobalState(dead);
            return;
        }
    }
}

class Dead : State{

    public override void Enter(){

        SteeringBehaviour[] sbs = owner.GetComponent<Boid>().GetComponents<SteeringBehaviour>();
        foreach (SteeringBehaviour sb in sbs){

            sb.enabled = false;
        }

        owner.GetComponent<StateMachine>().enabled = false;
    }
}

public class Starfighter : MonoBehaviour{

    public int health = 10;
    public GameObject bulletPrefab;
    public GameObject enemy;
    public Transform star;
    public String targetTag;
    public Boid leader;
    
    public AudioSource[] aSources;

    public void OnTriggerEnter(Collider other){

        if (other.tag == "Bullet"){

            if (GetComponent<Starfighter>().health > 0){

                health--;
            }

            aSources[1].volume = 1f;
            aSources[1].Play(0);
            Destroy(other.gameObject);
            if (GetComponent<StateMachine>().currentState.GetType() != typeof(Dead)){

                GetComponent<StateMachine>().ChangeState(new FleeState());
            }
        }
    }

    void Start(){

        if (targetTag == "Rebel"){

            leader = GameObject.Find("Sith Lead").GetComponent<Boid>();

        } else if (targetTag == "Sith"){
            
            leader = GameObject.Find("Rebel Lead").GetComponent<Boid>();
        }

        star = GameObject.Find("Death Star Position").transform;
        GetComponent<StateMachine>().ChangeState(new FollowLeaderState());
        GetComponent<StateMachine>().SetGlobalState(new Alive());
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieSpawner : MonoBehaviour{

    public int fighterCount = 20;
    public GameObject prefab;
    public Transform spawnPoint;
    public float spawnInterval = 1f;

    IEnumerator SpawnFighter(){

        yield return new WaitForSeconds(spawnInterval);
        while (true){

            GameObject[] fighters = GameObject.FindGameObjectsWithTag("Sith");
            if (fighters.Length < fighterCount){

                GameObject tie = GameObject.Instantiate(prefab, spawnPoint);
                tie.transform.localScale = new Vector3(50, 50, 50);
            
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Start(){

        StartCoroutine(SpawnFighter());
        
    }
}

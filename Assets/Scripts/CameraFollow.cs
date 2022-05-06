using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraFollow : MonoBehaviour
{
    public List<Transform> positions = new List<Transform>();
    public List<Transform> targets = new List<Transform>();

    public float interval = 5f;
    public float smoothSpeed = 0.5f;

    public int next = 0;

    private void OnEnable()
    {        
        StartCoroutine(Change());
    }
    
    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, positions[next].position, smoothSpeed);
        transform.LookAt(targets[next]);
    }

    IEnumerator Change()
    {
        yield return new WaitForSeconds(interval);
        while (true)
        {
            next = (next + 1) % positions.Count;
            yield return new WaitForSeconds(interval);
        }
    }
}

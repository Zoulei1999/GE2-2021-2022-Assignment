using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float destroyDelay = 2f;
    public float velocity = 30f;

    private void Start()
    {
        Destroy(this.gameObject, destroyDelay);
    }

    private void Update()
    {
        transform.Translate(0, 0, velocity * Time.deltaTime);
    }
}

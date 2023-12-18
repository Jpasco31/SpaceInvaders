using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    public Vector3 direction = Vector3.up;
    public float baseSpeed = 5f;
    public System.Action destroyed;
    
    public void Update()
    {
        transform.position += baseSpeed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(gameObject);
        if (this.destroyed != null)
        {
            this.destroyed.Invoke();
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Destroy(gameObject);
        if (this.destroyed != null)
        {
            this.destroyed.Invoke();
        }

    }
    
}
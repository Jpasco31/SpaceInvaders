using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Vector3 direction;
    public float baseSpeed;
    public System.Action destroyed;

    private void Update()
    {
        this.transform.position += this.direction * (this.baseSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (this.destroyed != null)
        {
            this.destroyed.Invoke();
        }
        
        Destroy(this.gameObject);
    }
}

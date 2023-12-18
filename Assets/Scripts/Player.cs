using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float baseSpeed = 5.5f;
    public Projectile bulletPrefab;
    private bool _bulletActive;

    private void Update()
    {
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        
        float padding = 1f; // Adjust this value for the desired padding

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            this.transform.position += Vector3.left * (this.baseSpeed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            this.transform.position += Vector3.right * (this.baseSpeed * Time.deltaTime);
        }
        
        // Clamp the position within the screen boundaries with padding
        float clampedX = Mathf.Clamp(transform.position.x, leftEdge.x + padding, rightEdge.x - padding);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (!_bulletActive)
        {
            Projectile projectile = Instantiate(this.bulletPrefab, this.transform.position, Quaternion.identity);
            projectile.destroyed += BulletDestroyed;
            _bulletActive = true;
        }
    }

    private void BulletDestroyed()
    {
        _bulletActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader") ||
            other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); //change this when adding scoring
        }
    }
}

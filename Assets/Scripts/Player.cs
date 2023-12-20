using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public float speed = 6f;
    public Projectile laserPrefab;
    Projectile laser;
    public bool _powerUpRapidShot;
    private float _rapidShotDuration = 3f;
    private bool _laserActive;

    private void Update()
    {
        // Check if the player is alive before handling input
        if (GameManager.Instance.gameStarted)
        {
            Shoot();
        }

        if (GameManager.Instance.gameStarted == false)
        {
            _powerUpRapidShot = false;
        }
        
        HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        Vector3 position = transform.position;

        // Update the position of the player based on the input
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            position.x -= speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            position.x += speed * Time.deltaTime;
        }

        // Clamp the position of the character so they do not go out of bounds
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        position.x = Mathf.Clamp(position.x, leftEdge.x, rightEdge.x);

        // Set the new position
        transform.position = position;
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            FireLaser();
        }

    }

    private void FireLaser()
    {
        if (!_laserActive && !_powerUpRapidShot)
        {
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            laser.destroyed += LaserDestroyed;
            _laserActive = true;
        }
        else if (_powerUpRapidShot)
        {
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
        }
    }

    
    private void LaserDestroyed()
    {
        _laserActive = false;
    }
     
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile"))
        {
            GameManager.Instance.OnPlayerKilled(this);
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.OnBoundaryReached();
        }
        
        if (other.gameObject.layer == LayerMask.NameToLayer("PowerUpRapidShot"))
        {
            _powerUpRapidShot = true;
            StartCoroutine(RapidShotTimer());
        }
    }
    
    private IEnumerator RapidShotTimer()
    {
        yield return new WaitForSeconds(_rapidShotDuration);
        _powerUpRapidShot = false;
    }
}
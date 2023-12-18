using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MysteryShip : Invader
{
    public float baseSpeed = 3f;
    public float cycleTime = 30f;
    // public int score = 300;

    private Vector2 leftDestination;
    private Vector2 rightDestination;
    private int direction = -1;
    private Vector3 _direction = Vector2.right;
    public Projectile missilePrefab;
    public float missileAttackRate = 0.7f;

    private void Start()
    {
        // Transform the viewport to world coordinates so we can set the mystery
        // ship's destination points
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // Offset each destination by 1 unit so the ship is fully out of sight
        leftDestination = new Vector2(leftEdge.x - 1f, transform.position.y);
        rightDestination = new Vector2(rightEdge.x + 1f, transform.position.y);

        // Check if the GameObject is active before starting the repeating invoke
        if (gameObject.activeSelf)
        {
            InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
        } else
        {
            OnDisable();
        }

        
    }

    private void Update()
    {
        // Move the object
        this.transform.position += _direction * (this.baseSpeed * Time.deltaTime);

        // Get the left and right edges of the screen
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // Check if the object has reached the right boundary
        if (_direction == Vector3.right && this.transform.position.x >= (rightEdge.x - 1.0f))
        {
            // Change direction to left
            _direction = Vector3.left;
        }
        // Check if the object has reached the left boundary
        else if (_direction == Vector3.left && this.transform.position.x <= (leftEdge.x + 1.0f))
        {
            // Change direction to right
            _direction = Vector3.right;
        }
    }
    
    private void OnDisable()
    {
        // Cancel the scheduled invocations when the GameObject becomes inactive
        CancelInvoke(nameof(MissileAttack));
    }
    
    private void MissileAttack()
    {
        if (Random.value < missileAttackRate)
        {
            Instantiate(this.missilePrefab, this.transform.position, Quaternion.identity);
        }
    }
}
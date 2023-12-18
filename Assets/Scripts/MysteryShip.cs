using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class MysteryShip : Invader
{
    public float baseSpeed = 3f;
    public override int score
    {
        get { return 100; }
        set { /* Optionally implement a setter if needed */ }
    }

    private Vector2 leftDestination;
    private Vector2 rightDestination;
    private Vector3 _direction = Vector2.right;
    public Projectile missilePrefab;
    public float missileAttackRate = 0.7f;
    
    private int hitCount = 0;
    private int maxHit = 2;
    private bool hasAnimated = false;

    private bool spawned;


    private new void Start()
    {
        // Transform the viewport to world coordinates so we can set the mystery
        // ship's destination points
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // Offset each destination by 1 unit so the ship is fully out of sight
        leftDestination = new Vector2(leftEdge.x - 1f, transform.position.y);
        rightDestination = new Vector2(rightEdge.x + 1f, transform.position.y);
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
        
        if (hitCount == 1 && !hasAnimated)
        {
            base.AnimateSprite();
            hasAnimated = true;
        }
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(MissileAttack), this.missileAttackRate, this.missileAttackRate);
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            // Increment the hit count
            hitCount++;

            // Check if the required number of hits is reached
            if (hitCount >= maxHit)
            {
                gameObject.SetActive(false);
                GameManager.Instance.OnMysteryShipKilled(this);
            }
        }
    }
}
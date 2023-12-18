using UnityEngine;
using Random = UnityEngine.Random;

public class Invaders : MonoBehaviour
{
    [Header("Invaders")]
    public Invader[] prefabs = new Invader[5];
    public AnimationCurve baseSpeed = new AnimationCurve();

    private Vector3 direction = Vector3.right;
    private Vector3 initialPosition;

    [Header("Grid")]
    public int rows = 5;
    public int columns = 11;

    [Header("Missiles")]
    public Projectile missilePrefab;
    public float missileSpawnRate = 1f;
    
    private bool gameStarted = false;

    public Projectile powerUpRapidShot;
    private void Awake()
    {
        initialPosition = transform.position;
    }
    
    
    public void StartInvaders()
    {
        if (gameStarted)
            return;

        gameStarted = true;
        CreateInvaderGrid();
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }


    private void CreateInvaderGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            float width = 2f * (columns - 1);
            float height = 2f * (rows - 1);

            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * i) + centerOffset.y, 0f);

            for (int j = 0; j < columns; j++)
            {
                // Create a new invader and parent it to this transform
                Invader invader = Instantiate(prefabs[i], transform);

                // Calculate and set the position of the invader in the row
                Vector3 position = rowPosition;
                position.x += 2f * j;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), missileSpawnRate, missileSpawnRate);
    }
    
    
    private void MissileAttack()
    {
        int amountAlive = GetAliveCount();

        // No missiles should spawn when no invaders are alive
        if (amountAlive == 0) {
            return;
        }

        foreach (Transform invader in transform)
        {
            // Any invaders that are killed cannot shoot missiles
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }

            // Random chance to spawn a missile based upon how many invaders are
            // alive (the more invaders alive the lower the chance)
            if (Random.value < (1f / amountAlive))
            {
                Instantiate(missilePrefab, invader.position, Quaternion.identity);
                break;
            }
        }
    }
    
    private void Update()
    {
        // Calculate the percentage of invaders killed
        int totalCount = rows * columns;
        int amountAlive = GetAliveCount();
        int amountKilled = totalCount - amountAlive;
        float percentKilled = (float)amountKilled / (float)totalCount;


        // Evaluate the speed of the invaders based on how many have been killed
        float baseSpeed = this.baseSpeed.Evaluate(percentKilled);
        transform.position += baseSpeed * Time.deltaTime * direction;
        
        // Transform the viewport to world coordinates so we can check when the
        // invaders reach the edge of the screen
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        // The invaders will advance to the next row after reaching the edge of
        // the screen
        // The invaders will advance to the next row after reaching the edge of
        // the screen
        foreach (Transform invader in transform)
        {
            // Skip any invaders that have been killed
            if (!invader.gameObject.activeInHierarchy) {
                continue;
            }

            // Check the left edge or right edge based on the current direction
            if (direction == Vector3.right && invader.position.x >= (rightEdge.x - 1f))
            {
                AdvanceRow();
                break;
            }
            else if (direction == Vector3.left && invader.position.x <= (leftEdge.x + 1f))
            {
                AdvanceRow();
                break;
            }
        }
    }

    private void AdvanceRow()
    {
        // Flip the direction the invaders are moving
        direction = new Vector3(-direction.x, 0f, 0f);

        // Move the entire grid of invaders down a row
        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;
    }
    
    
    public void ResetInvaders()
    {
        direction = Vector3.right; // Make sure it's set to Vector3.right
        transform.position = initialPosition;

        foreach (Transform invader in transform) {
            invader.gameObject.SetActive(true);
        }
    }

    public int GetAliveCount()
    {
        int count = 0;

        foreach (Transform invader in transform)
        {
            if (invader.gameObject.activeSelf) {
                count++;
            }
        }

        return count;
    }
    
    public void SpawnPowerUp()
    {
        // Check if there are any invaders alive
        if (GetAliveCount() > 0)
        {
            // Random chance to spawn a power-up based on the number of invaders alive
            if (Random.value < (20.0f / (float)GetAliveCount()))
            {
                // Spawn the power-up prefab at the position of the Invaders object
                Instantiate(powerUpRapidShot, transform.position, Quaternion.identity);
            }
        }
    }
}
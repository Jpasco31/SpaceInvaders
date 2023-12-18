using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [SerializeField] private GameObject gameStartUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text livesText;
    [SerializeField] private Text finalScoreText;
    
    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private int currentRound;
    public bool gameStarted;
    
    private int score;
    private int lives;

    public int Score => score;
    public int Lives => lives;
    
    private void Awake()
    {
        if (Instance != null) {
            DestroyImmediate(gameObject);
        } else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    // Function to spawn stars in the background
  
    private void Start()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        // Show the game start UI initially
        gameStartUI.SetActive(true);
        gameOverUI.SetActive(false);
        
        // You may want to hide other UI elements here if needed
        finalScoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        gameStarted = false;
        mysteryShip.gameObject.SetActive(false);
    }
    
    // Add a method to start the game
    private void StartGame()
    {
        // Hide the game start UI and show the necessary game UI elements
        gameStartUI.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        gameStarted = true;
        invaders.StartInvaders();
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }

        // Check for Enter key press to start the game
        if (!gameStartUI.activeSelf && Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    private void NewGame()
    {
        gameStartUI.SetActive(false);
        gameOverUI.SetActive(false);
        finalScoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        SetScore(0);
        SetLives(3);
        currentRound = 0;
        NewRound();
    }

    private void NewRound()
    {
        currentRound++;
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);
        Respawn();

        if (currentRound > 1)
        {
            SetScore(score + 200);
        }

        // Spawn the mystery ship only on the second round
        if (currentRound > 1)
        {
            mysteryShip.gameObject.SetActive(true);
        }
        else
        {
            mysteryShip.gameObject.SetActive(false);
        }
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        finalScoreText.text = "SCORE\n" + score.ToString().PadLeft(4, '0');
        livesText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        finalScoreText.gameObject.SetActive(true);
        invaders.gameObject.SetActive(false);
        mysteryShip.gameObject.SetActive(false);
        gameStarted = false;
        
    
        // Deactivate all active missile prefabs in the scene
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject); 
        }
        
    }

    private void SetScore(int score)
    {
        this.score = score;
        scoreText.text = "SCORE\n" + score.ToString().PadLeft(4, '0');
    }

    private void SetLives(int lives)
    {
        this.lives = Mathf.Max(lives, 0);
        livesText.text = "LIVES\n" + this.lives.ToString();
    }
    
    public void OnPlayerKilled(Player player)
    {
        SetLives(lives - 1);
        player._powerUpRapidShot = false;
        player.gameObject.SetActive(false);

        if (lives > 0) {
            Invoke(nameof(Respawn), 1f);
        } else {
            GameOver();
        }
    }

    public void OnInvaderKilled(Invader invader)
    {
        invader.gameObject.SetActive(false);
        

        SetScore(score + invader.score);
        
        invaders.SpawnPowerUp(); // Call the SpawnPowerUp method when an invader is killed

        if (invaders.GetAliveCount() == 0) {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        SetScore(score + mysteryShip.score);
    }

    public void OnBoundaryReached()
    {
        SetLives(0);
        GameOver();
    }
    
    public void RestartGame()
    {
        // Implement any additional restart logic here
        StartGame();
    }
}
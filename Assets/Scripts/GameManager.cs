using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private Player playerPrefab;
    private Invaders invaders;
    [SerializeField] private MysteryShip mysteryShipPrefab;
    private MysteryShip mysteryShip;

    private int currentRound;
    public bool gameStarted;
    
    private int score;
    private int lives;

    public int Score => score;
    public int Lives => lives;
    
    [SerializeField] private AudioSource playerDeadEffect;
    [SerializeField] private AudioSource invaderDeadEffect;
    [SerializeField] private AudioSource mysteryShipDeadEffect;
    [SerializeField] private AudioSource boundaryReachedEffect;
    
    [SerializeField] private AudioSource startEffect;
    [SerializeField] private AudioSource restartEffect;
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
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        mysteryShip = Instantiate(mysteryShipPrefab, Vector3.zero, Quaternion.identity);
        invaders = FindObjectOfType<Invaders>();
        gameStarted = false;
        // Show the game start UI initially
        gameStartUI.SetActive(true);
        gameOverUI.SetActive(false);
        mysteryShip.gameObject.SetActive(false);
        
        
        // You may want to hide other UI elements here if needed
        finalScoreText.gameObject.SetActive(false);
        livesText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
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
        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return) && gameStarted)
        {
            restartEffect.Play();
            Start();
        } else if (!gameStarted && Input.GetKeyDown(KeyCode.Return))
        {
            startEffect.Play();
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
        invaders.ResetInvaders();
        invaders.StartInvaders();
    }

    private void NewRound()
    {
        currentRound++;
        invaders.ResetInvaders();
        
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject); 
        }
        
        Respawn();

        if (currentRound + 1 > 1)
        {
            mysteryShip.ResetMysteryShipPosition();
            SetScore(score + 200);
            mysteryShip.gameObject.SetActive(true);
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
        finalScoreText.gameObject.SetActive(true);
        finalScoreText.text = "SCORE\n" + score.ToString().PadLeft(4, '0');
        livesText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        gameOverUI.SetActive(true);
        Destroy(player.gameObject);
        if (mysteryShip != null)
        {
            Destroy(mysteryShip.gameObject);
        }
    
        // Deactivate all active missile prefabs in the scene
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        foreach (Projectile projectile in projectiles)
        {
            Destroy(projectile.gameObject); 
        }

        invaders.DeactivateAll();
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
        playerDeadEffect.Play();
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
        invaderDeadEffect.Play();
        invader.gameObject.SetActive(false);
        SetScore(score + invader.score);
        
        invaders.SpawnPowerUp(); // Call the SpawnPowerUp method when an invader is killed

        if (invaders.GetAliveCount() == 0) {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        mysteryShipDeadEffect.Play();
        SetScore(score + mysteryShip.score);
        this.mysteryShip.gameObject.SetActive(false);
        mysteryShip.AnimateSprite();
        mysteryShip.hitCount = 0;
    }

    public void OnBoundaryReached()
    {
        boundaryReachedEffect.Play();
        player.gameObject.SetActive(false);
        SetLives(0);
        if (lives == 0)
        {
            GameOver();
        }
    }
}
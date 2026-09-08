using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Needed for the Button control

public class GameController3D : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public Rigidbody playerRb;
    public float moveSpeed = 6f;

    [Header("UI Blackout")]
    public CanvasGroup blindnessCanvasGroup;
    public TextMeshProUGUI timerText;
    public float fadeSpeed = 8f;

    [Header("Game Over UI")]
    public GameObject restartButtonObject; // Drag your UI Button here!

    [Header("Game Loop")]
    public float timeRemaining = 120f; 
    private bool isGameOver = false;

    [Header("Guns & Shooting")]
    public Transform[] gunSpawnPoints; 
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;
    public float fireRate = 1.3f; 
    private float nextFireTime;

    void Start()
    {
        // Make sure the restart button is hidden at the start of the game
        if (restartButtonObject != null)
        {
            restartButtonObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isGameOver) return; // Completely stops everything else if hit

        // Timer countdown
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            DisplayTime(timeRemaining);
        }
        else
        {
            WinGame();
        }

        // Movement input
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector3 moveDirection = new Vector3(moveX, 0f, moveZ).normalized;
        
        playerRb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, playerRb.linearVelocity.y, moveDirection.z * moveSpeed);

        // Movement checking for the black screen
        bool isMoving = moveDirection.magnitude > 0.01f;
        if (isMoving)
        {
            blindnessCanvasGroup.alpha = Mathf.MoveTowards(blindnessCanvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 0f); 
        }
        else
        {
            blindnessCanvasGroup.alpha = Mathf.MoveTowards(blindnessCanvasGroup.alpha, 0f, fadeSpeed * Time.deltaTime);
            timerText.color = new Color(timerText.color.r, timerText.color.g, timerText.color.b, 1f); 
        }

        // Enemy firing mechanics
        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        foreach (Transform spawnPoint in gunSpawnPoints)
        {
            if (spawnPoint == null || player == null) continue;

            GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, Quaternion.identity);
            Vector3 direction = (player.position - spawnPoint.position).normalized;
            direction.y = 0f; 

            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
            if (bulletRb == null)
            {
                bulletRb = bullet.AddComponent<Rigidbody>();
                bulletRb.useGravity = false;
            }
            bulletRb.linearVelocity = direction * bulletSpeed;

            Destroy(bullet, 5f);
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60); 
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void LoseGame()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Force freeze player movement instantly
        playerRb.linearVelocity = Vector3.zero;
        playerRb.isKinematic = true; 

        // Show UI elements above the blackout screen
        blindnessCanvasGroup.alpha = 0f; 
        timerText.color = Color.white;
        timerText.text = "GAME OVER!";

        // Show the restart button
        if (restartButtonObject != null)
        {
            restartButtonObject.SetActive(true);
        }
    }

    void WinGame()
    {
        isGameOver = true;
        playerRb.linearVelocity = Vector3.zero;
        playerRb.isKinematic = true;

        blindnessCanvasGroup.alpha = 0f;
        timerText.color = Color.white;
        timerText.text = "YOU WIN!";

        if (restartButtonObject != null)
        {
            restartButtonObject.SetActive(true);
        }
    }

    // This function will be triggered by clicking our UI Button
    public void RestartGameClick()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text scoreText;
    public Text winText;
    public GameObject winTextBg;
    public GameObject bugIconPrefab;
    public GameObject[] otherIconPrefabs; // Array to hold non-bug icons
    public float iconSpeed = 2f;
    public GameObject pauseMenu;
    public GameObject pause;

    private int score;
    private float timeSinceLastInteraction;
    private bool gameWon;

    

    void Start()
    {
        score = 0;
        UpdateScoreText();
        InvokeRepeating("SpawnIcon", 1f, 1.5f); // Invoke SpawnIcon every 1.5 seconds after 1 second delay
    }

    void Update()
    {
        if (Time.timeScale == 1)
        {
            // Check for touch or click input
            if (Input.GetMouseButtonDown(0))
            {
                timeSinceLastInteraction = 0f; // Reset the timer on user interaction

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

                if (hit.collider != null)
                {
                    if (hit.collider.CompareTag("BugIcon") || hit.collider.CompareTag("OtherIcon"))
                    {
                        HandleIcon(hit.collider.gameObject);
                    }
                }
            }

            // Update the time since the last interaction
            timeSinceLastInteraction += Time.deltaTime;

            // Check for win condition
            if (score == 10 && timeSinceLastInteraction >= 3f && !gameWon)
            {
                gameWon = true;
                ShowWinText();
            }


        }
        if (Input.GetMouseButtonDown(0))
        {
           
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Pause")|| hit.collider.CompareTag("Resume") || hit.collider.CompareTag("Restart"))
                {
                    HandlePauseIcon(hit.collider.gameObject);
                }
            }
        }
    }

    
    void SpawnIcon()
    {
        float randomX = Random.Range(Screen.width * 0.1f, Screen.width * 0.9f); // Spawn within 10% and 90% of the screen width
        randomX = Camera.main.ScreenToWorldPoint(new Vector3(randomX, 0, 0)).x; // Convert screen coordinates to world coordinates

        GameObject iconPrefab;

        if (Random.Range(0, 5) < 2) // 40% chance for bug icon
        {
            iconPrefab = bugIconPrefab;
        }
        else // 60% chance for non-bug icon
        {
            int randomIndex = Random.Range(0, otherIconPrefabs.Length);
            iconPrefab = otherIconPrefabs[randomIndex];
        }

        GameObject icon = Instantiate(iconPrefab, new Vector3(randomX, 6f, 0f), Quaternion.identity);
        Rigidbody2D iconRb = icon.AddComponent<Rigidbody2D>();
        iconRb.gravityScale = 0; // Disable default gravity
        iconRb.velocity = new Vector2(0, -iconSpeed); // Constant downward velocity
    }

    void HandleIcon(GameObject icon)
    {
        if (icon.CompareTag("BugIcon"))
        {
            score++;
        }
        else if (icon.CompareTag("OtherIcon"))
        {
            score--;
        }

        UpdateScoreText();
        Destroy(icon);
        
    }
    void HandlePauseIcon(GameObject icon)
    {
        if (icon.CompareTag("Pause"))
        {
            icon.SetActive(false);
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }else if (icon.CompareTag("Resume")) 
        {
            pauseMenu.SetActive(false);
            pause.SetActive(true);
            Time.timeScale = 1;
        }
        else if(icon.CompareTag("Restart"))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void ShowWinText()
    {
        winTextBg.SetActive(true);
        winText.text = "You Win!";
        Time.timeScale = 0;
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }
}

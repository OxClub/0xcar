using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private bool isPlaying = false;
    private float score = 0f;
    private float distance = 0f;
    private float highScore = 0f;

    public CarController playerCar;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        StartGame();
    }

    void Update()
    {
        if (!isPlaying || playerCar == null) return;

        distance += playerCar.GetSpeed() * Time.deltaTime;
        score = distance;

        if (UIManager.Instance != null)
            UIManager.Instance.UpdateUI(score, playerCar.GetSpeedKmh(), highScore);
    }

    public void StartGame()
    {
        isPlaying = true;
        score = 0f;
        distance = 0f;
    }

    public void GameOver()
    {
        if (!isPlaying) return;
        isPlaying = false;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetFloat("HighScore", highScore);
            PlayerPrefs.Save();
        }

        if (UIManager.Instance != null)
            UIManager.Instance.ShowGameOver(score, highScore);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public bool IsPlaying() => isPlaying;
    public float GetScore() => score;
}

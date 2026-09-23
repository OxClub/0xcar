using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("HUD")]
    public Text scoreText;
    public Text speedText;
    public Text highScoreText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text finalHighScoreText;
    public Button restartButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (restartButton != null)
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    public void UpdateUI(float score, float speed, float highScore)
    {
        if (scoreText != null) scoreText.text = "Score: " + Mathf.FloorToInt(score);
        if (speedText != null) speedText.text = "Speed: " + Mathf.FloorToInt(speed) + " km/h";
        if (highScoreText != null) highScoreText.text = "Best: " + Mathf.FloorToInt(highScore);
    }

    public void ShowGameOver(float score, float highScore)
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "Score: " + Mathf.FloorToInt(score);
        if (finalHighScoreText != null) finalHighScoreText.text = "High Score: " + Mathf.FloorToInt(highScore);
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text finalScoreText;
    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleStateChanged;

        if(restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleStateChanged;

        if(restartButton != null)
        {
            restartButton.onClick.RemoveListener(RestartGame);
        }
    }

    private void Start()
    {
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void HandleStateChanged(GameState newState)
    {
        if(newState == GameState.GameOver)
        {
            ShowGameOver();
        }
        else if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if(finalScoreText != null && ScoreManager.Instance != null)
        {
            finalScoreText.text = $"FINAL SCORE: {ScoreManager.Instance.Score}";
        }
    }
    
    private void RestartGame()
    {
        if(ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        GameManager.Instance.SetState(GameState.Gameplay);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text linesText;

    private void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateScoreUI;
        ScoreManager.OnLinesCountChanged += UpdateLinesUI;
    }

    private void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateScoreUI;
        ScoreManager.OnLinesCountChanged -= UpdateLinesUI;
    }

    private void Start()
    {
        UpdateScoreUI(0);
        UpdateLinesUI(0);
    }

    private void UpdateScoreUI(int newScore)
    {
        if(scoreText != null)
        {
            scoreText.text = $"SCORE:{newScore}";
        }
    }
    private void UpdateLinesUI(int newLines)
    {
        if (linesText != null)
        {
            linesText.text = $"Lines:{newLines}";
        }
    }
}

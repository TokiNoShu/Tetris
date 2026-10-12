using System;
using Unity.Mathematics;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance {  get; private set; }

    [SerializeField] private float baseStepDelay = 0.8f;
    [SerializeField] private float minStepDelay = 0.05f;
    [SerializeField] private float speedFactor = 0.015f;


    public int Score { get; private set; }
    public int TotalLinesCleared {  get; private set; }
    public int ComboCount {  get; private set; }
    public float CurrentStepDelay {  get; private set; }


    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnLinesCountChanged;
    public static event Action<int> OnComboChanged;
    public static event Action<float> OnSpeedChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        CurrentStepDelay = baseStepDelay;
    }
    void OnEnable()
    {
        GridManager.OnLinesCleared += ProcessLinesCleared;
    }

    void OnDisable()
    {
        GridManager.OnLinesCleared -= ProcessLinesCleared;
    }

    private void ProcessLinesCleared(int lines)
    {
        if(lines > 0)
        {
            ComboCount++;

            int basePoints = lines switch
            {
                1 => 100,
                2 => 300,
                3 => 500,
                4 => 800,
                _ => lines * 250
            };
            
            int comboBonus = ComboCount > 0 ? 50 * ComboCount : 0;

            float speedMultiplier = baseStepDelay / CurrentStepDelay;
            int totalGainedPoints = Mathf.RoundToInt((basePoints + comboBonus) * speedMultiplier);

            Score += totalGainedPoints;
            TotalLinesCleared += lines;

            RecalculateSpeed();

            OnScoreChanged?.Invoke(Score);
            OnLinesCountChanged?.Invoke(TotalLinesCleared);
            if(ComboCount > 0) OnComboChanged?.Invoke(ComboCount);
        }
        else
        {
            ComboCount = -1;
            OnComboChanged?.Invoke(0);
        }
    }
    private void RecalculateSpeed()
    {
        CurrentStepDelay = minStepDelay + (baseStepDelay - minStepDelay) * Mathf.Exp(-speedFactor * TotalLinesCleared);

        OnSpeedChanged?.Invoke(CurrentStepDelay);
    }

    public void ResetScore()
    {
        Score = 0;
        TotalLinesCleared = 0;
        ComboCount = -1;
        CurrentStepDelay = baseStepDelay;

        OnScoreChanged?.Invoke(Score);
        OnLinesCountChanged?.Invoke(TotalLinesCleared);
        OnSpeedChanged?.Invoke(CurrentStepDelay);
    }
}

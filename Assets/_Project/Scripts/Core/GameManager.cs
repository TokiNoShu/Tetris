using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    Gameplay,
    Paused,
    GameOver
}
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public static event Action<GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    void Start()
    {
        SetState(GameState.Gameplay);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Gameplay:
                Time.timeScale = 1f;
                break;
            case GameState.Paused:
            case GameState.GameOver:
                Time.timeScale = 0f;
                break;
        }
    }
    public void GameOver()
    {
        Debug.Log("Game Over!");
        SetState(GameState.GameOver);
    }
}

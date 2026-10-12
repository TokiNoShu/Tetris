using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance {  get; private set; }

    public static event Action<Vector2Int> OnMove;
    public static event Action<int> OnRotate;
    public static event Action OnHardDrop;
    public static event Action<bool> OnSoftDrop;
    public static event Action OnHold;

    [Header("Auto-Repeat Settings (DAS)")]
    [SerializeField] private float initialDelay = 0.18f;
    [SerializeField] private float repeatRate = 0.05f;

    private float horizontalTimer;
    private int lastHorizontalDir;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Gameplay)
            return;

        HandleHorizontalInput();
        HandleRotationInput();
        HandleDropInput();
        HandleHoldInput();
    }

    private void HandleHorizontalInput()
    {
        int dir = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) dir = -1;
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) dir = 1;

        if (dir != 0)
        {
            OnMove?.Invoke(new Vector2Int(dir, 0));
            horizontalTimer = -initialDelay;
            lastHorizontalDir = dir;
            return;
        }

        int holdDir = 0;
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) holdDir = -1;
        else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) holdDir = 1;

        if(holdDir != 0 && holdDir == lastHorizontalDir)
        {
            horizontalTimer += Time.deltaTime;
            if(horizontalTimer >= repeatRate)
            {
                horizontalTimer = 0f;
                OnMove?.Invoke(new Vector2Int(holdDir, 0));
            }
        }
        else if(holdDir == 0)
        {
            lastHorizontalDir = 0;
        }
    }

    private void HandleRotationInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.E))
        {
            OnRotate?.Invoke(1);
        }
        else if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            OnRotate?.Invoke(-1);
        }
    }

    private void HandleDropInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnHardDrop?.Invoke();
        }

        if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            OnSoftDrop?.Invoke(true);
        }
        else if(Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
        {
            OnSoftDrop?.Invoke(false);
        }
    }

    private void HandleHoldInput()
    {
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnHold?.Invoke();
        }
    }
}

using UnityEngine;

public class PieceController : MonoBehaviour
{
    [Header("Timings")]
    [SerializeField] private float softDropMultiplier = 10f;
    [SerializeField] private float lockDelay = 0.5f;

    private float baseStepDelay;
    private float currentStepDelay;
    private float stepTimer;
    private float lockTimer;
    private bool isGrounded;
    private bool isSoftDropping;

    private void Awake()
    {
        UpdateBaseStepDelay();
    }

    private void OnEnable()
    {
        InputManager.OnMove += HandleMove;
        InputManager.OnRotate += HandleRotate;
        InputManager.OnHardDrop += HandleHardDrop;
        InputManager.OnSoftDrop += HandleSoftDropToggle;

        ScoreManager.OnSpeedChanged += HandleSpeedChanged;
    }

    private void OnDisable()
    {
        InputManager.OnMove -= HandleMove;
        InputManager.OnRotate -= HandleRotate;
        InputManager.OnHardDrop -= HandleHardDrop;
        InputManager.OnSoftDrop -= HandleSoftDropToggle;

        ScoreManager.OnSpeedChanged -= HandleSpeedChanged;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameState.Gameplay)
            return;

        HandleStepDown();
        HandleLockDelay();
    }

    #region Event Handlers
    public void HandleMove(Vector2Int direction)
    {
        transform.position += (Vector3Int)direction;

        if (!GridManager.Instance.IsValidPosition(transform))
        {
            transform.position -= (Vector3Int)direction;
        }
        else
        {
            ResetLockTimer();
        }
    }

    public void HandleRotate(int direction)
    {
        float angle = direction > 0 ? -90f : 90f;
        transform.Rotate(0, 0, angle);

        if (!GridManager.Instance.IsValidPosition(transform))
        {
            if (TryWallKick()) return;

            transform.Rotate(0, 0, -angle);
        }
        else
        {
            ResetLockTimer();
        }
    }

    public void HandleHardDrop()
    {
        while (GridManager.Instance.IsValidPosition(transform))
        {
            transform.position += Vector3.down;
        }

        transform.position += Vector3.up;
        LockPiece();
    }

    private void HandleSoftDropToggle(bool isPressed)
    {
        isSoftDropping = isPressed;
        currentStepDelay = isSoftDropping ? baseStepDelay / softDropMultiplier : baseStepDelay;
    }

    private void HandleSpeedChanged(float newBaseDelay)
    {
        baseStepDelay = newBaseDelay;
        currentStepDelay = isSoftDropping ? baseStepDelay / softDropMultiplier : baseStepDelay;
    }
    #endregion

    #region Logic

    private void HandleStepDown()
    {
        stepTimer += Time.deltaTime;

        if (stepTimer >= currentStepDelay)
        {
            stepTimer = 0f;
            StepDown();
        }
    }

    private void StepDown()
    {
        transform.position += Vector3.down;

        if (!GridManager.Instance.IsValidPosition(transform))
        {
            transform.position += Vector3.up;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void HandleLockDelay()
    {
        if (!isGrounded)
        {
            lockTimer = 0f;
            return;
        }

        lockTimer += Time.deltaTime;
        if (lockTimer >= lockDelay)
        {
            LockPiece();
        }
    }

    private void ResetLockTimer()
    {
        if (isGrounded)
        {
            lockTimer = 0f;
        }
    }

    private void LockPiece()
    {
        GridManager.Instance.PlacePiece(transform);
        SpawnManager.Instance.ResetHoldLock();

        transform.DetachChildren();
        Destroy(gameObject);

        SpawnManager.Instance.SpawnPiece();
    }

    private bool TryWallKick()
    {
        Vector3[] offsets = {Vector3.left, Vector3.right, Vector3.up};

        foreach(var offset in offsets)
        {
            transform.position += offset;
            if (GridManager.Instance.IsValidPosition(transform))
            {
                ResetLockTimer();
                return true;
            }
            transform.position -= offset;
        }
        return false;
    }

    private void UpdateBaseStepDelay()
    {
        if (ScoreManager.Instance != null)
        {
            baseStepDelay = ScoreManager.Instance.CurrentStepDelay;
        }
        else
        {
            baseStepDelay = 0.8f;
        }
        currentStepDelay = isSoftDropping ? baseStepDelay / softDropMultiplier : baseStepDelay;
    }
    #endregion

}

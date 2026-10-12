using System;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private TetrominoData[] availableTetrominos;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(4, 18, 0);
    [SerializeField] private Vector3 holdPosition = new Vector3(-7, 16, 0);
    [SerializeField] private Vector3 nextPosition = new Vector3(16, 16, 0);

    private TetrominoData currentTetrominoData;
    private TetrominoData nextTetrominoData;
    private TetrominoData heldTetrominoData;

    private bool canHold = true;
    private GameObject activePieceObject;
    private GameObject heldPieceObject;
    private GameObject nextPieceObject;

    public static event Action<TetrominoData> OnNextPieceChanged;
    public static event Action<TetrominoData> OnHoldPieceChanged;

    [Header("Ghost Piece")]
    [SerializeField] private GameObject ghostPrefab;
    private GhostPiece currentGhost;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void OnEnable()
    {
        InputManager.OnHold += HoldPiece;
    }

    private void OnDisable()
    {
        InputManager.OnHold -= HoldPiece;
    }

    void Start()
    {
        PrepareNextTetromino();
        SpawnPiece();
        ShowHoldPiece();
    }

    private void PrepareNextTetromino()
    {
        int randomIndex = UnityEngine.Random.Range(0, availableTetrominos.Length);
        nextTetrominoData = availableTetrominos[randomIndex];
        OnNextPieceChanged?.Invoke(nextTetrominoData);
        ShowNextPiece(nextTetrominoData);
    }

    public GameObject SpawnPiece(TetrominoData dataToSpawn = null)
    {
        if (currentGhost != null)
        {
            currentGhost.DestroyGhost();
        }

        if (dataToSpawn == null)
        {
            currentTetrominoData = nextTetrominoData;
            PrepareNextTetromino();
        }
        else
        {
            currentTetrominoData = dataToSpawn;
        }

        activePieceObject = new GameObject($"Piece_{currentTetrominoData.tetrominoName}");
        activePieceObject.transform.position = spawnPosition;

        foreach(Vector2Int cell in currentTetrominoData.cells)
        {
            GameObject block = Instantiate(blockPrefab, activePieceObject.transform);
            block.transform.localPosition = new(cell.x, cell.y, 0);

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                sr.sprite = currentTetrominoData.blockSprite;
                sr.color = currentTetrominoData.color;
            }
        }
        activePieceObject.AddComponent<PieceController>();

        if (!GridManager.Instance.IsValidPosition(activePieceObject.transform))
        {
            GameManager.Instance.GameOver();
            Destroy(activePieceObject);
            return null;
        }

        if(ghostPrefab != null)
        {
            GameObject ghostObj = Instantiate(ghostPrefab);
            currentGhost = ghostObj.GetComponent<GhostPiece>();

            currentGhost.Setup(activePieceObject.transform, blockPrefab, currentTetrominoData);
        }

        return activePieceObject;
    }

    public void HoldPiece()
    {
        if(!canHold || activePieceObject == null) return;

        canHold = false;
        TetrominoData tempToHold = currentTetrominoData;

        Destroy(activePieceObject);

        if(heldTetrominoData == null)
        {
            heldTetrominoData = tempToHold;
            SpawnPiece();
        }
        else
        {
            TetrominoData previousHold = heldTetrominoData;
            heldTetrominoData = tempToHold;
            SpawnPiece(previousHold);
        }

        OnHoldPieceChanged?.Invoke(heldTetrominoData);


        ShowHoldPiece(heldTetrominoData);
    }

    public void ResetHoldLock()
    {
        canHold = true;
    }

    public void ShowHoldPiece(TetrominoData dataToShow = null)
    {
        if (heldPieceObject != null)
        {
            Destroy(heldPieceObject);
        }
        if (dataToShow == null) return;

        heldPieceObject = new GameObject($"HeldPiece_{heldTetrominoData}");
        heldPieceObject.transform.position = holdPosition;

        foreach (Vector2Int cell in dataToShow.cells)
        {
            GameObject block = Instantiate(blockPrefab, heldPieceObject.transform);
            block.transform.localPosition = new(cell.x, cell.y, 0);

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = heldTetrominoData.blockSprite;
                sr.color = heldTetrominoData.color;
            }
        } 
    }
    public void ShowNextPiece(TetrominoData dataToShow = null)
    {
        if (nextPieceObject != null)
        {
            Destroy(nextPieceObject);
        }
        if (dataToShow == null) return;

        nextPieceObject = new GameObject($"HeldPiece_{nextTetrominoData}");
        nextPieceObject.transform.position = nextPosition;

        foreach (Vector2Int cell in dataToShow.cells)
        {
            GameObject block = Instantiate(blockPrefab, nextPieceObject.transform);
            block.transform.localPosition = new(cell.x, cell.y, 0);

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = nextTetrominoData.blockSprite;
                sr.color = nextTetrominoData.color;
            }
        }
    }
}

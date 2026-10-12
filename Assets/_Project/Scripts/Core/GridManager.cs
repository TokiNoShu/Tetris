using System;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    public static GridManager Instance { get; private set; }

    public int Width { get; private set; } = 10;
    public int Height { get; private set; } = 20;

    private Transform[,] grid;

    public static event Action<int> OnLinesCleared;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        grid = new Transform[Width, Height];
    }

    public bool IsValidPosition(Transform pieceTransform)
    {
        foreach (Transform child in pieceTransform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);

            if (pos.x < 0 || pos.x >= Width || pos.y < 0) return false;
            if (pos.y < Height && grid[pos.x, pos.y] != null) return false;
        }
        return true;
    }

    public void PlacePiece(Transform pieceTransform)
    {
        foreach(Transform child in pieceTransform)
        {
            Vector2Int pos = Vector2Int.RoundToInt(child.position);
            if(pos.y < Height)
            {
                grid[pos.x, pos.y] = child;
            }
        }

        CheckAndClearLines();
    }

    public void CheckAndClearLines()
    {
        int linesCleared = 0;
        for(int y = 0; y < Height; y++)
        {
            if (isRowFull(y))
            {
                DeleteRow(y);
                DecreaseRowAbove(y + 1);
                linesCleared++;
                y--;
            }
        }
        if(linesCleared > 0)
        {
            OnLinesCleared?.Invoke(linesCleared);
        }
    }

    public bool isRowFull(int y)
    {
        for(int x = 0; x < Width; x++)
        {
            if (grid[x, y] == null) return false;
        }
        return true;
    }

    public void DeleteRow(int y)
    {
        for(int x = 0; x < Width; x++)
        {
            if(grid[x, y] != null)
            {
                Destroy(grid[x, y].gameObject);
                grid[x, y] = null;
            }
        }
    }

    private void DecreaseRowAbove(int startY)
    {
        for(int y = startY; y < Height; y++)
        {
            for(int x = 0; x < Width; x++)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y - 1] = grid[x, y];
                    grid[x, y] = null;
                    grid[x, y - 1].position += Vector3.down;
                }
            }
        }
    }
}

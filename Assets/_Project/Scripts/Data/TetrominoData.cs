using UnityEngine;

[CreateAssetMenu(fileName = "NewTetromino", menuName = "Tetris/Tetromino Data", order = 1)]
public class TetrominoData : ScriptableObject
{
    public string tetrominoName;
    public Sprite blockSprite;
    public Color color;
    public TetrominoData previousHold;

    public Vector2Int[] cells;
}

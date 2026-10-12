using System.Runtime.CompilerServices;
using UnityEngine;

public class GhostPiece : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color ghostColor = new Color(1f, 1f, 1f, 0.3f);

    private Transform currentPieceTransform;
    private SpriteRenderer[] ghostRenderers;

    public void Setup(Transform pieceTransform, GameObject blockPrefab, TetrominoData data)
    {
        currentPieceTransform = pieceTransform;

        foreach(Vector2Int cell in data.cells)
        {
            GameObject block = Instantiate(blockPrefab, transform);
            block.transform.localPosition = new Vector3(cell.x, cell.y, 0);

            SpriteRenderer sr = block.GetComponent<SpriteRenderer>();
            if(sr != null)
            {
                sr.sprite = data.blockSprite;
                Color c = data.color;
                c.a = ghostColor.a;
                sr.color = c;

                sr.sortingOrder = 4;
            }
        }
        ghostRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void LateUpdate()
    {
        if(currentPieceTransform == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = currentPieceTransform.position;
        transform.rotation = currentPieceTransform.rotation;

        DropToBottom();
    }

    private void DropToBottom()
    {
        Vector3 currentPos = transform.position;

        while (GridManager.Instance.IsValidPosition(transform))
        {
            currentPos.y -= 1f;
            transform.position = currentPos;
        }

        currentPos.y += 1f;
        transform.position = currentPos;
    }

    public void DestroyGhost()
    {
        Destroy(gameObject);
    }
}

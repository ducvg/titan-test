using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class BlockGroup : MonoBehaviour, 
    IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private Transform previewParent;
    public bool IsPlaced {get; private set;}
    public byte[,] Shape {get; private set;}
    private Board board;
    public List<Block> Blocks {get; private set;} = new();
    private List<Block> previewBlocks = new();
    private Vector2 shapeCenter;
    private Vector2 originPosition;
    private Sprite currentColorSprite;
    public new Transform transform { get; private set; }

    void Awake()
    {
        transform = base.transform;
        originPosition = transform.localPosition;
    }

    public void Init(byte[,] shape, Sprite colorSprite)
    {
        ReleaseAllBlocks();
        board = LevelManager.Instance.Board;
        this.Shape = shape;
        this.currentColorSprite = colorSprite;
        this.IsPlaced = false;

        int rows = shape.GetLength(0);
        int cols = shape.GetLength(1);
        shapeCenter = new Vector2(cols * 0.5f, rows * 0.5f);

        Vector2 cellPos;
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < cols; ++c)
            {
                if (shape[r, c] == 0) continue;

                int flippedRow = rows - 1 - r;
                cellPos = GetShapeBlockPosition(c, flippedRow);
                SpawnBlock(colorSprite, cellPos);
                SpawnPreviewBlock(colorSprite, cellPos);
            }
        }

        transform.localScale = Vector3.one * 0.5f;
    }

    private Vector2 GetShapeBlockPosition(int col, int row)
    {
        float x = (col - shapeCenter.x) * board.CellStep.x + board.CellPivot.x;
        float y = (row - shapeCenter.y) * board.CellStep.y + board.CellPivot.y;

        return new Vector2(x, y);
    }

    private void SpawnBlock(Sprite colorSprite, Vector2 cellPos)
    {
        Block block = BlockFactory.Instance.Get();
        block.transform.SetParent(transform);
        block.transform.localPosition = cellPos;
        block.transform.localScale = Vector3.one;

        block.SetSprite(colorSprite);
        block.SetOpacity(1f);
        
        Blocks.Add(block);
    }

    private void SpawnPreviewBlock(Sprite colorSprite, Vector2 cellPos)
    {
        Block preview = BlockFactory.Instance.Get();
        preview.transform.SetParent(previewParent);
        preview.transform.localPosition = cellPos;
        preview.transform.localScale = Vector3.one;

        preview.SetSprite(colorSprite);
        preview.SetOpacity(0.5f);
        preview.gameObject.SetActive(false);

        previewBlocks.Add(preview);
    }

    public void SetPreviewsActive(bool active)
    {
        foreach (Block preview in previewBlocks)
        {
            preview.gameObject.SetActive(active);
        }
    }

    public void SetPreviewPositions(Span<Vector2> previewPositions)
    {
        int length = previewPositions.Length;
        for(int i=0; i<length; ++i)
        {
            previewBlocks[i].transform.position = previewPositions[i];
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Sprite GetCurrentColorSprite() => currentColorSprite;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetColorSprite(Sprite sprite)
    {
        if(sprite == currentColorSprite) return;

        foreach(var block in Blocks)
        {
            block.SetSprite(sprite);
        }
        currentColorSprite = sprite;
    }

    public void ResetPosition()
    {
        transform.localPosition = originPosition;
    }

    public void OnPlaced()
    {
        transform.localScale = Vector3.one;
        IsPlaced = true;
        Blocks.Clear();
        ReleaseAllBlocks();
    }

    private void ReleaseAllBlocks()
    {
        foreach(var b in Blocks) BlockFactory.Instance.Release(b);
        foreach(var b in previewBlocks) BlockFactory.Instance.Release(b);
        previewBlocks.Clear();
        Blocks.Clear();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputManager.Instance.OnBeginDragGroup(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        InputManager.Instance.OnDragGroup(this, eventData);        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        InputManager.Instance.OnEndDragGroup(this, eventData);
    }
}
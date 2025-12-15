using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class Board : MonoBehaviour
{
    [field: SerializeField] public Vector2 CellSize { get; private set; } = new Vector2(1.14f, 1.14f);
    [SerializeField] private Vector2 cellGap = new Vector2(0.04f, 0.04f);
    [SerializeField] private Vector2 gridOffset = new Vector2(0f, 0.1f);
    public new Transform transform { get; private set; }
    public Vector2 CellStep {get; private set; }
    public Vector2 CellPivot {get; private set; }
    private Vector2 center;
    private Cell[,] boardGrid;

    private void Awake()
    {
        transform = base.transform;
    }

    public void Init(Vector2Int boardSize)
    {
        boardGrid = new Cell[boardSize.x, boardSize.y];
        FillCells();
        CellStep = CellSize + cellGap;
        CellPivot = CellSize * 0.5f + gridOffset;
        center = new Vector2(GetWidth(), GetHeight()) * 0.5f;  
    }

    private void FillCells()
    {
        float row = GetHeight();
        float col = GetWidth();
        for(int r = 0; r < row; ++r)
        {
            for(int c = 0; c < col; ++c)
            {
                boardGrid[c,r] = new Cell();
            }
        }
    }

    public bool CanPlaceHoverBlockGroup(BlockGroup blockGroup, Span<Vector2> outputPositions)
    {
        ClearHighlight();
        int count = blockGroup.Blocks.Count;
        for (int i = 0; i < count; ++i)
        {
            Block block = blockGroup.Blocks[i];
            Vector2Int boardCellPos = WorldToCellPosition(block.transform.position);
            Cell cell = GetCell(boardCellPos.x, boardCellPos.y);
            if (cell == null || !cell.CanPlace())
            {
                return false;
            }

            outputPositions[i] = CellToWorldPosition(boardCellPos.x, boardCellPos.y);
        }

        HighLightConnectable(blockGroup);
        return true;
    }

    private void HighLightConnectable(BlockGroup hoveringGroup)
    {
        int count = hoveringGroup.Blocks.Count;
        Span<Vector2Int> incoming = stackalloc Vector2Int[count];
        Span<int> touchedRows = stackalloc int[count];
        Span<int> touchedCols = stackalloc int[count];

        for(int i=0; i<count; ++i)
        {
            var block = hoveringGroup.Blocks[i];
            Vector2Int cellPos = WorldToCellPosition(block.transform.position);

            if(!incoming.Contains(cellPos)) incoming[i] = cellPos;
            if(!touchedCols.Contains(cellPos.x)) touchedCols[i] = cellPos.x;
            if(!touchedRows.Contains(cellPos.y)) touchedRows[i] = cellPos.y;
        }

        foreach (int row in touchedRows)
        {
            if (IsHoveringRowFull(row, incoming)) HighlightRow(row, hoveringGroup.GetCurrentColorSprite());
        }

        foreach (int col in touchedCols) 
        {
            if (IsHoveringColFull(col, incoming)) HighlightColumn(col, hoveringGroup.GetCurrentColorSprite());
        }
    }

    private bool IsHoveringRowFull(int row, Span<Vector2Int> incomingBlocks)
    {
        int col = GetWidth();
        for (int c = 0; c < col; c++)
        {
            if (!GetCell(c, row).CanPlace()) continue;
            if (incomingBlocks.Contains(new Vector2Int(c, row))) continue;

            return false;
        }
        return true;
    }

    private bool IsHoveringColFull(int col, Span<Vector2Int> incomingBlocks)
    {
        int row = GetHeight();
        for (int r = 0; r < row; r++)
        {
            if (!GetCell(col, r).CanPlace()) continue;
            if (incomingBlocks.Contains(new Vector2Int(col, r))) continue;

            return false;
        }
        return true;
    }

    private void HighlightRow(int rowIndex, Sprite highlightSprite)
    {
        for (int col = 0; col < GetWidth(); col++)
        {
            boardGrid[col, rowIndex].Hightlight(highlightSprite);
        }
    }

    private void HighlightColumn(int colIndex, Sprite highlightSprite)
    {
        for (int row = 0; row < GetHeight(); row++)
        {
            boardGrid[colIndex, row].Hightlight(highlightSprite);
        }
    }
    
    public void ClearHighlight()
    {
        int row = GetHeight();
        int col = GetWidth();
        for (int r = 0; r < row; ++r)
        {
            for (int c = 0; c < col; ++c)
            {
                boardGrid[c, r].UnHighlight();
            }
        }
    }

    public void PlaceBlockGroup(BlockGroup placeblockGroup, Span<Vector2> placePositions)
    {
        int count = placeblockGroup.Blocks.Count;
        for(int i=0; i < count; ++i)
        {
            Block placeBlock = placeblockGroup.Blocks[i];
            Vector2Int placeCellPos = WorldToCellPosition(placePositions[i]);
            Cell placeCell = GetCell(placeCellPos.x, placeCellPos.y);
            placeCell.PlaceBlock(placeBlock);

            placeBlock.transform.position = CellToWorldPosition(placeCellPos.x, placeCellPos.y);
            placeBlock.transform.SetParent(transform);
        }

        placeblockGroup.OnPlaced();
        UpdateBoardConnections();
    }

    public void UpdateBoardConnections()
    {
        CheckRowConnections();
        CheckColumnConnections();
    }

    private void CheckRowConnections()
    {
        int width = GetWidth();
        int height = GetHeight();

        for (int r = 0; r < height; r++)
        {
            bool fullRow = true;
            for (int c = 0; c < width; c++)
            {
                if (boardGrid[c, r].CanPlace())
                {
                    fullRow = false;
                    break;
                }
            }

            if (fullRow)
            {
                for (int c = 0; c < width; c++)
                {
                    boardGrid[c, r].OnConnected();
                }
            }
        }
    }

    private void CheckColumnConnections()
    {
        int width = GetWidth();
        int height = GetHeight();

        for (int c = 0; c < width; c++)
        {
            bool isColFull = true;
            for (int r = 0; r < height; r++)
            {
                if (boardGrid[c, r].CanPlace())
                {
                    isColFull = false;
                    break;
                }
            }

            if (isColFull)
            {
                for (int r = 0; r < height; r++)
                {
                    boardGrid[c, r].OnConnected();
                }
            }
        }
    }

    public bool CanShapeFitInBoard(byte[,] shape)
    {
        int shapeHeight = shape.GetLength(0);
        int shapeWidth = shape.GetLength(1);
        int rowLimit = GetHeight() - shapeHeight;
        int colLimit = GetWidth() - shapeWidth;

        for(int r = 0; r <= rowLimit; ++r)
        {
            for(int c = 0; c <= colLimit; ++c)
            {
                if(CanPlaceShapeAt(shape,shapeHeight,shapeWidth,r,c)) return true;
            }
        }
        return false;
    }

    private bool CanPlaceShapeAt(byte[,] shape, int shapeHeight, int shapeWidth, int startRow, int startCol)
    {
        for (int r = 0; r < shapeHeight; ++r)
        {
            for (int c = 0; c < shapeWidth; ++c)
            {
                if (shape[r, c] > 0 && !boardGrid[startCol + c, startRow + r].CanPlace())
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Clear()
    {
        if(boardGrid==null) return;

        int row = GetHeight();
        int col = GetWidth();

        for(int r=0; r<row; ++r)
        {
            for(int c=0; c<col; ++c)
            {
                boardGrid[r,c].Clear();
            }
        }
    }

    public Vector2 CellToWorldPosition(int col, int row)
    {
        float x = CellPivot.x + (col - center.x) * CellStep.x ;
        float y = CellPivot.y + (row - center.y) * CellStep.y ;

        return (Vector2)transform.position + new Vector2(x, y);
    }

    public Vector2Int WorldToCellPosition(Vector2 worldPos)
    {
        Vector2 localPos = worldPos - (Vector2)transform.position;

        float col = (localPos.x - CellPivot.x) / CellStep.x + center.x;
        float row = (localPos.y - CellPivot.y) / CellStep.y + center.y;

        return new Vector2Int(Mathf.RoundToInt(col), Mathf.RoundToInt(row));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Cell GetCell(int col, int row)
    {
        if (col < 0 || col >= GetWidth() || row < 0 || row >= GetHeight()) return null;
        return boardGrid[col, row];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetWidth() => boardGrid.GetLength(0);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetHeight() => boardGrid.GetLength(1);

#if UNITY_EDITOR
    [SerializeField] private Vector2Int debugSize = new(8,8);
    void OnDrawGizmosSelected()
    {
        if(debugSize.x <= 0 || debugSize.y <= 0) return;

        CellStep = CellSize + cellGap;
        CellPivot = CellSize * 0.5f + gridOffset;
        center = (Vector2)debugSize * 0.5f;
        transform = base.transform;
        
        Gizmos.color = Color.green;
        for (int r = 0; r < debugSize.x; ++r)
        {
            for (int c = 0; c < debugSize.y; ++c)
            {
                Vector2 cellPos = CellToWorldPosition(c, r);
                Gizmos.color = boardGrid[c,r].CanPlace() ? Color.green : Color.red;
                Gizmos.DrawWireCube((Vector3)cellPos, (Vector3)CellSize);
            }
        }
    }
#endif
}

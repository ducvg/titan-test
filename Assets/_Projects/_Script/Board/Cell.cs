using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class Cell
{
    public Block OccupiedBlock { get; private set; }
    private Sprite originalBlockSprite;

    public Cell()
    {
        OccupiedBlock = null;
    }

    public Cell(Block block)
    {
        PlaceBlock(block);
    }

    public void OnConnected()
    {
        Clear();   
    }

    public void PlaceBlock(Block block)
    {
        OccupiedBlock = block;
        originalBlockSprite = block.GetBlockSprite();
    }

    public void Clear()
    {
        if(!OccupiedBlock) return;
        BlockFactory.Instance.Release(OccupiedBlock);
        OccupiedBlock = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool CanPlace()
    {
        return OccupiedBlock == null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Hightlight(Sprite highlightSprite)
    {
        if(!OccupiedBlock) return;
        OccupiedBlock.SetSprite(highlightSprite);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void UnHighlight()
    {
        if(!OccupiedBlock) return;
        OccupiedBlock.SetSprite(originalBlockSprite);
    }
}
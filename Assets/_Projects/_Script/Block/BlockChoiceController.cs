using System;
using UnityEngine;

public sealed class BlockChoiceController : MonoBehaviour
{
    [SerializeField] private BlockDatabase blockDatabase;
    [SerializeField] private BlockGroup[] blockChoices;
    [SerializeField] private Board board;

    public void RegenerateChoices()
    {
        int length = blockChoices.Length;
        Span<int> spriteColors = stackalloc int[length];
        blockDatabase.GetRandomColorIndexes(spriteColors);
        for (int i = 0; i < length; i++)
        {
            byte[,] shape = blockDatabase.GetRandomColorShape(spriteColors[i]);
            blockChoices[i].Init(shape, blockDatabase.GetColorSprite(spriteColors[i]));
        }
    }

    public bool IsOutOfMove()
    {
        foreach(var blockGroup in blockChoices)
        {
            if(blockGroup.IsPlaced) continue;
            if(board.CanShapeFitInBoard(blockGroup.Shape))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsOutOfChoices()
    {
        foreach(var groupChoice in blockChoices)
        {
            if(!groupChoice.IsPlaced) return false;
        }
        return true;
    }
}
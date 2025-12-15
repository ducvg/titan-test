using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Block Database", menuName = "Data Object/Block Database", order = 1)]
public sealed class BlockDatabase : ScriptableObject
{
    [SerializeField] private Sprite[] blockColorSprites;

    //[Color] [shape id] [shape layout]
    private readonly byte[][][,] blockShapes = new byte[][][,]
    {
        new byte[][,] //0-9
        {
            new byte[,]
            {
                {1}
            },
            new byte[,]
            {
                {1,1},
            },
            new byte[,]
            {
                {1},
                {1}
            },
            new byte[,]
            {
                {0,1},
                {1,0},
            },
            new byte[,]
            {
                {1,0},
                {0,1},
            },
            new byte[,]
            {
                {1,1,1},
            },
            new byte[,]
            {
                {1},
                {1},
                {1},
            },
            new byte[,]
            {
                {0,0,1},
                {0,1,0},
                {1,0,0},
            },
            new byte[,]
            {
                {1,0,0},
                {0,1,0},
                {0,0,1},
            },
            new byte[,]
            {
                {1,1},
                {1,0},
            },
        },
        new byte[][,] //10-19
        {
            new byte[,]
            {
                {0,1},
                {1,1},
            },
            new byte[,]
            {
                {1,0},
                {1,1},
            },
            new byte[,]
            {
                {1,1},
                {0,1},
            },
            new byte[,]
            {
                {1,1},
                {1,1},
            },
            new byte[,]
            {
                {0,0,1},
                {1,1,1},
            },
            new byte[,]
            {
                {0,1,0},
                {1,1,1},
            },
            new byte[,]
            {
                {1,0,0},
                {1,1,1},
            },
            new byte[,]
            {
                {1,1,1,1},
            },
            new byte[,]
            {
                {1,0},
                {1,0},
                {1,0},
                {1,1},
            },
            new byte[,]
            {
                {0,1},
                {0,1},
                {0,1},
                {1,1},
            },
        },
        new byte[][,] //20-29
        {
            new byte[,]
            {
                {1,1,1},
                {0,1,0},
            },
            new byte[,]
            {
                {0,1,0},
                {1,1,1},
            },
            new byte[,]
            {
                {1,0},
                {1,1},
                {1,0},
            },
            new byte[,]
            {
                {0,1},
                {1,1},
                {0,1},
            },
            new byte[,]
            {
                {1,1,0},
                {0,1,1},
            },
            new byte[,]
            {
                {0,1,1},
                {1,1,0},
            },
            new byte[,]
            {
                {0,1,0,0},
                {1,1,1,1},
            },
            new byte[,]
            {
                {0,0,1,0},
                {1,1,1,1},
            },
            new byte[,]
            {
                {1,0,0,0},
                {1,1,1,1},
            },
        },
        new byte[][,] //30-39
        {
            new byte[,]
            {
                {1,1,1,1},
                {1,0,0,0},
            },
            new byte[,]
            {
                {0,1},
                {1,1},
                {1,0},
            },
            new byte[,]
            {
                {1,1,1,1,1},
            },
            new byte[,]
            {
                {1},
                {1},
                {1},
                {1},
                {1},
            },
            new byte[,]
            {
                {1,0,0},
                {1,0,0},
                {1,1,1},
            },
            new byte[,]
            {
                {0,0,1},
                {0,0,1},
                {1,1,1},
            },
            new byte[,]
            {
                {1,1,1},
                {0,0,1},
                {0,0,1},
            },
            new byte[,]
            {
                {1,1,1},
                {1,0,0},
                {1,0,0},
            },
            new byte[,]
            {
                {1,1,1},
                {1,1,1},
            },
            new byte[,]
            {
                {1,1},
                {1,1},
                {1,1},
            },
        }
    };

    public void GetRandomColorIndexes(Span<int> fillOutputBuffer)
    {
        ShuffleSpriteColors();
        int maxColorCount = Math.Min(blockShapes.Length, blockColorSprites.Length);
        RNG.UniqueNumbers(0, maxColorCount, fillOutputBuffer);
    }

    private void ShuffleSpriteColors()
    {
        int n = blockColorSprites.Length;
        for (int i = 0; i < n; ++i)
        {
            int j = Random.Range(0, i);
            (blockColorSprites[i], blockColorSprites[j]) = (blockColorSprites[j], blockColorSprites[i]);
        }
    }

    public byte[,] GetRandomColorShape(int colorIndex)
    {
        var shapeIds = blockShapes[colorIndex];
        int shapeId = Random.Range(0, shapeIds.Length);
        return shapeIds[shapeId];
    }

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Sprite GetColorSprite(int colorIndex) => blockColorSprites[colorIndex];
}
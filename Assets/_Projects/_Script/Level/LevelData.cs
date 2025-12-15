using System;
using UnityEngine;

[Serializable]
public sealed class LevelData 
{
    [field: SerializeField] public Vector2Int BoardSize { get; private set; }
}
using DVG.Common;
using DVG.Common.ObjectPool;
using UnityEngine;

public sealed class BlockFactory : PersistentSingleton<BlockFactory>
{
    [SerializeField] private Block blockPrefab;
    private UnityPool<Block> blockPool;

    protected override void Awake()
    {
        base.Awake();
        blockPool = new UnityPool<Block>();
        blockPool.Init(blockPrefab, defaultCapacity: 100, maxSize: 200);
        blockPool.Preload(64, parent: transform);
    }

    public Block Get()
    {
        var block = blockPool.Get();
        return block;
    }

    public void Release(Block block)
    {
        blockPool.Release(block);
    }
}
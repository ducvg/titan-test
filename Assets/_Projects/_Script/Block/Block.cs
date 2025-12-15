using System.Runtime.CompilerServices;
using UnityEngine;

public sealed class Block : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public new Transform transform {get; private set; }

    private void Awake()
    {
        transform = base.transform;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Sprite GetBlockSprite() => spriteRenderer.sprite;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetOpacity(float opacity)
    {
        Color color = spriteRenderer.color;
        color.a = opacity;
        spriteRenderer.color = color;
    }
}

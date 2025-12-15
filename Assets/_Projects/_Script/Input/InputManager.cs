using System;
using System.Runtime.CompilerServices;
using DVG.Common;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class InputManager : Singleton<InputManager>
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Board board;
    [SerializeField] private Sprite unplaceableSprite;
    [SerializeField] private BlockChoiceController blockChoiceController;
    private Sprite originalDragSprite;

    public void OnBeginDragGroup(BlockGroup blockGroup, PointerEventData eventData)
    {
        originalDragSprite = blockGroup.GetCurrentColorSprite();
        blockGroup.transform.localScale = Vector3.one;
        blockGroup.transform.localPosition += Vector3.up * 2f;
    }   

    public void OnDragGroup(BlockGroup blockGroup, PointerEventData eventData)
    {
        blockGroup.transform.localPosition += GetPointerWorldDelta(eventData);
        Span<Vector2> previewPos = stackalloc Vector2[blockGroup.Blocks.Count];
        if(board.CanPlaceHoverBlockGroup(blockGroup, previewPos))
        {
            blockGroup.SetColorSprite(originalDragSprite);
            blockGroup.SetPreviewsActive(true);
            blockGroup.SetPreviewPositions(previewPos);
        }
        else
        {
            blockGroup.SetColorSprite(unplaceableSprite);
            blockGroup.SetPreviewsActive(false);
        }
    }

    public void OnEndDragGroup(BlockGroup blockGroup, PointerEventData eventData)
    {
        blockGroup.SetColorSprite(originalDragSprite);

        Span<Vector2> position = stackalloc Vector2[blockGroup.Blocks.Count];
        if(board.CanPlaceHoverBlockGroup(blockGroup, position))
        {
            board.PlaceBlockGroup(blockGroup, position);
            if(blockChoiceController.IsOutOfChoices())
            {
                blockChoiceController.RegenerateChoices();
            }
            if(blockChoiceController.IsOutOfMove())
            {
                UIManager.Instance.SetLoseCanvasActive(true);
            }
        }
        else
        {
            blockGroup.transform.localScale = Vector3.one * 0.5f;
            blockGroup.SetPreviewsActive(false);
        }

        blockGroup.ResetPosition();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Vector3 GetPointerWorldDelta(PointerEventData eventData)
    {
        Vector3 currentWorldPos = mainCamera.ScreenToWorldPoint(eventData.position);
        Vector3 previousWorldPos = mainCamera.ScreenToWorldPoint(eventData.position - eventData.delta);
        return currentWorldPos - previousWorldPos;
    }
}
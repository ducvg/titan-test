using DVG.Common;
using UnityEngine;

public sealed class UIManager : Singleton<UIManager>
{
    [SerializeField] private BaseCanvas loseCanvas;

    public void SetLoseCanvasActive(bool active)
    {
        loseCanvas.gameObject.SetActive(active);
    }
}

public abstract class BaseCanvas : MonoBehaviour
{
}
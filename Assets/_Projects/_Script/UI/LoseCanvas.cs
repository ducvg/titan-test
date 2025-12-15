public sealed class LoseCanvas : BaseCanvas
{
    public void Retry()
    {
        UIManager.Instance.SetLoseCanvasActive(false);
        LevelManager.Instance.StartLevel();
    }
}
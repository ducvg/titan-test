using DVG.Common;
using UnityEngine;

public sealed class LevelManager : Singleton<LevelManager>
{
    [field: SerializeField] public Board Board {get; private set;}
    [SerializeField] private LevelData levelData;
    [SerializeField] private BlockChoiceController blockChoiceController;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 61;    
    }

    void Start()
    {
        StartLevel();
    }

    public void StartLevel()
    {
        Board.Clear();
        Board.Init(levelData.BoardSize);
        blockChoiceController.RegenerateChoices();
    }
}

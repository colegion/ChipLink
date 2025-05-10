using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Helpers;
using Interfaces;
using Pool;
using ScriptableObjects;
using ScriptableObjects.Level;
using UnityEngine;
using Grid = GridSystem.Grid;

public class GameController : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PoolController poolController;
    [SerializeField] private InputController inputController;

    private LevelManager _levelManager;
    private ChipConfigManager _chipConfigManager;
    private Grid _grid;
    private LinkSearcher _linkSearcher;
    private ShuffleManager _shuffleManager;
    private LevelProgressTracker _tracker;

    private readonly List<ITappable> _currentLink = new List<ITappable>();
    private readonly Dictionary<int, HashSet<int>> _columnEmptyRows = new();
    private List<TileData> _levelTiles = new();
    private List<BaseTile> _lastHighlightedTiles = new();
    
    private static GameController _instance;

    public static GameController Instance
    {
        get { return _instance; }
    }

    public int GridWidth => levelConfig.boardWidth;
    public int GridHeight => levelConfig.boardHeight;

    public static event Action<LevelConfig> OnLevelLoaded;
    public static event Action<LevelTargetConfig> OnSuccessfulMove;
    public static event Action<bool> OnGameOver;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void LoadLevel()
    {
        _grid = ServiceLocator.Get<Grid>();
        poolController.Initialize();
        cameraController.SetGridSize(GridWidth, GridHeight);
        _levelManager = new LevelManager(puzzleParent);
        _chipConfigManager = ServiceLocator.Get<ChipConfigManager>();
        _tracker = new LevelProgressTracker(levelConfig);
        _linkSearcher = new LinkSearcher(_grid);
        ServiceLocator.Register(_linkSearcher);
        _shuffleManager = ServiceLocator.Get<ShuffleManager>();
        inputController.ToggleInput(true);
        OnLevelLoaded?.Invoke(levelConfig);
    }

    public void ReturnPooledObject(IPoolable poolObject)
    {
        poolController.ReturnPooledObject(poolObject);
    }
    
    public void TryAppendToCurrentLink(ITappable tappable)
    {
        if (LinkRules.CanLink(_currentLink, tappable, out bool isBacktracking))
        {
            if (isBacktracking)
            {
                _currentLink.RemoveAt(_currentLink.Count - 1);
            }
            else
            {
                _currentLink.Add(tappable);
            }
        }

        if(_currentLink.Count > 0)
            HighlightAdjacentTiles(_currentLink[^1] as BaseTile);
    }

    private void HighlightAdjacentTiles(BaseTile origin)
    {
        ClearPreviousHighlights();
        Vector2Int originPos = origin.GetPosition();
        ChipType originType = origin.ChipType;

        foreach (var dir in DirectionUtils.Directions.Values)
        {
            Vector2Int checkPos = originPos + dir;
            var cell = _grid.GetCell(checkPos.x, checkPos.y);

            if (cell != null && cell.GetTile(Utilities.DefaultChipLayer) is BaseTile neighbor)
            {
                if (neighbor.ChipType == originType)
                    neighbor.HighlightView(HighlightType.Bright);
                else
                    neighbor.HighlightView(HighlightType.Dark);

                _lastHighlightedTiles.Add(neighbor);
            }
        }
    }

    private void ClearPreviousHighlights()
    {
        foreach (var tile in _lastHighlightedTiles)
            tile.HighlightView(HighlightType.None);

        _lastHighlightedTiles.Clear();
    }
    
    public void HandleOnRelease()
    {
        if (_currentLink.Count == 0) return;
        if (_currentLink.Count < Utilities.LinkThreshold)
        {
            _currentLink[^1].OnRelease();
            _currentLink.Clear();
            return;
        }

        ClearPreviousHighlights();
        FillFallConfig();
        StartCoroutine(ProcessLink());
    }

    private void FillFallConfig()
    {
        _columnEmptyRows.Clear();

        foreach (BaseTile tile in _currentLink.OfType<BaseTile>())
        {
            if (!_columnEmptyRows.TryGetValue(tile.X, out var emptyRows))
            {
                emptyRows = new HashSet<int>();
                _columnEmptyRows[tile.X] = emptyRows;
            }

            emptyRows.Add(tile.Y);
        }
    }

    private IEnumerator ProcessLink()
    {
        foreach (BaseTile tile in _currentLink.OfType<BaseTile>())
        {
            tile.OnLinked();
            yield return new WaitForSeconds(0.07f);
        }

        var config = new LevelTargetConfig();
        config.targetType = ((BaseTile)_currentLink[0]).ChipType;
        config.count = _currentLink.Count;
        _tracker.RegisterMove(config);
        OnSuccessfulMove?.Invoke(config);

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(DropAppropriateTiles());
        _currentLink.Clear();
        yield return null;
    }

    private IEnumerator DropAppropriateTiles()
    {
        foreach (var kvp in _columnEmptyRows)
        {
            int column = kvp.Key;
            var emptyYSet = kvp.Value;

            int emptyBelow = 0;

            for (int y = 0; y < _grid.Height; y++)
            {
                if (emptyYSet.Contains(y))
                {
                    emptyBelow++;
                }
                else
                {
                    BaseTile tile = _grid.GetCell(column, y)?.GetTile(Utilities.DefaultChipLayer);
                    if (tile != null && emptyBelow > 0)
                    {
                        tile.UpdatePosition(new Vector2Int(column, y - emptyBelow));
                    }
                }
            }

            yield return new WaitForSeconds(0.09f);
        }

        yield return new WaitForSeconds(0.2f);

        StartCoroutine(SpawnNewTiles());
    }

    private IEnumerator SpawnNewTiles()
    {
        foreach (var kvp in _columnEmptyRows)
        {
            int column = kvp.Key;
            var emptyRows = _grid.GetEmptyRowIndexesInColumn(column);
            foreach (int emptyRowIndex in emptyRows)
            {
                Debug.Log($"Spawning in column {column} - Empty Rows: {string.Join(",", emptyRows)}");

                BaseTile newTile = poolController.GetPooledObject(PoolableTypes.BaseTile) as BaseTile;
                if (newTile != null)
                {
                    int targetZ = emptyRowIndex;
                    newTile.transform.SetParent(puzzleParent);
                    newTile.ConfigureSelf(_chipConfigManager.GetRandomConfig(), column, targetZ);
                    float spawnHeight = _grid.Height + 1f;
                    Vector3 spawnPos = new Vector3(column, 0, spawnHeight);
                    newTile.transform.position = spawnPos;

                    newTile.UpdatePosition(new Vector2Int(column, targetZ));
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        _columnEmptyRows.Clear();

        if (!_linkSearcher.HasPossibleLink())
        {
            _shuffleManager.TriggerShuffle();
        }
    }

    public void AppendLevelTiles(TileData data)
    {
        _levelTiles.Add(data);
    }

    public void RemoveDataFromLevelTiles(TileData data)
    {
        _levelTiles.Remove(data);
    }

    private void OnDestroy()
    {
        levelConfig.moveLimit = _tracker.GetRemainingMoves();
        levelConfig.levelTargets = _tracker.GetRemainingTargets();

        var levelData = new LevelData
        {
            levelConfig = levelConfig,
            tiles = _levelTiles
        };

        _levelManager.SaveLevel(levelData);
    }

    public void OnLevelFinished(bool isSuccess)
    {
        inputController.ToggleInput(false);
        OnGameOver?.Invoke(isSuccess);
    }

    public ChipConfigManager GetChipConfigManager()
    {
        return _chipConfigManager;
    }

    public Transform GetPuzzleParent()
    {
        return puzzleParent;
    }

    [ContextMenu("is link possible")]
    public void TestLinkSearcher()
    {
        Debug.Log("Link possible? : " + _linkSearcher.HasPossibleLink());
    }
    
    [ContextMenu("shuffle")]
    public void TestShuffle()
    {
        _shuffleManager.TriggerShuffle();
    }
}
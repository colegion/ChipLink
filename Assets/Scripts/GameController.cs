using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Helpers;
using Interfaces;
using Pool;
using UnityEngine;
using Grid = GridSystem.Grid;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private PoolController poolController;
    [SerializeField] private int width, height;

    private LevelManager _levelManager;
    private Grid _grid;
    
    private List<ITappable> _currentLink = new List<ITappable>();
    private Dictionary<int, ColumnFallConfig> _columnFallConfigs = new Dictionary<int, ColumnFallConfig>();
    
    private static GameController _instance;

    public static GameController Instance
    {
        get
        {
            return _instance;
        }
    }
    
    public int GridWidth => width;
    public int GridHeight => height;
    
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
        cameraController.SetGridSize(width, height);
        _levelManager = new LevelManager(puzzleParent);
    }

    public void ReturnPooledObject(IPoolable poolObject)
    {
        poolController.ReturnPooledObject(poolObject);
    }

    public void TryAppendToCurrentLink(ITappable tappable)
    {
        if (LinkRules.CanLink(_currentLink, tappable))
        {
            _currentLink.Add(tappable);
        }
    }

    public void HandleOnRelease()
    {
        FillFallConfig();
        StartCoroutine(ProcessLink());
    }

    private void FillFallConfig()
    {
        foreach (BaseTile tile in _currentLink.OfType<BaseTile>())
        {
            if (_columnFallConfigs.TryGetValue(tile.X, out var config))
            {
                config.moveCount++;
            }
            else
            {
                _columnFallConfigs[tile.X] = new ColumnFallConfig(tile.X, 1);
            }
        }
    }

    private IEnumerator ProcessLink()
    {
        foreach (BaseTile tile in _currentLink.OfType<BaseTile>())
        {
            tile.OnLinked();
            yield return new WaitForSeconds(0.07f);
        }

        _currentLink.Clear();
        _columnFallConfigs.Clear();
        yield return null;
    }
}

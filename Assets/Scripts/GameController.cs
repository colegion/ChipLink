using System;
using System.Collections;
using System.Collections.Generic;
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

    public void TryAppendToCurrentLink(ITappable tappable)
    {
        if (LinkRules.CanLink(_currentLink, tappable))
        {
            _currentLink.Add(tappable);
        }
    }

    public void HandleOnLinkRequested()
    {
        foreach (var chip in _currentLink)
        {
            Debug.Log("this is a chip " , (chip as MonoBehaviour)?.gameObject);
        }
    }
}

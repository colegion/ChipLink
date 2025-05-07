using System;
using System.Collections;
using System.Collections.Generic;
using Helpers;
using UnityEngine;
using Grid = GridSystem.Grid;

public class GameController : MonoBehaviour
{
    [SerializeField] private Transform puzzleParent;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private int width, height;

    private LevelManager _levelManager;
    private Grid _grid;
    
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

    private void Start()
    {
        _grid = new Grid(width, height);
        ServiceLocator.Register(_grid);
        cameraController.SetGridSize(width, height);
        _levelManager = new LevelManager(puzzleParent);
    }
}

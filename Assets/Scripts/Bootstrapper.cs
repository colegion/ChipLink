using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Helpers;
using Interfaces;
using Pool;
using UnityEngine;
using Grid = GridSystem.Grid;

public class Bootstrapper : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(InitializeDependencies());
    }

    private IEnumerator InitializeDependencies()
    {
        var configManager = FindObjectOfType<ChipConfigManager>();
        while (!configManager.IsReady) 
        {
            yield return null;
        }
        
        ServiceLocator.Register(configManager);

        Debug.Log("Bootstrapper: Registering dependencies...");

        var grid = new Grid(GameController.Instance.GridWidth, GameController.Instance.GridHeight);
        ServiceLocator.Register(grid);

        var poolController = FindObjectOfType<PoolController>();
        var tileFactory = FindObjectOfType<TileFactory>();
        var highlightController = FindObjectOfType<TileHighlightController>();
        var linkController = FindObjectOfType<TileLinkController>();
        var fallController = FindObjectOfType<TileFallController>();
        var fillController = FindObjectOfType<TileFillController>();
        var shuffleManager = FindObjectOfType<ShuffleController>();
        
        ServiceLocator.Register(poolController);
        ServiceLocator.Register(tileFactory);
        ServiceLocator.Register(highlightController);
        ServiceLocator.Register(linkController);
        ServiceLocator.Register(fallController);
        ServiceLocator.Register(fillController);
        ServiceLocator.Register(shuffleManager);
        
        foreach (var injectable in FindObjectsOfType<MonoBehaviour>().OfType<IInjectable>())
        {
            injectable.InjectDependencies();
        }

        Debug.Log("Bootstrapper: Dependencies injected, loading level...");
        GameController.Instance.LoadFields();
    }
}

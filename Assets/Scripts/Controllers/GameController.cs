using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Helpers;
using Interfaces;
using Pool;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Serialization;
using Grid = GridSystem.Grid;

namespace Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private Transform puzzleParent;
        [SerializeField] private CameraController cameraController;
        [SerializeField] private InputController inputController;

        private PoolController _poolController;
        private LevelManager _levelManager;
        private LinkSearcher _linkSearcher;
        private ShuffleController _shuffleController;
        private LevelProgressTracker _tracker;
        private TileLinkController _linkController;
        private TileHighlightController _highlightController;
        private TileFallController _fallController;
        private TileFillController _fillController;

        private List<TileData> _levelTiles = new();

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

        public void LoadFields()
        {
            _poolController = ServiceLocator.Get<PoolController>();
            _poolController.Initialize();
            cameraController.SetGridSize(GridWidth, GridHeight);
            _linkController = ServiceLocator.Get<TileLinkController>();
            _fallController = ServiceLocator.Get<TileFallController>();
            _fillController = ServiceLocator.Get<TileFillController>();
            _highlightController = ServiceLocator.Get<TileHighlightController>();
            _linkSearcher = new LinkSearcher();
            ServiceLocator.Register(_linkSearcher);
            _shuffleController = ServiceLocator.Get<ShuffleController>();
            _levelManager = new LevelManager(puzzleParent);
            _tracker = new LevelProgressTracker(levelConfig);
            inputController.ToggleInput(true);
            OnLevelLoaded?.Invoke(levelConfig);
        }

        public void ReturnPooledObject(IPoolable poolObject)
        {
            _poolController.ReturnPooledObject(poolObject);
        }

        public void TryAppendToCurrentLink(ITappable tappable)
        {
            _linkController.TryAppendToCurrentLink(tappable);
        }

        public void HighlightAdjacentTiles(BaseTile origin)
        {
            _highlightController.HighlightAdjacentTiles(origin);
        }

        public void HandleOnRelease()
        {
            StartCoroutine(HandleOnReleaseRoutine());
        }

        private IEnumerator HandleOnReleaseRoutine()
        {
            if (!_linkController.IsLinkProcessable()) yield break;

            inputController.ToggleInput(false);

            _highlightController.ClearPreviousHighlights();
            _fallController.FillFallConfig(_linkController.GetCurrentLink());

            bool linkProcessed = false;

            yield return _linkController.TriggerLinkProcess((chipType, count) =>
            {
                if (count >= Utilities.LinkThreshold)
                {
                    var config = new LevelTargetConfig
                    {
                        targetType = chipType,
                        count = count
                    };
                    _tracker.RegisterMove(config);
                    OnSuccessfulMove?.Invoke(config);
                    linkProcessed = true;
                }
            });

            if (!linkProcessed)
                yield break;

            _fallController.TriggerDrop();
            yield return new WaitForSeconds(0.3f);
            _fillController.TriggerFillProcess(_fallController.GetEmptyRowsByColumn());
            yield return new WaitForSeconds(0.3f);

            if (!_linkSearcher.HasPossibleLink())
            {
                _shuffleController.TriggerShuffle();
                yield return new WaitForSeconds(0.5f);
            }

            inputController.ToggleInput(true);
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
            _levelManager.ClearProgress();
            OnGameOver?.Invoke(isSuccess);
        }

        public void RestartLevel()
        {
            _levelTiles.Clear();
            Transform[] children = new Transform[puzzleParent.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = puzzleParent.GetChild(i);
            }

            foreach (Transform child in children)
            {
                var poolable = child.GetComponent<BaseTile>();
                if (poolable != null)
                {
                    ReturnPooledObject(poolable);
                }
            }
            
            ServiceLocator.Get<Grid>().Clear();
            _tracker.Reset();
            _levelManager.CreateRandomBoard(GridWidth, GridHeight);
            inputController.ToggleInput(true);
            OnLevelLoaded?.Invoke(levelConfig);
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
            _shuffleController.TriggerShuffle();
        }
    }
}
using System.Collections.Generic;
using System.IO;
using Controllers;
using UnityEngine;
using GridSystem;
using Pool;
using ScriptableObjects;
using UnityEngine.SceneManagement;
using Grid = GridSystem.Grid;

namespace Helpers
{
    public class LevelManager
    {
        private const string DarkCellPath = "Prefabs/BaseCell";
        private const string LightCellPath = "Prefabs/BaseCell";
        private const string LevelDataResourcePath = "Levels/CurrentLevel";
        private const string PersistentLevelFolder = "Levels";
        private const string PersistentLevelFileName = "CurrentLevel.json";

        private readonly Transform _parent;

        public LevelManager(Transform parent)
        {
            _parent = parent;
            LoadLevel();
        }

        #region Public Methods
        
        public void SaveLevel(LevelData levelData)
        {
            string directory = Path.Combine(Application.persistentDataPath, PersistentLevelFolder);
            string savePath = Path.Combine(directory, PersistentLevelFileName);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonUtility.ToJson(levelData, prettyPrint: true);
            File.WriteAllText(savePath, json);

            Debug.Log($"[LevelManager] Level data saved to: {savePath}");
        }

        #endregion

        #region Private Methods
        
        private void LoadLevel()
        {
            LevelData levelData = TryLoadFromPersistentPath() ?? TryLoadFromResources();

            if (levelData != null)
            {
                Debug.Log("[LevelManager] Loaded level data successfully.");
                InitializeLevel(levelData);
            }
            else
            {
                Debug.LogWarning("[LevelManager] No level data found! Generating a default board.");
                GenerateDefaultBoard();
            }
        }

        private LevelData TryLoadFromPersistentPath()
        {
            string path = Path.Combine(Application.persistentDataPath, PersistentLevelFolder, PersistentLevelFileName);

            if (!File.Exists(path))
            {
                Debug.Log("[LevelManager] No saved level found at persistent path.");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                var levelData = JsonUtility.FromJson<LevelData>(json);
                Debug.Log($"[LevelManager] Loaded level from persistent path: {path}");
                return levelData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[LevelManager] Failed to load level from persistent path: {ex.Message}");
                return null;
            }
        }

        private LevelData TryLoadFromResources()
        {
            var jsonFile = Resources.Load<TextAsset>(LevelDataResourcePath);

            if (jsonFile == null)
            {
                Debug.LogError($"[LevelManager] No default level found in Resources at: {LevelDataResourcePath}");
                return null;
            }

            try
            {
                var levelData = JsonUtility.FromJson<LevelData>(jsonFile.text);
                Debug.Log("[LevelManager] Loaded level from Resources.");
                return levelData;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[LevelManager] Failed to parse level data from Resources: {ex.Message}");
                return null;
            }
        }

        private void InitializeLevel(LevelData levelData)
        {
            int width = levelData.levelConfig.boardWidth;
            int height = levelData.levelConfig.boardHeight;

            CreateCells(width, height);

            var tileFactory = ServiceLocator.Get<TileFactory>();
            var configManager = ServiceLocator.Get<ChipConfigManager>();
            
            foreach (var data in levelData.tiles)
            {
                var tile = tileFactory.SpawnTileByConfig();
                tile.transform.SetParent(GameController.Instance.GetPuzzleParent());
                tile.ConfigureSelf(configManager.GetItemConfig(data.chipType), data.xCoord,
                    data.yCoord);
                tile.SetTransform();
            }
        }

        public void GenerateDefaultBoard()
        {
            int width = GameController.Instance.GridWidth;
            int height = GameController.Instance.GridHeight;

            CreateCells(width, height);
            CreateRandomBoard(width, height);
        }

        private void CreateCells(int width, int height)
        {
            var lightPrefab = Resources.Load<BaseCell>(LightCellPath);
            var darkPrefab = Resources.Load<BaseCell>(DarkCellPath);

            if (lightPrefab == null || darkPrefab == null)
            {
                Debug.LogError("[LevelManager] Cell prefabs not found! Check the prefab paths.");
                return;
            }

            float xOffset = width / 2f;
            float yOffset = height / 2f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var prefab = (x + y) % 2 == 0 ? lightPrefab : darkPrefab;

                    Vector3 cellPosition = new Vector3(x - xOffset, 0, y - yOffset);
                    var cell = Object.Instantiate(prefab, cellPosition, prefab.transform.rotation, _parent);

                    cell.ConfigureSelf(x, y);
                }
            }

            Debug.Log($"[LevelManager] Created {width}x{height} grid of cells.");
        }

        public void CreateRandomBoard(int width, int height)
        {
            var tileFactory = ServiceLocator.Get<TileFactory>();
            var configManager = ServiceLocator.Get<ChipConfigManager>();
            
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    var randomConfig = configManager.GetRandomConfig();
                    var tile = tileFactory.SpawnTileByConfig();
                    tile.transform.SetParent(GameController.Instance.GetPuzzleParent());
                    tile.ConfigureSelf(randomConfig, i, j);
                    tile.SetTransform();
                }
            }
        }
        
        public void ClearProgress()
        {
            string path = Path.Combine(Application.persistentDataPath, PersistentLevelFolder, PersistentLevelFileName);

            if (File.Exists(path))
            {
                File.Delete(path);
                Debug.Log("[LevelManager] Cleared saved level progress.");
            }
            else
            {
                Debug.Log("[LevelManager] No saved level to clear.");
            }
        }


        #endregion
    }
}

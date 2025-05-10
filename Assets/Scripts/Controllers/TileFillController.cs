using System.Collections;
using System.Collections.Generic;
using GridSystem;
using Helpers;
using Interfaces;
using Pool;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Controllers
{
    public class TileFillController : MonoBehaviour, IInjectable
    {
        private PoolController _poolController;
        private ChipConfigManager _configManager;
        private Grid _grid;
        
        public void InjectDependencies()
        {
            _poolController = ServiceLocator.Get<PoolController>();
            _configManager = ServiceLocator.Get<ChipConfigManager>();
            _grid = ServiceLocator.Get<Grid>();
        }

        public void TriggerFillProcess(Dictionary<int, HashSet<int>> columnEmptyRows)
        {
            StartCoroutine(SpawnNewTiles(columnEmptyRows));
        }
        
        private IEnumerator SpawnNewTiles(Dictionary<int, HashSet<int>> columnEmptyRows)
        {
            foreach (var kvp in columnEmptyRows)
            {
                int column = kvp.Key;
                var emptyRows = _grid.GetEmptyRowIndexesInColumn(column);
                foreach (int emptyRowIndex in emptyRows)
                {
                    Debug.Log($"Spawning in column {column} - Empty Rows: {string.Join(",", emptyRows)}");

                    BaseTile newTile = _poolController.GetPooledObject(PoolableTypes.BaseTile) as BaseTile;
                    if (newTile != null)
                    {
                        int targetZ = emptyRowIndex;
                        var parent = GameController.Instance.GetPuzzleParent();
                        newTile.transform.SetParent(parent);
                        newTile.ConfigureSelf(_configManager.GetRandomConfig(), column, targetZ);
                        var topmostCell = _grid.GetCell(column, _grid.Height-1);
                        float spawnX = topmostCell.transform.position.x;
                        float spawnHeight = topmostCell.transform.position.z + 1f;
                        Vector3 spawnPos = new Vector3(spawnX, 0, spawnHeight);
                        newTile.transform.position = spawnPos;
                        yield return new WaitForSeconds(0.3f);
                        newTile.UpdatePosition(new Vector2Int(column, targetZ));
                    }

                    yield return new WaitForSeconds(0.03f);
                }

                yield return new WaitForSeconds(0.06f);
            }
            
            columnEmptyRows.Clear();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GridSystem;
using Helpers;
using Interfaces;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Controllers
{
    public class TileFallController : MonoBehaviour, IInjectable
    {
        private Grid _grid;
        private readonly Dictionary<int, HashSet<int>> _columnEmptyRows = new();
        
        public void InjectDependencies()
        {
            _grid = ServiceLocator.Get<Grid>();
        }
        
        public void FillFallConfig(List<ITappable> currentLink)
        {
            _columnEmptyRows.Clear();

            foreach (BaseTile tile in currentLink.OfType<BaseTile>())
            {
                if (!_columnEmptyRows.TryGetValue(tile.X, out var emptyRows))
                {
                    emptyRows = new HashSet<int>();
                    _columnEmptyRows[tile.X] = emptyRows;
                }

                emptyRows.Add(tile.Y);
            }
        }

        public void TriggerDrop()
        {
            StartCoroutine(DropAppropriateTiles());
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

                yield return new WaitForSeconds(0.03f);
            }

            yield return new WaitForSeconds(0.06f);
        }

        public Dictionary<int, HashSet<int>> GetEmptyRowsByColumn()
        {
            return _columnEmptyRows;
        }

        public void ClearEmptyRows()
        {
            _columnEmptyRows.Clear();
        }
    }
}

using System.Collections.Generic;
using GridSystem;
using Helpers;
using Interfaces;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace Controllers
{
    public class TileHighlightController : MonoBehaviour, IInjectable
    {
        private Grid _grid;
        private List<BaseTile> _lastHighlightedTiles = new();
        
        public void InjectDependencies()
        {
            _grid = ServiceLocator.Get<Grid>();
        }
        public void HighlightAdjacentTiles(BaseTile origin)
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

        public void ClearPreviousHighlights()
        {
            foreach (var tile in _lastHighlightedTiles)
                tile.HighlightView(HighlightType.None);

            _lastHighlightedTiles.Clear();
        }
    }
}

using System.Collections.Generic;
using Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class Grid
    {
        private BaseCell[,] _board;
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _board = new BaseCell[width, height];
        }

        public void PlaceCell(BaseCell cell)
        {
            _board[cell.X, cell.Y] = cell;
        }

        public BaseCell GetCell(int x, int y)
        {
            if (!IsCoordinateValid(x, y)) return null;
            return _board[x, y];
        }

        public void SetCell(BaseCell cell)
        {
            if (_board[cell.X, cell.Y] != null)
            {
                Debug.LogError($"Specified coordinate already holds for another cell! Coordinate: {cell.X} {cell.Y}");
            }
            else
            {
                _board[cell.X, cell.Y] = cell;
            }
        }

        public void PlaceTileToParentCell(BaseTile tile)
        {
            var cell = _board[tile.X, tile.Y];
            if (cell == null)
            {
                Debug.LogWarning($"Given tile has no valid coordinate X: {tile.X} Y: {tile.Y}");
            }
            else
            {
                cell.SetTile(tile);
            }
        }

        public void ClearTileOfParentCell(BaseTile tile)
        {
            var cell = _board[tile.X, tile.Y];
            if (cell == null)
            {
                Debug.LogWarning($"Given tile has no valid coordinate X: {tile.X} Y: {tile.Y}");
            }
            else
            {
                cell.SetTileNull(tile.Layer);
            }
        }
        
        public List<int> GetEmptyRowIndexesInColumn(int column)
        {
            List<int> emptyRows = new List<int>();
            
            for (int row = 0; row < Height; row++)
            {
                BaseCell cell = GetCell(column, row);
                if (cell != null && cell.GetTile(Utilities.DefaultChipLayer) == null)
                {
                    emptyRows.Add(row);
                }
            }
            Debug.Log($"Empty cells in column {column}: {string.Join(",", emptyRows)}");

            return emptyRows;
        }
        
        public void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var cell = _board[x, y];
                    if (cell != null)
                    {
                        cell.SetTileNull(Utilities.DefaultChipLayer);
                    }
                }
            }

            Debug.Log("[Grid] Cleared all tile references in the grid.");
        }

        public bool IsCoordinateValid(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    }
}
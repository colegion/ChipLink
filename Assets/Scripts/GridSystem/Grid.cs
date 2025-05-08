using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GridSystem
{
    public class Grid
    {
        private BaseCell[,] _board;
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private List<BaseTile> _tilesOnBoard;

        public Grid(int width, int height)
        {
            Width = width;
            Height = height;
            _board = new BaseCell[width, height];
            _tilesOnBoard = new List<BaseTile>();
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
                _tilesOnBoard.Add(tile);
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
                _tilesOnBoard.Remove(tile);
            }
        }
        
        public List<BaseCell> GetColumn(int columnIndex)
        {
            var column = new List<BaseCell>();

            if (columnIndex < 0 || columnIndex >= Width)
            {
                Debug.LogWarning($"Column index {columnIndex} is out of bounds.");
                return column;
            }

            for (int y = 0; y < Height; y++)
            {
                var cell = _board[columnIndex, y];
                if (cell != null)
                {
                    column.Add(cell);
                }
            }

            return column;
        }


        public List<BaseTile> GetAllTilesOnBoard()
        {
            return _tilesOnBoard;
        }
        
        public Transform GetCellTargetByCoordinate(int x, int y)
        {
            return _board[x, y].GetTarget();
        }

        public bool IsCoordinateValid(int x, int y) => x >= 0 && x < Width && y >= 0 && y < Height;

    }
}
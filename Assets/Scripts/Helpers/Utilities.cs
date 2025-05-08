using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
        public static int DefaultChipLayer = 1;
    }
    
    public enum PoolableTypes
    {
        BaseTile,
    }

    public enum ChipType
    {
        Circle,
        Diamond,
        Star,
        Octagon,
    }

    [Serializable]
    public class LevelData
    {
        public int width;
        public int height;
        public int remainingMoves;
        public int currentScore;
        public int targetScore;
        public List<TileData> tiles;
    }

    [Serializable]
    public class TileData
    {
        public int xCoord, yCoord;
        public ChipType chipType;
    }
    
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        UpLeft,
        UpRight,
        DownLeft,
        DownRight
    }

    public static class DirectionUtils
    {
        public static readonly Dictionary<Direction, Vector2Int> Directions = new()
        {
            { Direction.Up, new Vector2Int(0, 1) },
            { Direction.Down, new Vector2Int(0, -1) },
            { Direction.Left, new Vector2Int(-1, 0) },
            { Direction.Right, new Vector2Int(1, 0) },
            { Direction.UpLeft, new Vector2Int(-1, 1) },
            { Direction.UpRight, new Vector2Int(1, 1) },
            { Direction.DownLeft, new Vector2Int(-1, -1) },
            { Direction.DownRight, new Vector2Int(1, -1) }
        };
    }

    [Serializable]
    public class ColumnFallConfig
    {
        public int columnIndex;
        public int moveCount;

        public ColumnFallConfig(int columnIndex, int moveCount)
        {
            this.columnIndex = columnIndex;
            this.moveCount = moveCount;
        }
    }
}

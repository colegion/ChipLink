using System;
using System.Collections.Generic;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
        public static int LinkThreshold = 3;
        public static int DefaultChipLayer = 1;
    }
    
    public enum PoolableTypes
    {
        BaseTile,
        TargetUIElement,
        TrailObject
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
        public LevelConfig levelConfig;
        public List<TileData> tiles;
    }

    [Serializable]
    public class TileData
    {
        public int xCoord, yCoord;
        public ChipType chipType;
    }

    public enum HighlightType
    {
        None,
        Dark,
        Bright
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
}

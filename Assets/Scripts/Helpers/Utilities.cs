using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Helpers
{
    public class Utilities : MonoBehaviour
    {
    }
    
    public enum PoolableTypes
    {
        BaseTile,
        
    }

    public enum ChipType
    {
        
    }

    [Serializable]
    public class LevelDta
    {
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
}

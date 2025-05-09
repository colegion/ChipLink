using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObjects/Level/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        public int boardWidth;
        public int boardHeight;
        public int moveLimit;
        public List<LevelTargetConfig> levelTargets;
    }

    [Serializable]
    public struct LevelTargetConfig
    {
        public ChipType targetType;
        public int count;
    }
}
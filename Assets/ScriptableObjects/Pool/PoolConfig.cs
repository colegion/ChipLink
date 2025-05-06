using System;
using System.Collections.Generic;
using Helpers;
using UnityEngine;

namespace ScriptableObjects.Pool
{
    [CreateAssetMenu(fileName = "GlobalPoolConfig", menuName = "ScriptableObjects/Pool/PoolConfig")]
    public class PoolConfig : ScriptableObject
    {
        public List<PoolEntry> poolConfigs;
    }

    [Serializable]
    public class PoolEntry
    {
        public PoolableTypes poolItemType;
        public int poolCount;
    }
}
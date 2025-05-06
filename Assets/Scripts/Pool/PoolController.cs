using System;
using Helpers;
using Interfaces;
using ScriptableObjects.Pool;
using UnityEngine;

namespace Pool
{
    public class PoolController : MonoBehaviour
    {
        [SerializeField] private PoolConfig poolConfig;
        private const int DefaultPoolAmount = 100;
        private GamePool _gamePool;

        public void Initialize()
        {
            _gamePool = new GamePool();
            InitializePool();   
        }

        private void InitializePool()
        {
            foreach (var config in poolConfig.poolConfigs)
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/{config.poolItemType}");
                if (prefab == null)
                {
                    Debug.LogError($"Prefab for {config.poolItemType} not found in Resources/Prefabs!");
                    continue;
                }

                var poolable = prefab.GetComponent<IPoolable>();
                if (poolable == null)
                {
                    Debug.LogError($"Prefab for {config.poolItemType} does not implement IPoolable!");
                    continue;
                }

                _gamePool.PoolObjects(config.poolItemType, poolable, config.poolCount, transform);
            }
        }

        public IPoolable GetPooledObject(PoolableTypes type)
        {
            return _gamePool.FetchFromPool(type);
        }

        public void ReturnPooledObject(IPoolable poolObject)
        {
            var poolGo = poolObject.GetGameObject(); 
            poolGo.transform.SetParent(transform);
            poolGo.transform.localPosition = Vector3.zero;
            _gamePool.ReturnToPool(poolObject.GetPoolableType(), poolObject);
        }
    }
}
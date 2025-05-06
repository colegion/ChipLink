using System.Collections.Generic;
using Helpers;
using Interfaces;
using UnityEngine;

namespace Pool
{
    public class GamePool
    {
        private readonly Dictionary<PoolableTypes, Queue<IPoolable>> _pools = new();

        public void PoolObjects(PoolableTypes type, IPoolable poolObject, int amount, Transform parent)
        {
            if (!_pools.ContainsKey(type))
                _pools[type] = new Queue<IPoolable>();

            for (int i = 0; i < amount; i++)
            {
                var instance = Object.Instantiate((poolObject as MonoBehaviour)?.gameObject, parent).GetComponent<IPoolable>();
                instance.OnPooled();
                _pools[type].Enqueue(instance);
            }
        }

        public IPoolable FetchFromPool(PoolableTypes type)
        {
            if (_pools.ContainsKey(type) && _pools[type].Count > 0)
            {
                var poolable = _pools[type].Dequeue();
                poolable.OnFetchFromPool();
                return poolable;
            }

            Debug.LogWarning($"No objects of type {type} available in the pool. Consider pre-pooling more objects.");
            return null;
        }

        public void ReturnToPool(PoolableTypes type, IPoolable poolObject)
        {
            if (!_pools.ContainsKey(type))
                _pools[type] = new Queue<IPoolable>();
            poolObject.OnReturnPool();
            _pools[type].Enqueue(poolObject);
        }
    }
}
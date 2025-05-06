using System;
using System.Collections.Generic;
using ScriptableObjects;
using ScriptableObjects.Chip;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = System.Random;

namespace Helpers
{
    public class ChipConfigManager : MonoBehaviour
    {
        private List<ChipConfig> _chipConfigs;
        public List<AssetReference> itemConfigReferences;

        private int _pendingLoads = 0; 
        
        public bool IsReady { get; private set; } = false;
        
        private void Awake()
        {
            _chipConfigs = new List<ChipConfig>();
            LoadItemConfigs();
        }

        private void LoadItemConfigs()
        {
            _pendingLoads = itemConfigReferences.Count;

            foreach (var reference in itemConfigReferences)
            {
                reference.LoadAssetAsync<ChipConfig>().Completed += OnItemConfigLoaded;
            }
        }

        private void OnItemConfigLoaded(AsyncOperationHandle<ChipConfig> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                var itemConfig = handle.Result;
                _chipConfigs.Add(itemConfig);
            }
            else
            {
                Debug.LogError("Failed to load ItemConfig.");
            }

            _pendingLoads--;

            if (_pendingLoads == 0)
            {
                ServiceLocator.Register(this);
                IsReady = true;
                Debug.LogWarning("All item configs loaded.");
            }
        }

        public ChipConfig GetItemConfig(ChipType itemType)
        {
            var config = _chipConfigs.Find(c => c.chipType == itemType);
            return config;
        }

        public ChipConfig GetRandomConfig()
        {
            var index = UnityEngine.Random.Range(0, _chipConfigs.Count);
            var config = _chipConfigs[index];
                
            return config;
        }
    }
}
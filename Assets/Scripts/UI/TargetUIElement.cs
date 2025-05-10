using Helpers;
using Interfaces;
using ScriptableObjects.Level;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TargetUIElement : MonoBehaviour, IPoolable
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private RectTransform trailTarget;
        [SerializeField] private Image targetImage;
        [SerializeField] private TextMeshProUGUI targetField;
        
        private LevelTargetConfig _config;

        public void ConfigureSelf(LevelTargetConfig config)
        {
            _config = new LevelTargetConfig
            {
                targetType = config.targetType,
                count = config.count
            };
            var configManager = ServiceLocator.Get<ChipConfigManager>();
            targetImage.sprite = configManager.GetItemConfig(_config.targetType).chipSprite;
            targetField.text = $"{_config.count}";
        }

        public void HandleOnMove(LevelTargetConfig moveConfig)
        {
            _config.count -= moveConfig.count;
            if(_config.count < 0)
                _config.count = 0;
            targetField.text = $"{_config.count}";
        }

        public RectTransform GetTarget()
        {
            return trailTarget;
        }

        public void OnPooled()
        {
            visuals.SetActive(false);
        }

        public void OnFetchFromPool()
        {
            visuals.SetActive(true);
            transform.rotation = Quaternion.Euler(Vector3.zero);
            transform.localScale = Vector3.one;
        }

        public void OnReturnPool()
        {
            ResetSelf();
        }

        private void ResetSelf()
        {
            visuals.SetActive(false);
            _config = null;
            targetField.text = "";
            targetImage.sprite = null;
        }

        public PoolableTypes GetPoolableType()
        {
            return PoolableTypes.TargetUIElement;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}

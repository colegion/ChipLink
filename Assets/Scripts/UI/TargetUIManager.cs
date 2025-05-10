using System.Collections.Generic;
using Controllers;
using DG.Tweening;
using Helpers;
using Pool;
using ScriptableObjects.Level;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TargetUIManager : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup layoutGroup;
        [SerializeField] private float offset;
        [SerializeField] private float baseDelay;
        private Dictionary<ChipType, TargetUIElement> _targetUIs = new();
        private PoolController _poolController;

        public void Initialize(List<LevelTargetConfig> targets)
        {
            _poolController = ServiceLocator.Get<PoolController>();
            foreach (var config in targets)
            {
                var pooledElement = _poolController.GetPooledObject(PoolableTypes.TargetUIElement);
                var element = pooledElement.GetGameObject().GetComponent<TargetUIElement>();
                element.transform.SetParent(layoutGroup.transform, false);
                element.transform.localPosition = Vector3.zero;
                element.ConfigureSelf(config);
                _targetUIs.Add(config.targetType, element);
            }
        }

        public void OnMove(LevelTargetConfig moveConfig)
        {
            if (!_targetUIs.TryGetValue(moveConfig.targetType, out var element)) return;
            var totalCount = moveConfig.count;
            moveConfig.count = 1;
            var configManager = ServiceLocator.Get<ChipConfigManager>();
            Sequence sequence = DOTween.Sequence();

            for (int i = 0; i < totalCount; i++)
            {
                var pooledTrail = _poolController.GetPooledObject(PoolableTypes.TrailObject);
                var trailGo = pooledTrail.GetGameObject();
                trailGo.transform.position = GetRandomPositionAroundCenter();
                var trail = trailGo.GetComponent<TrailObject>();
                trail.ConfigureSelf(configManager.GetItemConfig(moveConfig.targetType).chipSprite);
                sequence.InsertCallback(baseDelay * (i + 1), () =>
                {
                    trail.MoveTowardsTarget(element.GetTarget(), () =>
                    {
                        element.HandleOnMove(moveConfig);
                        _poolController.ReturnPooledObject(trail);
                    });
                });
            }
        }

        private Vector3 GetRandomPositionAroundCenter()
        {
            Vector3 worldCenter = Vector3.zero;

            float randomX = Random.Range(-offset, offset);
            float randomZ = Random.Range(-offset, offset);

            return worldCenter + new Vector3(randomX, 6, randomZ);
        }

        public void Reset()
        {
            foreach (var pair in _targetUIs)
            {
                var element = pair.Value;
                _poolController.ReturnPooledObject(element);
            }

            _targetUIs.Clear();
        }
    }
}
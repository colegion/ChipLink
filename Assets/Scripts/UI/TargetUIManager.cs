using System.Collections.Generic;
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
        private Dictionary<ChipType, TargetUIElement> _targetUIs = new();

        public void Initialize(List<LevelTargetConfig> targets)
        {
            var poolController = ServiceLocator.Get<PoolController>();
            foreach (var config in targets)
            {
                var pooledElement = poolController.GetPooledObject(PoolableTypes.TargetUIElement);
                var element = pooledElement.GetGameObject().GetComponent<TargetUIElement>();
                element.transform.SetParent(layoutGroup.transform, false);
                element.transform.localPosition = Vector3.zero;
                element.ConfigureSelf(config);
                _targetUIs.Add(config.targetType, element);
            }
        }

        public void OnMove(LevelTargetConfig moveConfig)
        {
            var element = _targetUIs[moveConfig.targetType];
            element.HandleOnMove(moveConfig);
        }
    }
}

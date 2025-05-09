using System;
using ScriptableObjects;
using ScriptableObjects.Level;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHelper : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moveLimitField;
        [SerializeField] private TargetUIManager targetUIManager;

        private int _moveCount;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnLevelLoaded(LevelConfig config)
        {
            _moveCount = config.moveLimit;
            moveLimitField.text = $"{_moveCount}";
            LevelConfig.MergeDuplicateTargets(config.levelTargets);
            targetUIManager.Initialize(config.levelTargets);
        }

        private void HandleOnMove(LevelTargetConfig moveConfig)
        {
            _moveCount--;
            moveLimitField.text = $"{_moveCount}";
            targetUIManager.OnMove(moveConfig);
        }

        private void AddListeners()
        {
            GameController.OnLevelLoaded += HandleOnLevelLoaded;
            GameController.OnSuccessfulMove += HandleOnMove;
        }
        
        private void RemoveListeners()
        {
            GameController.OnLevelLoaded -= HandleOnLevelLoaded;
            GameController.OnSuccessfulMove -= HandleOnMove;
        }
    }
}

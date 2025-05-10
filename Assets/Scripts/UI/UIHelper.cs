using System;
using Controllers;
using ScriptableObjects;
using ScriptableObjects.Level;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIHelper : MonoBehaviour
    {
        [SerializeField] private Canvas mainCanvas;
        [SerializeField] private TextMeshProUGUI moveLimitField;
        [SerializeField] private TargetUIManager targetUIManager;
        [SerializeField] private UIScreenLoader screenLoader;

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
            targetUIManager.Reset();
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

        private void HandleOnGameOver(bool isSuccess)
        {
            screenLoader.LoadPopup(isSuccess, mainCanvas.transform);
        }

        private void AddListeners()
        {
            GameController.OnLevelLoaded += HandleOnLevelLoaded;
            GameController.OnSuccessfulMove += HandleOnMove;
            GameController.OnGameOver += HandleOnGameOver;
        }
        
        private void RemoveListeners()
        {
            GameController.OnLevelLoaded -= HandleOnLevelLoaded;
            GameController.OnSuccessfulMove -= HandleOnMove;
            GameController.OnGameOver -= HandleOnGameOver;
        }
    }
}

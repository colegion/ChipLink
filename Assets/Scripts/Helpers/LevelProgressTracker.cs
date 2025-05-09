using System;
using System.Collections.Generic;
using System.Linq;
using ScriptableObjects.Level;

namespace Helpers
{
    public class LevelProgressTracker
    {
        private List<LevelTargetConfig> _targets;
        private int _remainingMoves;

        public LevelProgressTracker(LevelConfig config)
        {
            _targets = LevelConfig.MergeDuplicateTargets(config.levelTargets)
                .Select(t => new LevelTargetConfig
                {
                    targetType = t.targetType,
                    count = t.count
                }).ToList();

            _remainingMoves = config.moveLimit;
        }

        public void RegisterMove(LevelTargetConfig move)
        {
            _remainingMoves--;

            var target = _targets.FirstOrDefault(t => t.targetType == move.targetType);
            if (target != null)
            {
                target.count = Math.Max(0, target.count - move.count);
            }

            if (CheckIfLevelCompleted())
            {
                GameController.Instance.OnLevelFinished(true);
            }
            else if (_remainingMoves <= 0)
            {
                GameController.Instance.OnLevelFinished(false);
            }
        }

        private bool CheckIfLevelCompleted()
        {
            return _targets.All(t => t.count <= 0);
        }

        public int GetRemainingMoves() => _remainingMoves;
        public List<LevelTargetConfig> GetRemainingTargets() => _targets;
    }
}
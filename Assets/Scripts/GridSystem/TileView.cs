using System;
using DG.Tweening;
using UnityEngine;

namespace GridSystem
{
    public class TileView : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private SpriteRenderer tileRenderer;
        
        public void SetSprite(Sprite sprite)
        {
            tileRenderer.sprite = sprite;
        }

        public void ResetSelf()
        {
            tileRenderer.sprite = null;
        }

        public void ToggleVisuals(bool toggle)
        {
            visuals.SetActive(toggle);
        }

        public void MoveTowardsTarget(Transform target, Action onComplete)
        {
            transform.DOMove(target.position, 0.15f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}
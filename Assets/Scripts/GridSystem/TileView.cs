using System;
using DG.Tweening;
using Helpers;
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

        public void AnimateOnHighlight(bool toggle)
        {
            var targetScale = toggle ? 1.08f : 1f;
            var scaleVector = new Vector3(targetScale, targetScale, targetScale);
            transform.DOScale(scaleVector, 0.15f).SetEase(Ease.OutBack);
        }

        public void Disappear(Action onComplete)
        {
            transform.DOPunchScale(new Vector3(1.01f, 1.01f, 1.01f), .09f).OnComplete(() =>
            {
                transform.DOScale(Vector3.zero, 0.09f).SetEase(Ease.InBounce).OnComplete(() =>
                {
                    onComplete?.Invoke();
                    transform.localScale = Vector3.one;
                });
            });
        }

        public void HighlightSelf(HighlightType type)
        {
            switch (type)
            {
                case HighlightType.None:
                    tileRenderer.color = Color.white;
                    break;
                case HighlightType.Dark:
                    tileRenderer.color = new Color(0.5f, 0.5f, 0.5f, 0.75f);
                    break;
                case HighlightType.Bright:
                    tileRenderer.color = new Color(1f, 1f, 0.4f);
                    break;
            }
        }

        public void MoveTowardsTarget(Transform target, Action onComplete)
        {
            transform.DOMove(target.position, 0.15f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }
    }
}
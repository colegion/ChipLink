using System;
using DG.Tweening;
using Interfaces;
using UnityEngine;

namespace Helpers
{
    public class TrailObject : MonoBehaviour, IPoolable
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        public void ConfigureSelf(Sprite sprite)
        {
            spriteRenderer.sprite = sprite;
            transform.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }

        public void MoveTowardsTarget(RectTransform uiTarget, Action onComplete)
        {
            Camera cam = Camera.main;
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, uiTarget.position);
            Ray ray = cam.ScreenPointToRay(screenPos);
            float targetY = 0f;
            float distance = (targetY - ray.origin.y) / ray.direction.y;
            Vector3 worldPos = ray.GetPoint(distance);

            transform.DOJump(worldPos, 0.5f, 1, 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => onComplete?.Invoke());
        }
        
        public void OnPooled()
        {
            visuals.SetActive(false);
        }

        public void OnFetchFromPool()
        {
            visuals.SetActive(true);
        }

        public void OnReturnPool()
        {
            visuals.SetActive(false);
            spriteRenderer.sprite = null;
            transform.position = Vector3.zero;
        }

        public PoolableTypes GetPoolableType()
        {
            return PoolableTypes.TrailObject;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}


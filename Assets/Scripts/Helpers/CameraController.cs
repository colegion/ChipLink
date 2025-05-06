using UnityEngine;

namespace Helpers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float padding = 1f;

        private int _gridWidth;
        private int _gridHeight;

        public void SetGridSize(int width, int height)
        {
            _gridWidth = width;
            _gridHeight = height;
            AdjustCamera();
        }

        private void AdjustCamera()
        {
            if (mainCamera == null)
                mainCamera = Camera.main;

            if (mainCamera == null)
            {
                Debug.LogError("No main camera assigned!");
                return;
            }
            
            float aspectRatio = (float)Screen.width / Screen.height;
            float verticalSize = (_gridHeight / 2f) + padding;
            float horizontalSize = ((_gridWidth / 2f) + padding) / aspectRatio; 

            mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
        }
    }
}
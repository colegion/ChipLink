using UnityEngine;

namespace UI
{
    public class UIScreenLoader : MonoBehaviour
    {
        private const string SuccessPopupPath = "Prefabs/UI/SuccessInfoPopup";
        private const string FailPopupPath = "Prefabs/UI/FailInfoPopup";

        public void LoadPopup(bool isSuccess, Transform parent)
        {
            var path = isSuccess ? SuccessPopupPath : FailPopupPath;
            var popupPrefab = Resources.Load<GameObject>(path);
    
            if (popupPrefab != null)
            {
                var popupInstance = GameObject.Instantiate(popupPrefab, parent);
                popupInstance.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.LogError($"Failed to load prefab at path: {path}");
            }
        }
    }
}

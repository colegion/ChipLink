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
                var popupInstance = Instantiate(popupPrefab, parent);
                popupInstance.transform.localPosition = Vector3.zero;
                popupInstance.SetActive(true);
            }
            else
            {
                Debug.LogError($"Failed to load prefab at path: {path}");
            }
        }
    }
}

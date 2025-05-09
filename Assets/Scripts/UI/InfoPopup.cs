using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class InfoPopup : MonoBehaviour
    {
        [SerializeField] private Button popupButton;

        private void OnEnable()
        {
            popupButton.onClick.AddListener(LoadScene);
        }

        private void OnDisable()
        {
            popupButton.onClick.RemoveListener(LoadScene);
        }
        
        private void LoadScene()
        {
            SceneManager.LoadScene("Game");
        }
    }
}

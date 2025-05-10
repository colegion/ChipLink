using Controllers;
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
            popupButton.onClick.AddListener(Restart);
        }

        private void OnDisable()
        {
            popupButton.onClick.RemoveListener(Restart);
        }
        
        private void Restart()
        {
            GameController.Instance.RestartLevel();
            gameObject.SetActive(false);
        }
    }
}

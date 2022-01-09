using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class GameOverWindow : MonoBehaviour
    {
        [SerializeField]
        private Button _reloadButton;

        private void OnEnable()
        {
            _reloadButton.onClick.AddListener(OnClickHandler);
        }
        
        private void OnDisable()
        {
            _reloadButton.onClick.RemoveListener(OnClickHandler);
        }

        private void OnClickHandler()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
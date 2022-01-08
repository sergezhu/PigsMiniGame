using System;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;

namespace UI
{
    public class BottomBarView : MonoBehaviour
    {
        public event Action BombButtonClick;
    
        [SerializeField]
        private TMP_Text _scoresText;
        [SerializeField]
        private Button _bombButton;

        public void SetScores(int scores)
        {
            _scoresText.text = $"Scores : {scores}";
        }

        private void OnEnable()
        {
            Subscribe();
        }
    
        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _bombButton.onClick.AddListener(OnBombButtonClick);
        }

        private void Unsubscribe()
        {
            _bombButton.onClick.RemoveListener(OnBombButtonClick);
        }

        private void OnBombButtonClick()
        {
            BombButtonClick?.Invoke();
        }
    }
}

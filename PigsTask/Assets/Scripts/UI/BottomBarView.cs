using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
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
        [SerializeField]
        private Slider _slider;

        private readonly float _duration = 0.5f;

        public void SetScores(int scores)
        {
            _scoresText.text = $"Scores : {scores}";
        }

        public void SetHP(int currentHp, int maxHp)
        {
            var currentValue = _slider.value;
            var targetValue = (float) currentHp / maxHp;

            DOVirtual.Float(currentValue, targetValue, _duration, value => _slider.value = value).SetEase(Ease.OutCubic);
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

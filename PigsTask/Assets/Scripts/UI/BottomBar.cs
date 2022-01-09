using System.Collections.Generic;
using System.Linq;
using Core;
using DG.Tweening;
using UnityEngine;

namespace UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private BottomBarView _view;

        private IBombButtonClickHandler _bombButtonClickHandler;
        private IEnumerable<IEarnScoresProvider> _spawnControllerEarnScoresProviders;

        private int _currentScores;

        public void Initialize(IBombButtonClickHandler bombButtonClickHandler, IEnumerable<IEarnScoresProvider> spawnControllerEarnScoresProviders)
        {
            _bombButtonClickHandler = bombButtonClickHandler;
            _spawnControllerEarnScoresProviders = spawnControllerEarnScoresProviders;

            _currentScores = 0;
            _view.SetScores(_currentScores);

            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();   
        }

        private void Subscribe()
        {
            _view.BombButtonClick += OnBombButtonClick;
            
            _spawnControllerEarnScoresProviders.ToList().ForEach(provider => provider.EarnScoresReady += OnEarnScoresReady);
        }
        
        private void Unsubscribe()
        {
            _view.BombButtonClick -= OnBombButtonClick;
            
            _spawnControllerEarnScoresProviders.ToList().ForEach(provider => provider.EarnScoresReady -= OnEarnScoresReady);
        }

        private void OnEarnScoresReady(int scores)
        {
            var newCurrentScores = _currentScores + scores;

            DOVirtual.Float(_currentScores, newCurrentScores, 0.5f, value => _view.SetScores((int)value))
                .SetEase(Ease.OutQuad);

            _currentScores = newCurrentScores;
        }

        private void OnBombButtonClick()
        {
            _bombButtonClickHandler.HandleBombButtonClick();
        }
    }
}
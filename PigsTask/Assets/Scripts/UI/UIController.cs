using System;
using System.Collections.Generic;
using Core;
using Core.Interfaces;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField]
        private BottomBar _bottomBar;
        [SerializeField]
        private GameOverWindow _gameOverWindow;

        private Player _player;

        public void Initialize(IBombButtonClickHandler bombButtonClickHandler, IEnumerable<IEarnScoresProvider> spawnControllerEarnScoresProviders, Player player)
        {
            _player = player;
            _bottomBar.Initialize(bombButtonClickHandler, spawnControllerEarnScoresProviders, player.Health);

            Subscribe();
        }

        private void Awake()
        {
            _gameOverWindow.Hide();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _player.Dead += OnDead;
        }
        
        private void Unsubscribe()
        {
            _player.Dead -= OnDead;
        }

        private void OnDead()
        {
            _gameOverWindow.Show();
        }
    }
}
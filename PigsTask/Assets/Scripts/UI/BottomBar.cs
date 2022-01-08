using Core;
using UnityEngine;

namespace UI
{
    public class BottomBar : MonoBehaviour
    {
        [SerializeField]
        private BottomBarView _view;

        private IBombButtonClickHandler _bombButtonClickHandler;

        public void Initialize(IBombButtonClickHandler bombButtonClickHandler)
        {
            _bombButtonClickHandler = bombButtonClickHandler;
            
            Subscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();   
        }

        private void Subscribe()
        {
            _view.BombButtonClick += OnBombButtonClick;
        }
        
        private void Unsubscribe()
        {
            _view.BombButtonClick -= OnBombButtonClick;
        }

        private void OnBombButtonClick()
        {
            _bombButtonClickHandler.HandleBombButtonClick();
        }
    }
}
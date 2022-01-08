using System;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.Move;
using Core.View;
using UnityEngine;

namespace Core
{
    public class Player : MonoBehaviour, IBombButtonClickHandler
    {
        public enum PlayerState
        {
            Idle,
            Walk,
            DirtyStun,
        }

        [SerializeField]
        private PlayerView _view;
        [SerializeField]
        private PlayerMover _playerMover;

        private PlayerState _currentState;
        private InputController _inputController;
        private MoveController _moveController;

        public PlayerMover PlayerMover => _playerMover;
        public PlayerState CurrentState => _currentState;

        public async Task Initialize(MoveController moveController, InputController inputController, IAssetProvider assetProvider, int order)
        {
            _moveController = moveController;
            await _view.Initialize(assetProvider);
            _view.SetDirection(moveController.CurrentDirection);
            _view.SetOrder(order);

            _inputController = inputController;
            _playerMover.Initialize(moveController);
            
            Subscribe();
        }

        public void DoUpdate()
        {
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _inputController.CellClick += OnCellClick;
            _moveController.Changed += OnMoveChanged;
        }

        private void Unsubscribe()
        {
            _inputController.CellClick -= OnCellClick;
            _moveController.Changed -= OnMoveChanged;
        }

        public void HandleBombButtonClick()
        {
            Debug.Log("BOMB!");
        }

        private void OnCellClick(GridCell cell)
        {
            _playerMover.HandleCellClick(cell);
        }

        private void OnMoveChanged()
        {
            UpdateView(_moveController.CurrentDirection, _moveController.CurrentPosition.Y + 1);
        }

        private void UpdateView(MoveDirection moveDirection, int order)
        {
            _view.SetDirection(moveDirection);
            _view.SetOrder(order);
        }
    }
}
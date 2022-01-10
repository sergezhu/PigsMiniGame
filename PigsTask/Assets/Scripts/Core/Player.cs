using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.Interfaces;
using Core.Move;
using Core.Spawn;
using Core.View;
using UnityEngine;

namespace Core
{
    public class Player : MonoBehaviour, IBombButtonClickHandler, IExplosionHandler, IDamageableOwner
    {
        public event Action Dead;
        
        public enum PlayerState
        {
            Idle,
            Walk,
            DirtyStun,
            Dead
        }
        
        public const float DirtyStunDuration = 1.5f;


        [SerializeField]
        private PlayerView _view;
        [SerializeField]
        private PlayerMover _playerMover;

        private InputController _inputController;
        private MoveController _moveController;
        private IAssetProvider _assetProvider;
        private SpawnController _spawnController;
        private Coroutine _dirtyStunCoroutine;
        private AStar _pathFinder;
        private Health _health;
        
        private bool _isStunned;

        public PlayerMover PlayerMover => _playerMover;
        public PlayerState CurrentState { get; private set; }
        public Health Health => _health;

        public async Task Initialize(MoveController moveController, InputController inputController, IAssetProvider assetProvider, 
            int order, SpawnController spawnController, AStar pathFinder, Health health)
        {
            _assetProvider = assetProvider;
            _moveController = moveController;
            _inputController = inputController;
            _spawnController = spawnController;
            _health = health;
            
            _pathFinder = pathFinder;

            await _view.Initialize(assetProvider);
            _view.SetDirection(moveController.CurrentDirection);
            _view.SetOrder(order);

            _playerMover.Initialize(moveController);
            
            Subscribe();
        }

        public void DoUpdate()
        {
            HandleState();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _inputController.CellClick += OnCellClick;
            _moveController.Changed += OnMoveChanged;
            _health.Changed += OnHealthChanged;
        }

        private void Unsubscribe()
        {
            _inputController.CellClick -= OnCellClick;
            _moveController.Changed -= OnMoveChanged;
            _health.Changed -= OnHealthChanged;
        }

        public async void HandleBombButtonClick()
        {
            var targetBombCell = _moveController.IsMoving 
                ? _moveController.PreviousCell 
                : _moveController.GetNearCellWithPriorityFront();

            if (targetBombCell == null)
                return;
            
            await _spawnController.SpawnBomb(targetBombCell);
        }

        public void HandleExplosion(GridCell cell, int distance)
        {
            _pathFinder.Initialize(cell.Coords, _moveController.CurrentPosition, AStar.AllowedDirectionsType.EightDirections);
            var distanceBetweenCells = _pathFinder.GetPath().Count;

            if (distanceBetweenCells < distance)
                HandleExplosionInternal();
        }
        
        public IEnumerable<IDamageable> GetAllDamageable()
        {
            return new List<IDamageable>(){Health};
        }

        private void HandleExplosionInternal()
        {
            _isStunned = true;
        }

        private void OnCellClick(GridCell cell)
        {
            _playerMover.HandleCellClick(cell);
        }

        private void OnMoveChanged()
        {
            UpdateView(_moveController.CurrentDirection, _moveController.CurrentPosition.y + 1);
        }
        
        private void OnHealthChanged()
        {
            _view.HandleTakeDamage();
        }

        private void UpdateView(MoveDirection moveDirection, int order)
        {
            _view.SetDirection(moveDirection);
            _view.SetOrder(order);
        }

        private void HandleState()
        {
            switch (CurrentState)
            {
                case PlayerState.Idle:
                    HandleIdle();
                    break;
                case PlayerState.Walk:
                    HandleWalk();
                    break;
                case PlayerState.DirtyStun:
                    HandleDirtyStun();
                    break;
                case PlayerState.Dead:
                    HandleDead();
                    break;
            }
        }

        private void HandleIdle()
        {
            if (_health.IsDead) 
                DeadEnter();
            
            if (_isStunned)
                CurrentState = PlayerState.DirtyStun;
            
            if (_playerMover.IsMoving)
                CurrentState = PlayerState.Walk;
        }

        private void HandleWalk()
        {
            if (_health.IsDead) 
                DeadEnter();
            
            if (_isStunned)
            {
                _playerMover.Stop();
                CurrentState = PlayerState.DirtyStun;
            }
            
            if (_playerMover.IsMoving == false)
                CurrentState = PlayerState.Idle;
        }

        private void HandleDirtyStun()
        {
            if (_health.IsDead) 
                DeadEnter();

            if (_dirtyStunCoroutine == null)
                _dirtyStunCoroutine = StartCoroutine(DirtyStunCoroutine(DirtyStunDuration));

            if (_isStunned == false)
            {
                _dirtyStunCoroutine = null;
                CurrentState = PlayerState.Idle;
            }
        }

        private void HandleDead()
        {
        }

        private IEnumerator DirtyStunCoroutine(float duration)
        {
            _view.EnableDirtyView();
            
            yield return new WaitForSeconds(duration);
            
            _view.EnableDefaultView();
            _isStunned = false;
        }

        private void DeadEnter()
        {
            _playerMover.Stop();
            _playerMover.CleanUp();
            _view.HandleDead();
            Unsubscribe();
            
            CurrentState = PlayerState.Dead;
            
            Dead?.Invoke();
        }
    }
}
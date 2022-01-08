using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using Core.View;
using UnityEngine;

namespace Core
{
    public class Enemy : MonoBehaviour
    {
        public enum EnemyState
        {
            Idle,
            Walk,
            Chasing,
            DirtyStun,
            Attack
        }

        [SerializeField]
        private EnemyView _view;
        [SerializeField]
        private EnemyMover _enemyMover;
        [SerializeField]
        private AggroZone _aggroZone;

        private bool _lockedState;
        
        private DistanceProvider _distanceProvider;
        private List<WaitForSeconds> _cachedWaiters;
        private MoveController _moveController;

        public EnemyMover EnemyMover => _enemyMover;
        public EnemyState CurrentState { get; private set; }
        

        public async Task Initialize(MoveController moveController, IAssetProvider assetProvider)
        {
            _moveController = moveController;
            
            await _view.Initialize(assetProvider);
            UpdateView(moveController.CurrentDirection, moveController.CurrentPosition.Y + 1);

            _enemyMover.Initialize(moveController);
            _aggroZone.Initialize(_enemyMover);

            CurrentState = EnemyState.Idle;
            
            Subscribe();
        }

        public void CreateDistanceProvider(AStar pathFinder, PlayerMover playerMover)
        {
            _distanceProvider = new DistanceProvider(pathFinder, playerMover);
            _aggroZone.DistanceProvider = _distanceProvider;
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
            _moveController.Changed += OnMoveChanged;
        }

        private void Unsubscribe()
        {
            _moveController.Changed -= OnMoveChanged;
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

        private void HandleState()
        {
            switch (CurrentState)
            {
                case EnemyState.Idle:
                    HandleIdle();
                    break;
                case EnemyState.Walk:
                    HandleWalk();
                    break;
                case EnemyState.Chasing:
                    break;
                case EnemyState.DirtyStun:
                    break;
                case EnemyState.Attack:
                    break;
            }
        }

        private void HandleIdle()
        {
            if (_enemyMover.IsRelaxing)
                return;
            
            _enemyMover.TryStartMove();

            if (_enemyMover.IsMoving)
                CurrentState = EnemyState.Walk;
        }
        
        private void HandleWalk()
        {
            if (_enemyMover.IsMoving)
                return;
            
            _enemyMover.TryStartRelax();

            if (_enemyMover.IsRelaxing)
                CurrentState = EnemyState.Idle;
        }
    }
}
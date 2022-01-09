using System;
using System.Collections;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.Move;
using Core.View;
using UnityEngine;

namespace Core
{
    public class Enemy : MonoBehaviour, IExplosionHandler, IEarnScoresProvider
    {
        public enum EnemyState
        {
            Idle,
            Walk,
            Chasing,
            DirtyStun,
            Attack
        }

        public event Action<int> EarnScoresReady;

        public const float DirtyStunDuration = 5f;
        
        [SerializeField]
        private EnemyView _view;
        [SerializeField]
        private EnemyMover _enemyMover;
        [SerializeField]
        private AggroZone _aggroZone;

        private DistanceProvider _distanceProvider;
        private MoveController _moveController;
        private Coroutine _dirtyStunCoroutine;
        private AStar _pathFinder;
        private int _earnScores;
        private bool _isStunned;

        public EnemyMover EnemyMover => _enemyMover;
        public EnemyState CurrentState { get; private set; }


        public async Task Initialize(MoveController moveController, IAssetProvider assetProvider, AStar pathFinder, int earnScores)
        {
            _moveController = moveController;
            _pathFinder = pathFinder;
            _earnScores = earnScores;

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

        public void HandleExplosion(GridCell cell, int distance)
        {
            _pathFinder.Initialize(cell.Coords.AsVector(), _moveController.CurrentPosition.AsVector(), AStar.AllowedDirectionsType.EightDirections);
            var distanceBetweenCells = _pathFinder.GetPath().Count;

            if (distanceBetweenCells < distance)
                HandleExplosionInternal();
        }
        
        private void HandleExplosionInternal()
        {
            EarnScoresReady?.Invoke(_earnScores);

            _isStunned = true;
            Debug.Log("Enemy under explosion effect");
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
                    HandleDirtyStun();
                    break;
                case EnemyState.Attack:
                    break;
            }
        }

        private void HandleIdle()
        {
            if(_isStunned)
                CurrentState = EnemyState.DirtyStun;
            
            if (_enemyMover.IsRelaxing)
                return;
            
            _enemyMover.TryStartMove();

            if (_enemyMover.IsMoving)
                CurrentState = EnemyState.Walk;
        }

        private void HandleWalk()
        {
            if (_isStunned)
            {
                _enemyMover.Stop();
                CurrentState = EnemyState.DirtyStun;
            }
            
            if (_enemyMover.IsMoving)
                return;
            
            _enemyMover.TryStartRelax();

            if (_enemyMover.IsRelaxing)
                CurrentState = EnemyState.Idle;
        }
        
        private void HandleDirtyStun()
        {
            if (_dirtyStunCoroutine == null)
                _dirtyStunCoroutine = StartCoroutine(DirtyStunCoroutine(DirtyStunDuration));

            if (_isStunned == false)
            {
                _dirtyStunCoroutine = null;
                CurrentState = EnemyState.Walk;
            }
        }

        private IEnumerator DirtyStunCoroutine(float duration)
        {
            _view.EnableDirtyView();
            
            yield return new WaitForSeconds(duration);
            
            _view.EnableDefaultView();
            _isStunned = false;
        }
    }
}
using System;
using System.Collections;
using Core.Interfaces;
using DG.Tweening;
using UnityEngine;

namespace Core
{
    public class Bomb : MonoBehaviour, IExplosionHandler
    {
        public event Action<Bomb> Explosion;
        
        [SerializeField] [Range(0.5f, 10f)]
        private float _lifeTime = 3f;
        [SerializeField] [Range(0.1f, 1f)]
        private float _lifeTimeAfterInterrupted = .1f;
        [SerializeField] [Range(0.1f, 3f)]
        private float _vibrateTime = 3f;
        [SerializeField] [Range(1, 10)]
        private int _distance = 4;

        private GridCell _cell;
        private AStar _pathFinder;
        private Coroutine _timerCoroutine;
        private bool _timerInterrupted;

        public int Distance => _distance;
        public GridCell Cell => _cell;

        public void Initialize(GridCell cell, AStar pathFinder)
        {
            _cell = cell;
            _pathFinder = pathFinder;
            _timerInterrupted = false;

            RunAnimation();
            RunTimer();
        }

        public void HandleExplosion(GridCell cell, int distance)
        {
            if (_timerInterrupted)
                return;

            _pathFinder.Initialize(cell.Coords.AsVector(), _cell.Coords.AsVector(), AStar.AllowedDirectionsType.EightDirections);
            var distanceBetweenCells = _pathFinder.GetPath().Count;

            if (distanceBetweenCells < distance)
                HandleExplosionInternal();
        }

        private void HandleExplosionInternal()
        {
            if(_timerCoroutine != null)
                StopCoroutine(_timerCoroutine);

            _timerInterrupted = true;
            _timerCoroutine = StartCoroutine(RunTimer(_lifeTimeAfterInterrupted));
            
            Debug.Log("Other Bomb under explosion effect");
        }

        private void RunTimer()
        {
            _timerCoroutine = StartCoroutine(RunTimer(_lifeTime));
        }

        private void RunAnimation()
        {
            transform.DOScale(1.1f, _vibrateTime)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private IEnumerator RunTimer(float time)
        {
            yield return new WaitForSeconds(time);

            OnTimerFinish();
            
            yield return new WaitForEndOfFrame();
            
            Destroy(gameObject);
        }

        private void OnTimerFinish()
        {
            transform.DOKill();
            _cell.Bomb = null;
            
            InstantiateExplosionPrefab();
            Explosion?.Invoke(this);
        }

        private void InstantiateExplosionPrefab()
        {
            Debug.Log("Explosion! Here must be implementation of spawn Explosion particles");
        }
    }
}
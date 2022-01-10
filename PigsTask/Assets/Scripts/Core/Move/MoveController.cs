using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Move
{
    public class MoveController
    {
        public event Action Changed;

        private const AStar.AllowedDirectionsType PathFinderMode = AStar.AllowedDirectionsType.FourDirections;

        private readonly AStar _pathFinder;
        private readonly Transform _targetTransform;
        private Dictionary<MoveDirection, Vector2Int> _directionsVectors;
        private readonly MoveEntityCallbacks _moveCallbacks;
        private Sequence _sequence;
        private bool _needStopFlag;
        
        public MoveDirection CurrentDirection { get; private set; }
        public Vector2Int CurrentPosition { get; private set; }
        public Vector2Int PreviousPosition { get; private set; }
        public bool IsMoving { private set; get; }
        public GridCell CurrentCell => _pathFinder.ToCell(CurrentPosition);
        public GridCell PreviousCell => _pathFinder.ToCell(PreviousPosition);

        public MoveController(Vector2Int startCoords, MoveDirection startDirection, Transform targetTransform, AStar pathFinder, MoveEntityCallbacks moveCallbacks)
        {
            _moveCallbacks = moveCallbacks;
            _pathFinder = pathFinder;

            _targetTransform = targetTransform;

            CurrentDirection = startDirection;
            CurrentPosition = startCoords;
            PreviousPosition = startCoords;

            InitializeDirectionsVectors();

            Changed?.Invoke();
        }

        public void Move(Vector2Int targetCoords, float speed, bool trackPath)
        {
            var path = GetPath(targetCoords);

            if (trackPath)
                TrackPath(path);

            MoveAlongPath(path, speed);
        }
        
        public void Move(Vector2Int targetCoords, int distance, float speed, bool trackPath)
        {
            var path = GetPath(targetCoords);
            var cutPath = path.Count <= distance ? path : path.GetRange(0, distance);

            if (trackPath)
                TrackPath(cutPath);

            MoveAlongPath(cutPath, speed);
        }

        public void Move(int distance, float speed, bool trackPath)
        {
            var path = GetRandomPath(distance);

            if (trackPath)
                TrackPath(path);

            MoveAlongPath(path, speed);
        }

        public void Stop()
        {
            if (_sequence == null)
                return;

            _needStopFlag = true;
        }

        public GridCell GetNearCellWithPriorityFront()
        {
            var priorityPosition = CurrentPosition + _directionsVectors[CurrentDirection];
            var cell = _pathFinder.ToCellOrNull(priorityPosition);

            if (cell != null)
                if (cell.IsFree)
                    return cell;

            var freeCells = new List<GridCell>();

            foreach (var pair in _directionsVectors)
            {
                if (pair.Key == CurrentDirection)
                    continue;

                var position = CurrentPosition + pair.Value;
                cell = _pathFinder.ToCellOrNull(position);

                if (cell != null)
                    if (cell.IsFree)
                        freeCells.Add(cell);
            }

            if (freeCells.Count == 0)
                return null;

            var rndIndex = Random.Range(0, freeCells.Count);
            
            return freeCells[rndIndex];
        }

        private void TrackPath(List<Vector2Int> path)
        {
            var cells = _pathFinder.ToCellPath(path);
            cells.ForEach(c => c.AnimateMarker());
        }


        private void InitializeDirectionsVectors()
        {
            _directionsVectors = new Dictionary<MoveDirection, Vector2Int>()
            {
                {MoveDirection.Left, Vector2Int.left},
                {MoveDirection.Right, Vector2Int.right},
                {MoveDirection.Up, Vector2Int.up},
                {MoveDirection.Down, Vector2Int.down},
            };
        }

        private List<Vector2Int> GetPath(Vector2Int targetCoords)
        {
            _pathFinder.Initialize(CurrentPosition, targetCoords, PathFinderMode);

            return _pathFinder.GetPath();
        }

        private List<Vector2Int> GetRandomPath(int distance)
        {
            var randomPoint = _pathFinder.GetRandomPoint(CurrentPosition);

            _pathFinder.Initialize(CurrentPosition, randomPoint, PathFinderMode);
            var fullPath = _pathFinder.GetPath();
            var distancePath = fullPath.Count <= distance ? fullPath : fullPath.GetRange(0, distance);

            return distancePath;
        }

        private void MoveAlongPath(List<Vector2Int> pathCoords, float speed)
        {
            var worldPath = _pathFinder.ToWorldPath(pathCoords);
            var cellPath = _pathFinder.ToCellPath(pathCoords);
            
            _sequence = DOTween.Sequence();

            IsMoving = true;

            for (var i = 1; i < worldPath.Count; i++)
            {
                var coords = pathCoords[i];

                var previousCell = cellPath[i - 1];
                var newCell = cellPath[i];

                var easing = GetEasing(i, worldPath.Count);

                _sequence.AppendCallback(() =>
                {
                    var directionVector = new Vector2Int(coords.x - CurrentPosition.x, -1 * coords.y + CurrentPosition.y);
                    var index = _directionsVectors.Values.ToList().FindIndex(v => v == directionVector);
                    if (index == -1)
                        throw new InvalidOperationException("Direction vector not found!");

                    if (_moveCallbacks.CheckCellIfStopFunc(newCell))
                    {
                        //Debug.Log(" ! next cell is busy ! Path interrupted. Enemy is waiting next try to walk");
                        _sequence.Kill();
                        _sequence = null;
                        
                        IsMoving = false;
                    }
                    else
                    {
                        _moveCallbacks.CellMarkAction(newCell);

                        CurrentDirection = _directionsVectors.Keys.ToList()[index];
                        Changed?.Invoke();
                    }
                });

                _sequence.Append(_targetTransform.DOMove(worldPath[i], 1f / speed).SetEase(easing));
                _sequence.AppendCallback(() =>
                {
                    _moveCallbacks.CellUnmarkAction(previousCell);

                    PreviousPosition = CurrentPosition;
                    CurrentPosition = coords;
                    Changed?.Invoke();

                    TryStopStopInternal();
                });
            }

            _sequence.AppendCallback(() =>
            {
                _sequence = null;
                IsMoving = false;
            });
        }

        private void TryStopStopInternal()
        {
            if (_needStopFlag == false)
                return;
            
            _sequence.Kill();
            _sequence = null;
            _needStopFlag = false;

            IsMoving = false;
        }

        public void CleanUp()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
            
            IsMoving = false;
            
            _moveCallbacks.CellUnmarkAction(CurrentCell);
            _moveCallbacks.CellUnmarkAction(PreviousCell);
        }

        private static Ease GetEasing(int i, int count)
        {
            var easing = Ease.Linear;

            if (i == 1)
                easing = Ease.InQuad;

            if (i == count - 1)
                easing = Ease.OutQuad;

            if (1 == count - 1)
                easing = Ease.InOutQuad;

            return easing;
        }
    }
}
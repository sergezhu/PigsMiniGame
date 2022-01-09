using System;
using System.Collections.Generic;
using System.Linq;
using Core.Spawn;
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
        private readonly EntitySpawner.EntityType _spawnerType;
        private Dictionary<MoveDirection, Vector2Int> _directionsVectors;
        private Sequence _sequence;
        private bool _needStopFlag;
        
        public MoveDirection CurrentDirection { get; private set; }
        public CellCoords CurrentPosition { get; private set; }
        public CellCoords PreviousPosition { get; private set; }
        public bool IsMoving { private set; get; }
        public GridCell CurrentCell => _pathFinder.ToCell(CurrentPosition.AsVector());
        public GridCell PreviousCell => _pathFinder.ToCell(PreviousPosition.AsVector());


        private static readonly Dictionary<EntitySpawner.EntityType, MoveEntityCallbacks> CellMoveCallbacks =
            new Dictionary<EntitySpawner.EntityType, MoveEntityCallbacks>
            {
                {
                    EntitySpawner.EntityType.Player, new MoveEntityCallbacks(
                        cell => { cell.HasPlayer = true; },
                        cell => { cell.HasPlayer = false; },
                        cell => cell.HasEnemy || cell.HasBomb)
                },

                {
                    EntitySpawner.EntityType.Farmer, new MoveEntityCallbacks(
                        cell => { cell.HasEnemy = true; },
                        cell => { cell.HasEnemy = false; },
                        cell => cell.HasPlayer || cell.HasEnemy || cell.HasBomb)
                },

                {
                    EntitySpawner.EntityType.Dog, new MoveEntityCallbacks(
                        cell => { cell.HasEnemy = true; },
                        cell => { cell.HasEnemy = false; },
                        cell => cell.HasPlayer || cell.HasEnemy || cell.HasBomb)
                },
            };

        public MoveController(CellCoords startCoords, MoveDirection startDirection, Transform targetTransform, AStar pathFinder,
            EntitySpawner.EntityType spawnerType)
        {
            _pathFinder = pathFinder;

            _targetTransform = targetTransform;
            _spawnerType = spawnerType;

            CurrentDirection = startDirection;
            CurrentPosition = startCoords;
            PreviousPosition = startCoords;

            InitializeDirectionsVectors();

            Changed?.Invoke();
        }

        public void Move(CellCoords targetCoords, float speed, bool trackPath)
        {
            var path = GetPath(targetCoords);

            if (trackPath)
                TrackPath(path);

            MoveAlongPath(path, speed);
        }
        
        public void Move(CellCoords targetCoords, int distance, float speed, bool trackPath)
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
            var priorityPosition = CurrentPosition.AsVector() + _directionsVectors[CurrentDirection];
            var cell = _pathFinder.ToCellOrNull(priorityPosition);

            if (cell != null)
                if (cell.IsFree)
                    return cell;

            var freeCells = new List<GridCell>();

            foreach (var pair in _directionsVectors)
            {
                if (pair.Key == CurrentDirection)
                    continue;

                var position = CurrentPosition.AsVector() + pair.Value;
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

        private List<Vector2Int> GetPath(CellCoords targetCoords)
        {
            _pathFinder.Initialize(CurrentPosition.AsVector(), targetCoords.AsVector(), PathFinderMode);

            return _pathFinder.GetPath();
        }

        private List<Vector2Int> GetRandomPath(int distance)
        {
            var randomPoint = _pathFinder.GetRandomPoint(CurrentPosition.AsVector());

            _pathFinder.Initialize(CurrentPosition.AsVector(), randomPoint, PathFinderMode);
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
                var newCoords = new CellCoords(coords);

                var previousCell = cellPath[i - 1];
                var newCell = cellPath[i];

                var easing = GetEasing(i, worldPath.Count);

                _sequence.AppendCallback(() =>
                {
                    var directionVector = new Vector2Int(newCoords.X - CurrentPosition.X, -newCoords.Y + CurrentPosition.Y);
                    var index = _directionsVectors.Values.ToList().FindIndex(v => v == directionVector);
                    if (index == -1)
                        throw new InvalidOperationException("Direction vector not found!");

                    if (CellMoveCallbacks[_spawnerType].CheckCellIfStopFunc(newCell))
                    {
                        //Debug.Log(" ! next cell is busy ! Path interrupted. Enemy is waiting next try to walk");
                        _sequence.Kill();
                        _sequence = null;
                        
                        IsMoving = false;
                    }
                    else
                    {
                        CellMoveCallbacks[_spawnerType].CellMarkAction(newCell);

                        CurrentDirection = _directionsVectors.Keys.ToList()[index];
                        Changed?.Invoke();
                    }
                });

                _sequence.Append(_targetTransform.DOMove(worldPath[i], 1f / speed).SetEase(easing));
                _sequence.AppendCallback(() =>
                {
                    CellMoveCallbacks[_spawnerType].CellUnmarkAction(previousCell);

                    PreviousPosition = CurrentPosition;
                    CurrentPosition = newCoords;
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
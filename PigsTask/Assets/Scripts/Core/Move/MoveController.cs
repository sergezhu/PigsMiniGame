using System;
using System.Collections.Generic;
using System.Linq;
using Core.Spawn;
using DG.Tweening;
using UnityEngine;

namespace Core.Move
{
    public class MoveController
    {
        public event Action Changed;
        
        private const AStar.AllowedDirectionsType PathFinderMode = AStar.AllowedDirectionsType.FourDirections;

        private CellCoords _startCoords;
        private MoveDirection _startDirection;

        public MoveDirection CurrentDirection { get; private set; }
        public CellCoords CurrentPosition { get; private set; }
        public bool IsMoving { get; private set; }

        private Dictionary<MoveDirection, Vector2Int> _directionsVectors;
        private AStar _pathFinder;
        private Transform _targetTransform;
        private EntitySpawner.EntityType _spawnerType;

        private static Dictionary<EntitySpawner.EntityType, MoveEntityCallbacks> _cellMoveCallbacks =
            new Dictionary<EntitySpawner.EntityType, MoveEntityCallbacks>
            {
                {EntitySpawner.EntityType.Player, new MoveEntityCallbacks(
                    cell => { cell.HasPlayer = true; },
                    cell => { cell.HasPlayer = false; },
                    cell => cell.HasPlayer)},
                
                {EntitySpawner.EntityType.Farmer, new MoveEntityCallbacks(
                    cell => { cell.HasEnemy = true; },
                    cell => { cell.HasEnemy = false; },
                    cell => cell.HasEnemy)},
                
                {EntitySpawner.EntityType.Dog, new MoveEntityCallbacks(
                    cell => { cell.HasEnemy = true; },
                    cell => { cell.HasEnemy = false; },
                    cell => cell.HasEnemy)},
                
                {EntitySpawner.EntityType.Bomb, new MoveEntityCallbacks(
                    cell => { cell.HasBomb = true; },
                    cell => { cell.HasBomb = false; },
                    cell => cell.HasBomb)},
            };

        public MoveController(CellCoords startCoords, MoveDirection startDirection, Transform targetTransform, AStar pathFinder,
            EntitySpawner.EntityType spawnerType)
        {
            _pathFinder = pathFinder;

            _startDirection = startDirection;
            _startCoords = startCoords;
            _targetTransform = targetTransform;
            _spawnerType = spawnerType;

            CurrentDirection = _startDirection;
            CurrentPosition = startCoords;

            InitializeDirectionsVectors();
        
            Changed?.Invoke();
        }

        public void Move(CellCoords targetCoords, float speed)
        {
            var path = GetPath(targetCoords);
            MoveAlongPath(path, speed);
        }
        
        public void Move(int distance, float speed)
        {
            var path = GetRandomPath(distance);
            MoveAlongPath(path, speed);
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
            var sequence = DOTween.Sequence();

            IsMoving = true;

            for (var i = 1; i < worldPath.Count; i++)
            {
                var coords = pathCoords[i];
                var newCoords = new CellCoords(coords);

                var previousCell = cellPath[i - 1];
                var newCell = cellPath[i];

                var easing = GetEasing(i, worldPath.Count);

                sequence.AppendCallback(() =>
                {
                    var directionVector = new Vector2Int(newCoords.X - CurrentPosition.X, -newCoords.Y + CurrentPosition.Y);
                    var index = _directionsVectors.Values.ToList().FindIndex(v => v == directionVector);
                    if (index == -1)
                        throw new InvalidOperationException("Direction vector not found!");

                    if (_cellMoveCallbacks[_spawnerType].CheckCellMarkFunc(newCell))
                    {
                        //Debug.Log(" ! next cell is busy ! Path interrupted. Enemy is waiting next try to walk");
                        sequence.Kill();
                        
                        IsMoving = false;
                    }

                    _cellMoveCallbacks[_spawnerType].CellMarkAction(newCell);

                    CurrentDirection = _directionsVectors.Keys.ToList()[index];
                    Changed?.Invoke();
                });
                
                sequence.Append(_targetTransform.DOMove(worldPath[i], 1f / speed).SetEase(easing));
                sequence.AppendCallback(() =>
                {
                    _cellMoveCallbacks[_spawnerType].CellUnmarkAction(previousCell);
                    
                    CurrentPosition = newCoords;
                    Changed?.Invoke();
                });
            }
            
            sequence.AppendCallback(() =>
            {
                IsMoving = false;
            });
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
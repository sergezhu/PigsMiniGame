using System.Collections.Generic;
using System.Linq;
using Core.Move;
using UnityEngine;

namespace Core
{
    public class DistanceProvider
    {
        private readonly AStar _pathFinder;
        private readonly PlayerMover _playerMover;

        public bool IsEnabled => _playerMover.IsEnabled;

        public DistanceProvider(AStar pathFinder, PlayerMover playerMover)
        {
            _pathFinder = pathFinder;
            _playerMover = playerMover;
        }

        public int GetDistanceFromEnemy(EnemyMover enemyMover)
        {
            _pathFinder.Initialize(_playerMover.CurrentPosition, enemyMover.CurrentPosition, AStar.AllowedDirectionsType.FourDirections);
            var path = _pathFinder.GetPath();

            return path.Count;
        }
        
        public bool TryGetNearestNeighborOfTarget(out Vector2Int coord, EnemyMover enemyMover)
        {
            var playerPosition =  _playerMover.CurrentPosition;

            var nearFreePositions = _pathFinder
                .GetFreeNeighbors(playerPosition)
                .Select(cell => cell.Coords)
                .ToList();

            if (nearFreePositions.Count == 0)
            {
                coord = default;
                return false;
            }

            coord = nearFreePositions.OrderBy(pos =>
            {
                _pathFinder.Initialize(pos, enemyMover.CurrentPosition, AStar.AllowedDirectionsType.FourDirections);
                return _pathFinder.GetPath().Count;
            }).First();

            return true;
        }

        public List<GridCell> GetNeighborsOfTarget(Vector2Int position)
        {
            return _pathFinder.GetNonObstacleNeighbors(position);
        }
    }
}
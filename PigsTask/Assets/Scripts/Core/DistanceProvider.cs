using Core.Move;

namespace Core
{
    public class DistanceProvider
    {
        private readonly AStar _pathFinder;
        private readonly PlayerMover _playerMover;

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
    }
}
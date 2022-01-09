using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.DebugUtils
{
    public class PathDebug
    {
        private MonoBehaviour _coroutineRunner;
        private Grid _grid;
        private AStar _pathFinder;
        private const float DurationPerOnePath = .5f;

        public PathDebug(MonoBehaviour coroutineRunner, Grid grid, AStar pathFinder)
        {
            _coroutineRunner = coroutineRunner;
            _pathFinder = pathFinder;
            _grid = grid;
        }

        public void Run()
        {
            _coroutineRunner.StartCoroutine(DebugCoroutine());
        }
        
        private IEnumerator DebugCoroutine()
        {
            var start = Vector2Int.zero;

            var waiter1 = new WaitForSeconds(DurationPerOnePath);
            var waiter2 = new WaitForEndOfFrame();

            for (var x = 0; x < _grid.SizeX; x++)
            {
                var end = new Vector2Int(x, _grid.SizeY - 1);
                var path = ShowPath(start, end, AStar.AllowedDirectionsType.FourDirections);
                yield return waiter1;
                HidePath(path);
                yield return waiter2;
            }
            
            for (var y = _grid.SizeY - 1; y > 0; y--)
            {
                var end = new Vector2Int(_grid.SizeX - 1, y);
                
                var path = ShowPath(start, end, AStar.AllowedDirectionsType.FourDirections);
                yield return waiter1;
                HidePath(path);
                yield return waiter2;
            }

            for (var x = 0; x < _grid.SizeX; x++)
            {
                var end = new Vector2Int(x, _grid.SizeY - 1);
                var path = ShowPath(start, end, AStar.AllowedDirectionsType.EightDirections);
                yield return waiter1;
                HidePath(path);
                yield return waiter2;
            }

            for (var y = _grid.SizeY - 1; y > 0; y--)
            {
                var end = new Vector2Int(_grid.SizeX - 1, y);
                
                var path = ShowPath(start, end, AStar.AllowedDirectionsType.EightDirections);
                yield return waiter1;
                HidePath(path);
                yield return waiter2;
            }
        }

        private List<Vector2Int> ShowPath(Vector2Int start, Vector2Int end, AStar.AllowedDirectionsType allowedDirectionsType)
        {
            _pathFinder.Initialize(start, end, allowedDirectionsType);
            var path = _pathFinder.GetPath();
            path.ForEach(coords =>
            {
                var cell = _grid.GetCell(coords);
                cell.Enable();
            });
            return path;
        }

        private void HidePath(List<Vector2Int> path)
        {
            path.ForEach(coords =>
            {
                var cell = _grid.GetCell(coords);
                cell.Disable();
            });
        }
    }
}
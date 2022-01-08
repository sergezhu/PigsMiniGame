using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.Spawn;
using UI;
using UnityEngine;

namespace Core
{
    public class EntryPoint : MonoBehaviour
    {
        private const float DurationPerOnePath = .5f;

        [SerializeField]
        private Grid _grid;
        [SerializeField]
        private SpawnController _spawnController;
        [SerializeField]
        private UIController _uiController;
        [SerializeField]
        private RayCaster2D _rayCaster;

        private AStar _aStarPathFinder;
        private IAssetProvider _assetProvider;
        private InputController _inputController;
        
        private bool _isUpdateReady = false;


        private async void Awake()
        {
            _inputController = new InputController(_rayCaster);

            _aStarPathFinder = new AStar(_grid);

            _assetProvider = new AssetProvider();
            _assetProvider.Initialize();

            _grid.DisableCells();
            
            _spawnController.Initialize(_grid, _assetProvider, _aStarPathFinder, _inputController);
            await _spawnController.SpawnEnemies();
            await _spawnController.SpawnPlayer();
            _spawnController.LateInitialize();

            _isUpdateReady = true;
            
            _uiController.Initialize(_spawnController.Player);

            //StartCoroutine(PathDebug());
        }

        private void Update()
        {
            if (_isUpdateReady == false)
                return;
            
            _inputController.DoUpdate();
            _spawnController.Player.DoUpdate();
            _spawnController.Enemies.ToList().ForEach(e => e.DoUpdate());
        }

        private IEnumerator PathDebug()
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
            _aStarPathFinder.Initialize(start, end, allowedDirectionsType);
            var path = _aStarPathFinder.GetPath();
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
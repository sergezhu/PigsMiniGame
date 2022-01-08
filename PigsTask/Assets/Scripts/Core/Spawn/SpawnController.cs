using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.Move;
using Core.View;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core.Spawn
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField]
        private Transform _spawnContainer;
        [SerializeField]
        private List<SpawnCellRecord> _spawnersRecords;

        private Grid _grid;
        private Player _player;
        private List<Enemy> _enemies;
        private IAssetProvider _assetProvider;
        private InputController _inputController;
        private AStar _pathFinder;

        public Player Player => _player;
        public IEnumerable<Enemy> Enemies => _enemies;

        public void Initialize(Grid grid, IAssetProvider assetProvider, AStar pathFinder, InputController inputController)
        {
            _inputController = inputController;
            _grid = grid;
            _assetProvider = assetProvider;
            _pathFinder = pathFinder;

            FillSpawnRecords();
        }

        public async Task SpawnEnemies()
        {
            var types = new[] {EntitySpawner.EntityType.Farmer, EntitySpawner.EntityType.Dog};
            _enemies = new List<Enemy>();

            var spawnRecords = _spawnersRecords
                .Where(record => types.Contains(record.Spawner.Type))
                .ToList();

            foreach (var record in spawnRecords)
            {
                record.Spawner.Initialize(_assetProvider, _spawnContainer);

                var spawnTask = record.Spawner.Spawn();
                
                await spawnTask;
                var enemy = spawnTask.Result.GetComponent<Enemy>();

                var moveController = new MoveController(record.Cell.Coords, GetRandomDirection(), enemy.transform, _pathFinder, record.Spawner.Type);
                await enemy.Initialize(moveController, _assetProvider);
                
                _enemies.Add(enemy);
            }
            
            /*_spawnersRecords
                .Where(record => types.Contains(record.Spawner.Type))
                .ToList()
                .ForEach(async record =>
                {
                    record.Spawner.Initialize(_assetProvider, _spawnContainer);

                    var spawnTask = record.Spawner.Spawn();
                    await spawnTask;

                    var moveController = new MoveController(record.Cell.Coords, GetRandomDirection(), _pathFinder);

                    var enemy = spawnTask.Result.GetComponent<Enemy>();
                    await enemy.Initialize(moveController, _assetProvider, record.Cell.Y + 1);
                    enemy.SetDirection(GetRandomDirection());
                });*/
        }

        public async Task SpawnPlayer()
        {
            var types = new []{EntitySpawner.EntityType.Player};
            
            var spawnRecords = _spawnersRecords
                .Where(record => types.Contains(record.Spawner.Type))
                .ToList();

            if (spawnRecords.Count == 0)
                throw new InvalidOperationException("You dont have Player spawners");

            var randomSpawnerIndex = Random.Range(0, spawnRecords.Count);
            var spawnCellRecord = spawnRecords[randomSpawnerIndex];
            spawnCellRecord.Spawner.Initialize(_assetProvider, _spawnContainer);
            
            var spawnTask = spawnCellRecord.Spawner.Spawn();
            await spawnTask;

            var player = spawnTask.Result.GetComponent<Player>();
            var moveController = new MoveController(spawnCellRecord.Cell.Coords, MoveDirection.Right, player.transform, _pathFinder, spawnCellRecord.Spawner.Type);
            await player.Initialize(moveController, _inputController, _assetProvider, spawnCellRecord.Cell.Y + 1);

            _player = player;
        }

        public void LateInitialize()
        {
            _enemies.ForEach(e => e.CreateDistanceProvider(_pathFinder, _player.PlayerMover));
        }

        private void FillSpawnRecords()
        {
            _spawnersRecords = new List<SpawnCellRecord>();
            
            var cells = _grid.Cells.Select(data => data.Cell).ToList();
            cells.ForEach(cell =>
            {
                if (cell.TryGetComponent(out EntitySpawner spawner)) 
                    _spawnersRecords.Add(new SpawnCellRecord(cell, spawner));
            });
        }

        private MoveDirection GetRandomDirection()
        {
            var directions = new[] {MoveDirection.Left, MoveDirection.Right, MoveDirection.Down, MoveDirection.Up};
            var randomDirectionIndex = Random.Range(0, directions.Length);
            return directions[randomDirectionIndex];
        }
    }
}
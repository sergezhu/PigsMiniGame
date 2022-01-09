using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.Interfaces;
using Core.Move;
using Core.SO;
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
        private List<IExplosionHandler> _explosionHandlers;
        private List<IEarnScoresProvider> _earnScoresProviders;
        private IAssetProvider _assetProvider;
        private InputController _inputController;
        private EarnScoresSettings _earnScoresSettings;

        public Player Player => _player;
        public IEnumerable<Enemy> Enemies => _enemies;
        public IEnumerable<IExplosionHandler> ExplosionHandlers => _explosionHandlers;
        public IEnumerable<IEarnScoresProvider> EarnScoresProviders => _earnScoresProviders;

        public void Initialize(Grid grid, IAssetProvider assetProvider, InputController inputController, EarnScoresSettings earnScoresSettings)
        {
            _grid = grid;
            _inputController = inputController;
            _assetProvider = assetProvider;
            _earnScoresSettings = earnScoresSettings;

            _explosionHandlers = new List<IExplosionHandler>();
            _earnScoresProviders = new List<IEarnScoresProvider>();

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

                var pathFinder = new AStar(_grid);
                var moveController = new MoveController(record.Cell.Coords, GetRandomDirection(), enemy.transform, pathFinder, record.Spawner.Type);
                var scores = _earnScoresSettings.EntityTypeScores.First(settings => settings.EntityType == record.Spawner.Type).EarnScores;
                await enemy.Initialize(moveController, _assetProvider, pathFinder, scores);
                
                _enemies.Add(enemy);
                
                if(enemy is IExplosionHandler handler)
                    _explosionHandlers.Add(handler);
                
                if(enemy is IEarnScoresProvider earnScoresProvider)
                    _earnScoresProviders.Add(earnScoresProvider);
            }
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
            var pathFinder = new AStar(_grid);
            var moveController = new MoveController(spawnCellRecord.Cell.Coords, MoveDirection.Right, player.transform, pathFinder, spawnCellRecord.Spawner.Type);
            await player.Initialize(moveController, _inputController, _assetProvider, spawnCellRecord.Cell.Y + 1, this, pathFinder, new Health(10));

            _player = player;
            
            if(_player is IExplosionHandler handler)
                _explosionHandlers.Add(handler);
        }

        public async Task SpawnBomb(GridCell targetBombCell)
        {
            var spawner = targetBombCell.gameObject.AddComponent<EntitySpawner>();
            spawner.Initialize(_assetProvider, _spawnContainer, EntitySpawner.EntityType.Bomb);

            var spawnTask = spawner.Spawn();
            await spawnTask;

            var bomb = spawnTask.Result.GetComponent<Bomb>();
            var pathFinder = new AStar(_grid);
            bomb.Initialize(targetBombCell, pathFinder);

            SubscribeOnExplosionEvent(bomb);
            
            if(bomb is IExplosionHandler handler)
                _explosionHandlers.Add(handler);
        }

        private void SubscribeOnExplosionEvent(Bomb bomb) => 
            bomb.Explosion += OnBombExplosion;

        private void OnBombExplosion(Bomb bomb)
        {
            bomb.Explosion -= OnBombExplosion;

            _explosionHandlers.Remove(bomb);
            _explosionHandlers.ForEach(handler => handler.HandleExplosion(bomb.Cell, bomb.Distance));
        }

        public void LateInitialize()
        {
            _enemies.ForEach(e =>
            {
                var pathFinder = new AStar(_grid);
                e.CreateDistanceProvider(pathFinder, _player.PlayerMover);
            });
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Infrastructure.AssetManagement;
using UnityEngine;

namespace Core.Spawn
{
    [RequireComponent(typeof(GridCell))]
    public class EntitySpawner : MonoBehaviour
    {
        public enum EntityType
        {
            Player,
            Farmer,
            Dog,
            Eat,
            Bomb
        }

        [SerializeField]
        private EntityType _type;

        private static Dictionary<EntityType, string> _entityAddresses = new Dictionary<EntityType, string>()
        {
            {EntityType.Player, AssetAddress.PlayerPrefab},
            {EntityType.Farmer, AssetAddress.FarmerPrefab},
            {EntityType.Dog, AssetAddress.DogPrefab},
            {EntityType.Bomb, AssetAddress.BombPrefab},
            {EntityType.Eat, AssetAddress.EatPrefab},
        };

        private GridCell _gridCell;
        private IAssetProvider _assetProvider;
        private Transform _spawnContainer;
        private EntityType[] _enemiesTypes;
        private EntityType[] _eatTypes;

        public EntityType Type => _type;

        public void Initialize(IAssetProvider assetProvider, Transform spawnContainer)
        {
            _spawnContainer = spawnContainer;
            _assetProvider = assetProvider;
            _gridCell = GetComponent<GridCell>();

            _enemiesTypes = new[] {EntityType.Farmer, EntityType.Dog};
            _eatTypes = new[] {EntityType.Eat};
        }
        
        public void Initialize(IAssetProvider assetProvider, Transform spawnContainer, EntityType entityType)
        {
            _spawnContainer = spawnContainer;
            _assetProvider = assetProvider;
            _type = entityType;
            _gridCell = GetComponent<GridCell>();

            _enemiesTypes = new[] {EntityType.Farmer, EntityType.Dog};
            _eatTypes = new[] {EntityType.Eat};
        }

        public async Task<GameObject> Spawn()
        {
            var address = _entityAddresses[_type];
            var worldPosition = _gridCell.WorldPosition;

            if (_gridCell.IsObstacle)
                throw new InvalidOperationException("You can not spawn on cell that is Obstacle");

            return await _assetProvider.Instantiate(address, worldPosition, _spawnContainer);;
        }
    }
}
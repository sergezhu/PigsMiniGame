using System;
using System.Collections.Generic;
using Core.Extensions;
using Core.Infrastructure.AssetManagement;
using UnityEngine;

namespace Core.Spawn
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField]
        private Obstacle _obstaclePrefab;
        
        private IAssetProvider _assetProvider;
        private Transform _obstaclesParent;

        public void Initialize(Transform obstaclesParent, IAssetProvider assetProvider)
        {
            _obstaclesParent = obstaclesParent;
            _assetProvider = assetProvider;
        }

        public void RefreshObstacles(List<GridCell> cells)
        {
            if (cells == null)
                throw new NullReferenceException("Cells list is null");

            _obstaclesParent.DestroyImmediateAllChildren();

            foreach (var cell in cells)
            {
                if(cell.IsObstacle == false)
                    continue;

                var obstaclePosition = cell.WorldPosition;
                //var obstacle = _assetProvider.Instantiate(AssetAddress.StonePrefab, obstaclePosition, _obstaclesParent);
                var obstacle = Instantiate(_obstaclePrefab, obstaclePosition, Quaternion.identity, _obstaclesParent);
                var obstacleRenderer = obstacle.GetComponent<SpriteRenderer>();
                obstacleRenderer.sortingOrder = cell.Coords.Y + 1;
            }
        }
    }
}
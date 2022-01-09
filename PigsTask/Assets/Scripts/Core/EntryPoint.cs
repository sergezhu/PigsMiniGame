using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Infrastructure.AssetManagement;
using Core.InputControl;
using Core.SO;
using Core.Spawn;
using UI;
using UnityEngine;

namespace Core
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField]
        private Grid _grid;
        [SerializeField]
        private SpawnController _spawnController;
        [SerializeField]
        private UIController _uiController;
        [SerializeField]
        private RayCaster2D _rayCaster;
        [SerializeField]
        private EarnScoresSettings _earnScoresSettings;

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
            
            _spawnController.Initialize(_grid, _assetProvider, _aStarPathFinder, _inputController, _earnScoresSettings);
            await _spawnController.SpawnEnemies();
            await _spawnController.SpawnPlayer();
            _spawnController.LateInitialize();

            _isUpdateReady = true;
            
            _uiController.Initialize(_spawnController.Player, _spawnController.EarnScoresProviders);
        }

        private void Update()
        {
            if (_isUpdateReady == false)
                return;
            
            _inputController.DoUpdate();
            _spawnController.Player.DoUpdate();
            _spawnController.Enemies.ToList().ForEach(e => e.DoUpdate());

            //_grid.UpdateCellsView();
        }
    }
}
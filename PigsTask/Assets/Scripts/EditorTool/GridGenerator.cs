using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Extensions;
using Core.Infrastructure.AssetManagement;
using Core.Spawn;
using UnityEngine;
using Grid = Core.Grid;

public class GridGenerator : MonoBehaviour
{
    [SerializeField]
    private Grid _grid;
    [SerializeField]
    private ObstacleSpawner _obstacleSpawner;
    [SerializeField]
    private Transform _cellsContainer;
    [SerializeField]
    private Transform _obstaclesContainer;
    [SerializeField]
    private GridCell _gridCellPrefab;
    
    [Space]
    [SerializeField]
    private Transform _leftTopCorner;
    [SerializeField]
    private Transform _leftBottomCorner;
    [SerializeField]
    private Transform _rightTopCorner;
    [SerializeField]
    private Transform _rightBottomCorner;

    [Space]
    [SerializeField][Min(2)]
    private int _sizeX = 17;
    [SerializeField][Min(2)]
    private int _sizeY = 9;

    private IAssetProvider _assetProvider;

    private IAssetProvider AssetsProvider => _assetProvider ??= new AssetProvider();
    

    [ContextMenu("Generate")]
    private void Generate()
    {
        ObstacleSpawnerInitialize();
        
        ClearChildren();
        CreateCells();
    }
    
    [ContextMenu("Obstacles Configuration Setup")]
    private void ObstaclesConfigurationSetup()
    {
        _grid.Cells.ToList().ForEach(data =>
        {
            if((data.Coords.X % 2) != 0 && (data.Coords.Y % 2) != 0)
                data.Cell.SetAsObstacle();
            else
                data.Cell.SetAsDefault();
        });
    }

    [ContextMenu("Refresh Obstacles")]
    private void RefreshObstacles()
    {
        ObstacleSpawnerInitialize();
        _obstacleSpawner.RefreshObstacles(_grid.Cells.Select(data => data.Cell).ToList());
    }

    [ContextMenu("Update World Coords")]
    private void UpdateWorldCoords()
    {
        var cells = _grid.Cells.ToList();
        
        for (int i = 0; i < cells.Count; i++)
        {
            var row = cells[i].Coords.Y;
            
            var leftBound = Vector3.Lerp(_leftTopCorner.position, _leftBottomCorner.position, (float) row / (_sizeY - 1));
            var rightBound = Vector3.Lerp(_rightTopCorner.position, _rightBottomCorner.position, (float) row / (_sizeY - 1));

            var col = cells[i].Coords.X;
            
            var cellWorldCoord = Vector3.Lerp(leftBound, rightBound, (float) col / (_sizeX - 1));
            cells[i].Cell.Initialize(col, row, cellWorldCoord);
        }
        
        _grid.Initialize(_sizeX, _sizeY, cells);
    }

    private void ObstacleSpawnerInitialize()
    {
        _obstacleSpawner.Initialize(_obstaclesContainer, AssetsProvider);
    }

    private void CreateCells()
    {
        var cells = new List<CellData>();
        
        for (int row = 0; row < _sizeY; row++)
        {
            var leftBound = Vector3.Lerp(_leftTopCorner.position, _leftBottomCorner.position, (float) row / (_sizeY - 1));
            var rightBound = Vector3.Lerp(_rightTopCorner.position, _rightBottomCorner.position, (float) row / (_sizeY - 1));

            for (int col = 0; col < _sizeX; col++)
            {
                var cellWorldCoord = Vector3.Lerp(leftBound, rightBound, (float) col / (_sizeX - 1));
                var cell = CreateCell(cellWorldCoord);
                cell.Initialize(col, row, cellWorldCoord);

                var cellCoords = new CellCoords(col, row);
                cells.Add(new CellData(cellCoords, cell));
            }
        }
        
        _grid.Initialize(_sizeX, _sizeY, cells);
    }

    private GridCell CreateCell(Vector3 position)
    {
        Debug.Log($"{_gridCellPrefab == null}  {_cellsContainer == null}");
        var cell = Instantiate(_gridCellPrefab, _cellsContainer);
        var cellTransform = cell.transform;
        cellTransform.position = position;

        return cell;
    }

    private void ClearChildren()
    {
        _cellsContainer.DestroyImmediateAllChildren();
        _grid.ClearCells();
    }
}

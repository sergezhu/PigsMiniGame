using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Core
{
    public class AStar
    {
        public enum AllowedDirectionsType
        {
            FourDirections,
            EightDirections
        }
        
        private readonly Grid _grid;
        private HashSet<GridCell> _openList;
        private HashSet<GridCell> _closedList;
        private GridCell _startCell;
        private GridCell _endCell;
        private GridCell _currentCell;
        private AllowedDirectionsType _allowedDirectionsType;
        
        private readonly List<Vector2Int> _fourDirections;
        private readonly List<Vector2Int> _eightDirections;
        private List<Vector2Int> _currentAllowedDirections;
        private Stack<Vector2Int> _path;

        public AStar(Grid grid)
        {
            _grid = grid;

            _fourDirections = new List<Vector2Int>()
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down,
            };
            
            _eightDirections = new List<Vector2Int>()
            {
                Vector2Int.left,
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.right + Vector2Int.up,
                Vector2Int.right + Vector2Int.down,
                Vector2Int.left + Vector2Int.up,
                Vector2Int.left + Vector2Int.down,
            };
        }

        public void Initialize(Vector2Int startCoord, Vector2Int endCoord, AllowedDirectionsType allowedDirectionsType)
        {
            _allowedDirectionsType = allowedDirectionsType;
            _currentAllowedDirections = _allowedDirectionsType == AllowedDirectionsType.FourDirections ? _fourDirections : _eightDirections; 
            
            _startCell = _grid.GetCell(startCoord);
            _endCell = _grid.GetCell(endCoord);
            
            //Debug.Log($"start : {_startCell.Coords.AsVector()} , end : {_endCell.Coords.AsVector()}");
            
            _openList = new HashSet<GridCell>();
            _openList.Add(_startCell);

            _closedList = new HashSet<GridCell>();
            
            _currentCell = _startCell;
            _path = null;
        }

        public List<Vector2Int> GetPath()
        {
            if (_currentCell == null)
                throw new InvalidOperationException("Initialize you this instance before.");

            while (_openList.Count > 0 && _path == null)
            {
                var neighbors = FindNeighbors(_currentCell.Coords.AsVector());
                HandleNeighbors(neighbors, _currentCell);
                HandleCurrent(_currentCell);

                _path = TryCreatePath(_currentCell);
            }

            return _path == null ? new List<Vector2Int>() : _path.ToList();
        }
        
        public Vector2Int GetRandomPoint(Vector2Int current, bool ignoreObstacles = true)
        {
            var cells = _grid
                .Cells
                .Select(data => data.Cell)
                .ToList()
                .Where(c => c.Coords.AsVector() != current)
                .ToList();

            if (ignoreObstacles)
                cells = cells
                    .Where(cell => cell.IsObstacle == false)
                    .ToList();

            var rndIndex = Random.Range(0, cells.Count);

            return cells[rndIndex].Coords.AsVector();
        }

        public List<Vector3> ToWorldPath(List<Vector2Int> path)
        {
            return path.Select(ToWorldPoint).ToList();
        }

        public Vector3 ToWorldPoint(Vector2Int coords)
        {
            var cell = _grid.GetCell(coords);
            if (cell == null)
                throw new InvalidOperationException("Invalid path point. Cell with this coords is not exist");
                
            return cell.WorldPosition;
        }
        
        public List<GridCell> ToCellPath(List<Vector2Int> path)
        {
            return path.Select(ToCell).ToList();
        }
        
        public GridCell ToCell(Vector2Int coords)
        {
            var cell = ToCellOrNull(coords);
            if (cell == null)
                throw new InvalidOperationException("Invalid path point. Cell with this coords is not exist");
                
            return cell;
        }
        
        public GridCell ToCellOrNull(Vector2Int coords) => 
            _grid.GetCell(coords);

        private List<GridCell> FindNeighbors(Vector2Int parentCoords)
        {
            var neighbors = new List<GridCell>();
            
            _currentAllowedDirections.ForEach(direction =>
            {
                var cell = _grid.GetCell(parentCoords + direction);
                if(cell != null)
                    neighbors.Add(cell);
            });

            return neighbors;
        }

        private void HandleNeighbors(List<GridCell> neighbors, GridCell current)
        {
            foreach (var neighbor in neighbors)
            {
                var gScore = DetermineGScore(neighbor.Coords.AsVector(), current.Coords.AsVector());
                
                if(IsAllowPassDiagonally(current, neighbor) == false)
                    continue;

                if (_openList.Contains(neighbor))
                {
                    if (current.G + gScore < neighbor.G) 
                        CalcNeighborValues(current, neighbor, gScore);
                }
                else if (_closedList.Contains(neighbor) == false && neighbor.IsObstacle == false)
                {
                    CalcNeighborValues(current, neighbor, gScore);
                    _openList.Add(neighbor);
                }
            }
        }

        private void HandleCurrent(GridCell currentCell)
        {
            _openList.Remove(currentCell);
            _closedList.Add(currentCell);

            if (_openList.Count > 0)
            {
                var cellWithMinF = _openList.OrderBy(cell => cell.F).First();
                var cellsWithSameF = _openList.Where(cell => cell.F == cellWithMinF.F).ToList();

                var randomIndex = Random.Range(0, cellsWithSameF.Count);
                _currentCell = cellsWithSameF[randomIndex];
            }
        }

        private int DetermineGScore(Vector2Int neighbor, Vector2Int current)
        {
            int x = neighbor.x - current.x;
            int y = neighbor.y - current.y;

            return Math.Abs(x - y) % 2 == 1 ? 10 : 14;
        }
        
        private int DetermineHScore(Vector2Int neighbor, Vector2Int end)
        {
            int x = Math.Abs(neighbor.x - end.x);
            int y = Math.Abs(neighbor.y - end.y);

            return (x + y) * 10;
        }

        private void CalcNeighborValues(GridCell parent, GridCell neighbor, int cost)
        {
            neighbor.Parent = parent;
            neighbor.G = parent.G + cost;
            neighbor.H = DetermineHScore(neighbor.Coords.AsVector(), _endCell.Coords.AsVector());
            neighbor.F = neighbor.G + neighbor.H;
        }

        private Stack<Vector2Int> TryCreatePath(GridCell current)
        {
            if (current.Coords.AsVector() == _endCell.Coords.AsVector())
            {
                var finalPath = new Stack<Vector2Int>();

                while (current.Coords.AsVector() != _startCell.Coords.AsVector())
                {
                    finalPath.Push(current.Coords.AsVector());
                    current = current.Parent;
                }

                finalPath.Push(current.Coords.AsVector());
                return finalPath;
            }

            return null;
        }

        private bool IsAllowPassDiagonally(GridCell parent, GridCell neighbor)
        {
            var direction = parent.Coords.AsVector() - neighbor.Coords.AsVector();
            var first = new Vector2Int(parent.Coords.AsVector().x - direction.x, parent.Coords.Y);
            var second = new Vector2Int(parent.Coords.AsVector().x , parent.Coords.Y - direction.y);

            return _grid.GetCell(first).IsObstacle == false && _grid.GetCell(second).IsObstacle == false;
        }
    }
}
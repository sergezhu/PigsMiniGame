using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] 
        private List<CellData> _cells;
        [SerializeField]
        private int _sizeX;
        [SerializeField]
        private int _sizeY;

        private GridCell[,] _cellsArray;

        public IEnumerable<CellData> Cells => _cells;
        public int SizeX => _sizeX;
        public int SizeY => _sizeY;

        public void Initialize(int sizeX, int sizeY, List<CellData> cells)
        {
            _sizeY = sizeY;
            _sizeX = sizeX;
            _cells = cells.ToList();
        }

        public void InitializeArray()
        {
            _cellsArray = new GridCell[_sizeX, _sizeY];
            _cells.ForEach(data => _cellsArray[data.Coords.x, data.Coords.y] = data.Cell);
        }

        public GridCell GetCell(Vector2Int coords)
        {
            if (coords.x < 0 || coords.x >= _sizeX || coords.y < 0 || coords.y >= _sizeY)
                return null;
            
            return _cellsArray[coords.x, coords.y];
        }

        public void ClearCells() => _cells.Clear();

        public void EnableCells() => 
            _cells.ForEach(data => data.Cell.Enable());
        
        public void DisableCells() => 
            _cells.ForEach(data => data.Cell.Disable());

        public void UpdateCellsView() => 
            _cells.ForEach(data => data.Cell.UpdateView());
    }
}
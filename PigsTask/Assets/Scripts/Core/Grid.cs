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

        public IEnumerable<CellData> Cells => _cells;
        public int SizeX => _sizeX;
        public int SizeY => _sizeY;

        public void Initialize(int sizeX, int sizeY, List<CellData> cells)
        {
            _sizeY = sizeY;
            _sizeX = sizeX;
            _cells = cells.ToList();
        }

        public GridCell GetCell(Vector2Int coords)
        {
            var firstData = Cells.FirstOrDefault(data => data.Coords.X == coords.x && data.Coords.Y == coords.y);
            return firstData?.Cell;
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
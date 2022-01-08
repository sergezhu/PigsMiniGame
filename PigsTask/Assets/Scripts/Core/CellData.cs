using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class CellData
    {
        [SerializeField]
        private CellCoords _coords;
        [SerializeField]
        private GridCell _cell;

        public CellData(CellCoords coords, GridCell cell)
        {
            _coords = coords;
            _cell = cell;
        }

        public CellCoords Coords => _coords;
        public GridCell Cell => _cell;
    }
}
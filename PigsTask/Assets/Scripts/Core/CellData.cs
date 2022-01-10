using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class CellData
    {
        [SerializeField]
        private Vector2Int _coords;
        [SerializeField]
        private GridCell _cell;

        public CellData(Vector2Int coords, GridCell cell)
        {
            _coords = coords;
            _cell = cell;
        }

        public Vector2Int Coords => _coords;
        public GridCell Cell => _cell;
    }
}
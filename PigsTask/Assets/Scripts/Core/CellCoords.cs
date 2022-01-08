using System;
using UnityEngine;

namespace Core
{
    [Serializable]
    public struct CellCoords
    {
        [SerializeField]
        private int _x;
        [SerializeField]
        private int _y;

        public CellCoords(int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentException("X or Y less than zero");
            
            _x = x;
            _y = y;
        }
        
        public CellCoords(Vector2Int v)
        {
            if (v.x < 0 || v.y < 0)
                throw new ArgumentException("X or Y less than zero");
            
            _x = v.x;
            _y = v.y;
        }

        public int X => _x;
        public int Y => _y;

        public Vector2Int AsVector() =>
            new Vector2Int(_x, _y);
    }
}